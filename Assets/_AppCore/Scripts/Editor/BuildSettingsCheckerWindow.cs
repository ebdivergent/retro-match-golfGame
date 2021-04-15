using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AppCore
{
    public class BuildSettingsCheckerWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        private BuildCheckerSettings _settings => BuildCheckerSettings.Instance;

        [MenuItem("AppCore/Build Settings Checker")]
        public static void ShowWindow()
        {
            var window = GetWindow<BuildSettingsCheckerWindow>("Build Settings Checker", true, typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"));

            window.Init();
        }

        private void OnFocus()
        {
            EditorUtility.SetDirty(_settings);
        }

        private void OnLostFocus()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Init()
        {
            Fetch();
        }

        private void Fetch()
        {
            //_settings = BuildCheckerSettings.Instance;
        }

        private void OnGUI()
        {
            if (AppCoreSettings.Instance == null)
                return;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            DrawOwnSettings();

            if (_settings.SelectedPreferences)
            {
                DrawSharedSettings();
                DrawPlatformButtons();
                DrawPlatformSettings(_settings.selectedBuildTarget);
                DrawSDKsSettings();
            }
            else
            {
                EditorGUILayout.HelpBox("Selected type of preferences is null.", MessageType.Warning);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
        }

        private void DrawOwnSettings()
        {
            EditorGUIExtensions.DrawHeader("Internal");
            DrawBuildType();
            DrawLinkToPreferences();
        }

        private void DrawBuildType()
        {
            EditorGUI.BeginChangeCheck();

            var prefType = (BuildPreferencesType)EditorGUILayout.EnumPopup("Build Type", _settings.selectedPrefType);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_settings, "Edited Build Preferences");

                _settings.selectedPrefType = prefType;
            }

            if (prefType != BuildPreferencesType.Release && EditorGUIExtensions.DrawFixHelpBox("Selected non-release build type.", "Switch to release"))
            {
                Undo.RecordObject(_settings, "Edited Build Preferences");
                _settings.selectedPrefType = BuildPreferencesType.Release;
            }
        }

        private void DrawLinkToPreferences()
        {
            EditorGUI.BeginChangeCheck();

            var pref = (BuildPreferences)EditorGUILayout.ObjectField(_settings.GetPreferences(_settings.selectedPrefType), typeof(BuildPreferences), false);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_settings, "Edited Build Preferences");

                _settings.SetPreferences(_settings.selectedPrefType, pref);
            }

            if (GUILayout.Button("Edit Preferences"))
            {
                Selection.activeObject = _settings.SelectedPreferences;

                EditorApplication.ExecuteMenuItem("Window/General/Inspector");
            }
        }

        private void DrawSharedSettings()
        {
            EditorGUIExtensions.DrawHeader("Application");
            DrawProWarning();
            DrawAppName();
            DrawCompany();
            DrawVersion();
            DrawIcon();
            EditorGUIExtensions.DrawHeader("Splash screen");
            DrawSplashLogos();
            DrawSplashBackgroundColor();
            EditorGUIExtensions.DrawHeader("Orientation");
            DrawOrientationMode();
        }

        private void DrawProWarning()
        {
            if (_settings.SelectedPreferences.unityPro.Required && !UnityEditorInternal.InternalEditorUtility.HasPro())
            {
                EditorGUILayout.HelpBox("Editor doesn't have Unity Pro license!", MessageType.Warning);
            }
        }

        private void DrawAppName()
        {
            EditorGUI.BeginChangeCheck();

            var value = EditorGUILayout.TextField("Application", PlayerSettings.productName);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.productName = value;
            }
        }

        private void DrawCompany()
        {
            EditorGUI.BeginChangeCheck();

            var value = EditorGUILayout.TextField("Company", PlayerSettings.companyName);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.companyName = value;
            }

            if (_settings.SelectedPreferences.company.Required
                && _settings.SelectedPreferences.company.name != value
                && EditorGUIExtensions.DrawFixHelpBox("Company name doesn't match with preferences."))
            {
                PlayerSettings.companyName = _settings.SelectedPreferences.company.name;
            }
        }

        private void DrawVersion()
        {
            EditorGUI.BeginChangeCheck();

            var versionString = EditorGUILayout.TextField("Version", PlayerSettings.bundleVersion);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.bundleVersion = versionString;
            }
        }

        private void DrawIcon()
        {
            var totalIcons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
            Texture2D icon = totalIcons.FirstOrDefault();

            EditorGUI.BeginChangeCheck();

            var newIcon = EditorGUIExtensions.TextureField("Default Icon", icon);

            if (EditorGUI.EndChangeCheck())
            {
                if (totalIcons.Length <= 0)
                {
                    totalIcons = new Texture2D[] { newIcon };
                }
                else
                {
                    totalIcons[0] = newIcon;
                }

                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, totalIcons);
            }

            if (_settings.SelectedPreferences.icon.Required && newIcon == null)
            {
                EditorGUILayout.HelpBox("App has no icon.", MessageType.Warning);
            }
        }

        private void DrawSplashLogos()
        {
            var logos = PlayerSettings.SplashScreen.logos.ToList();

            for (int i = 0; i < logos.Count; i++)
            {
                var logo = logos[i];

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                var newLogo = EditorGUIExtensions.SpriteField($"Logo {i}", logo.logo);

                if (_settings.SelectedPreferences.splash.Required && !_settings.SelectedPreferences.splash.logos.Contains(logo.logo))
                {
                    EditorGUILayout.HelpBox("This logo wasn't expected by preferences.", MessageType.Warning);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    logo.logo = newLogo;

                    logos[i] = logo;

                    PlayerSettings.SplashScreen.logos = logos.ToArray();
                }

                if (GUILayout.Button("Remove"))
                {
                    logos.RemoveAt(i--);

                    PlayerSettings.SplashScreen.logos = logos.ToArray();
                }


                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add logo"))
            {
                logos.Add(new PlayerSettings.SplashScreenLogo());

                PlayerSettings.SplashScreen.logos = logos.ToArray();
            }

            foreach (var prefLogo in _settings.SelectedPreferences.splash.logos)
            {
                if (prefLogo == null)
                    continue;

                bool found = false;

                foreach (var existingLogo in logos)
                {
                    if (existingLogo.logo == prefLogo)
                        found = true;
                }

                if (!found)
                {
                    if (_settings.SelectedPreferences.splash.Required && EditorGUIExtensions.DrawFixHelpBox($"Logo {prefLogo.name} wasn't added."))
                    {
                        var newLogo = new PlayerSettings.SplashScreenLogo();

                        newLogo.logo = prefLogo;

                        logos.Add(newLogo);

                        PlayerSettings.SplashScreen.logos = logos.ToArray();
                    }
                }
            }

            if (_settings.SelectedPreferences.splash.Required && (logos == null || logos.Count <= 0))
            {
                EditorGUILayout.HelpBox("App has logos.", MessageType.Warning);
            }
        }

        private void DrawSplashBackgroundColor()
        {
            EditorGUI.BeginChangeCheck();

            var newColor = EditorGUILayout.ColorField("Splash Screen Background Color", PlayerSettings.SplashScreen.backgroundColor);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.SplashScreen.backgroundColor = newColor;
            }

            if (_settings.SelectedPreferences.splash.Required
                && _settings.SelectedPreferences.splash.backgroundColor != PlayerSettings.SplashScreen.backgroundColor
                && EditorGUIExtensions.DrawFixHelpBox("Incorrect Splash Background Color."))
            {
                PlayerSettings.SplashScreen.backgroundColor = _settings.SelectedPreferences.splash.backgroundColor;
            }
        }

        private void DrawOrientationMode()
        {
            EditorGUI.BeginChangeCheck();

            var orientation = (UIOrientation)EditorGUILayout.EnumPopup("Default orientation", PlayerSettings.defaultInterfaceOrientation);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.defaultInterfaceOrientation = orientation;
            }

            if (orientation != _settings.SelectedPreferences.deviceOrientation.Orientation
                && EditorGUIExtensions.DrawFixHelpBox($"Incorrect device orientation. It should be {_settings.SelectedPreferences.deviceOrientation.Orientation}"))
            {
                PlayerSettings.defaultInterfaceOrientation = _settings.SelectedPreferences.deviceOrientation.Orientation;
            }

            if (orientation == UIOrientation.AutoRotation)
            {
                EditorGUI.BeginChangeCheck();
                var leftRot = EditorGUILayout.Toggle("Landscape Left", PlayerSettings.allowedAutorotateToLandscapeLeft);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.allowedAutorotateToLandscapeLeft = leftRot;
                }

                if (_settings.SelectedPreferences.deviceOrientation.AutoRotation)
                {
                    if (!_settings.SelectedPreferences.deviceOrientation.allowed.Contains(UIOrientation.LandscapeLeft))
                    {
                        if (PlayerSettings.allowedAutorotateToLandscapeLeft && EditorGUIExtensions.DrawFixHelpBox("Unexpected orientation", "Disable"))
                        {
                            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                        }
                    }
                    else
                    {
                        if (!PlayerSettings.allowedAutorotateToLandscapeLeft && EditorGUIExtensions.DrawFixHelpBox("Missing orientation", "Enable"))
                        {
                            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                var rightRot = EditorGUILayout.Toggle("Landscape Right", PlayerSettings.allowedAutorotateToLandscapeRight);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.allowedAutorotateToLandscapeRight = rightRot;
                }

                if (_settings.SelectedPreferences.deviceOrientation.AutoRotation)
                {
                    if (!_settings.SelectedPreferences.deviceOrientation.allowed.Contains(UIOrientation.LandscapeRight))
                    {
                        if (PlayerSettings.allowedAutorotateToLandscapeRight && EditorGUIExtensions.DrawFixHelpBox("Unexpected orientation", "Disable"))
                        {
                            PlayerSettings.allowedAutorotateToLandscapeRight = false;
                        }
                    }
                    else
                    {
                        if (!PlayerSettings.allowedAutorotateToLandscapeRight && EditorGUIExtensions.DrawFixHelpBox("Missing orientation", "Enable"))
                        {
                            PlayerSettings.allowedAutorotateToLandscapeRight = true;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                var portrait = EditorGUILayout.Toggle("Portrait", PlayerSettings.allowedAutorotateToPortrait);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.allowedAutorotateToPortrait = portrait;
                }

                if (_settings.SelectedPreferences.deviceOrientation.AutoRotation)
                {
                    if (!_settings.SelectedPreferences.deviceOrientation.allowed.Contains(UIOrientation.Portrait))
                    {
                        if (PlayerSettings.allowedAutorotateToPortrait && EditorGUIExtensions.DrawFixHelpBox("Unexpected orientation", "Disable"))
                        {
                            PlayerSettings.allowedAutorotateToPortrait = false;
                        }
                    }
                    else
                    {
                        if (!PlayerSettings.allowedAutorotateToPortrait && EditorGUIExtensions.DrawFixHelpBox("Missing orientation", "Enable"))
                        {
                            PlayerSettings.allowedAutorotateToPortrait = true;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                var portraitUpsideDown = EditorGUILayout.Toggle("Portrait Upside Down", PlayerSettings.allowedAutorotateToPortraitUpsideDown);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.allowedAutorotateToPortraitUpsideDown = portraitUpsideDown;
                }

                if (_settings.SelectedPreferences.deviceOrientation.AutoRotation)
                {
                    if (!_settings.SelectedPreferences.deviceOrientation.allowed.Contains(UIOrientation.PortraitUpsideDown))
                    {
                        if (PlayerSettings.allowedAutorotateToPortraitUpsideDown && EditorGUIExtensions.DrawFixHelpBox("Unexpected orientation", "Disable"))
                        {
                            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                        }
                    }
                    else
                    {
                        if (!PlayerSettings.allowedAutorotateToPortraitUpsideDown && EditorGUIExtensions.DrawFixHelpBox("Missing orientation", "Enable"))
                        {
                            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
                        }
                    }
                }
            }
        }

        private void DrawPlatformButton(BuildTarget platform)
        {
            EditorGUI.BeginDisabledGroup(_settings.selectedBuildTarget == platform);
            if (GUILayout.Button(platform.ToString()))
            {
                _settings.selectedBuildTarget = platform;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPlatformButtons()
        {
            EditorGUIExtensions.DrawHeader("Platform settings");

            GUILayout.BeginHorizontal();

            DrawPlatformButton(BuildTarget.Android);
            DrawPlatformButton(BuildTarget.iOS);

            GUILayout.EndHorizontal();
        }

        private void DrawSwitchButton(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
            {
                EditorGUILayout.HelpBox("Selected platform doesn't match with the active platform.", MessageType.Warning);

                if (GUILayout.Button("Switch to " + buildTarget))
                {
                    if (EditorUtility.DisplayDialog("Confirmation", "Do you really want to switch build target to " + buildTarget.ToString() + "?", "Switch", "Cancel"))
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
                    }
                }
            }
        }

        private void DrawPlatformSettings(BuildTarget settingsPlatform)
        {
            switch (settingsPlatform)
            {
                case BuildTarget.Android:
                    DrawSwitchButton(BuildTargetGroup.Android, BuildTarget.Android);
                    DrawAndroidSettings();
                    break;
                case BuildTarget.iOS:
                    DrawSwitchButton(BuildTargetGroup.iOS, BuildTarget.iOS);
                    DrawIOSSettings();
                    break;
                default:
                    EditorGUILayout.HelpBox("Unexpected platform.", MessageType.Warning);
                    break;
            }
        }

        private void DrawAppIdentifier(BuildTargetGroup buildTargetGroup)
        {
            EditorGUI.BeginChangeCheck();

            var identifier = EditorGUILayout.TextField("Identifier", PlayerSettings.GetApplicationIdentifier(buildTargetGroup));

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, identifier);
            }
        }

        private void DrawScriptingBackend(BuildTargetGroup buildTargetGroup)
        {
            EditorGUI.BeginChangeCheck();

            var value = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting Backend", PlayerSettings.GetScriptingBackend(buildTargetGroup));

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.SetScriptingBackend(buildTargetGroup, value);
            }

            ScriptingImplementation prefSI;

            switch (buildTargetGroup)
            {
                case BuildTargetGroup.Android:
                    prefSI = _settings.SelectedPreferences.android.scriptingImplementation;
                    break;
                case BuildTargetGroup.iOS:
                    prefSI = _settings.SelectedPreferences.iOS.scriptingImplementation;
                    break;
                default:
                    return;
            }

            if (value != prefSI && EditorGUIExtensions.DrawFixHelpBox($"Incorrect scripting backend on {buildTargetGroup.ToString()}"))
            {
                PlayerSettings.SetScriptingBackend(buildTargetGroup, prefSI);
            }
        }

        private void DrawAndroidSettings()
        {
            DrawAppIdentifier(BuildTargetGroup.Android);
            DrawBundleCodeAndroid();
            DrawMinSDK();
            DrawTargetSDK();
            DrawScriptingBackend(BuildTargetGroup.Android);
            DrawManifestInfo();
        }

        private void DrawMinSDK()
        {
            EditorGUI.BeginChangeCheck();

            var code = EditorGUILayout.IntSlider("Min SDK Version", (int)PlayerSettings.Android.minSdkVersion, 16, 30);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)code;
            }

            if (code != _settings.SelectedPreferences.android.minApiVersionInt && EditorGUIExtensions.DrawFixHelpBox("Mismatching min API version"))
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)_settings.SelectedPreferences.android.minApiVersionInt;
            }
        }

        private void DrawTargetSDK()
        {
            EditorGUI.BeginChangeCheck();

            var code = EditorGUILayout.IntSlider("Target SDK Version", (int)PlayerSettings.Android.targetSdkVersion, 16, 30);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)code;
            }

            if (code != _settings.SelectedPreferences.android.targetApiVersionInt && EditorGUIExtensions.DrawFixHelpBox("Mismatching target API version"))
            {
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)_settings.SelectedPreferences.android.targetApiVersionInt;
            }
        }

        private void DrawBundleCodeAndroid()
        {
            EditorGUI.BeginChangeCheck();

            var code = EditorGUILayout.IntField("Version Code", PlayerSettings.Android.bundleVersionCode);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.Android.bundleVersionCode = code;
            }
        }

        private void DrawManifestInfo()
        {
            EditorGUIExtensions.DrawHeader("Android Manifest Settings");

            if (ManifestUtility.HasManifest())
            {
                DrawIsDebuggable();
            }
            else
            {
                EditorGUILayout.HelpBox("AndroidManifest.xml doesn't exist.", MessageType.Warning);
            }
        }

        private void DrawIsDebuggable()
        {
            EditorGUI.BeginChangeCheck();

            var debuggable = EditorGUILayout.Toggle("Is debuggable", ManifestUtility.IsDebuggable());

            if (EditorGUI.EndChangeCheck())
            {
                ManifestUtility.SetDebuggableValue(debuggable);
            }

            if (_settings.SelectedPreferences.android.manifestPreferences.debuggable != debuggable
                && EditorGUIExtensions.DrawFixHelpBox("Is debuggable is incorrect"))
            {
                ManifestUtility.SetDebuggableValue(_settings.SelectedPreferences.android.manifestPreferences.debuggable);
            }
        }

        private void DrawIOSSettings()
        {
            DrawAppIdentifier(BuildTargetGroup.iOS);
            DrawBundleCodeIOS();
            DrawScriptingBackend(BuildTargetGroup.iOS);
            DrawAutomaticallySign();
            DrawSigningTeamId();
        }

        private void DrawBundleCodeIOS()
        {
            EditorGUI.BeginChangeCheck();

            var number = EditorGUILayout.TextField("Build Number", PlayerSettings.iOS.buildNumber);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.iOS.buildNumber = number;
            }
        }

        private void DrawAutomaticallySign()
        {
            EditorGUI.BeginChangeCheck();

            var value = EditorGUILayout.Toggle("Automatically Sign", PlayerSettings.iOS.appleEnableAutomaticSigning);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.iOS.appleEnableAutomaticSigning = value;
            }

            if (_settings.SelectedPreferences.iOS.automaticallySign != value
                && EditorGUIExtensions.DrawFixHelpBox("Automatically sign value is incorrect"))
            {
                PlayerSettings.iOS.appleEnableAutomaticSigning = _settings.SelectedPreferences.iOS.automaticallySign;
            }
        }

        private void DrawSigningTeamId()
        {
            EditorGUI.BeginChangeCheck();

            var id = EditorGUILayout.TextField("Developer Team Id", PlayerSettings.iOS.appleDeveloperTeamID);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.iOS.appleDeveloperTeamID = id;
            }

            if (_settings.SelectedPreferences.iOS.signingTeamId != id
                && EditorGUIExtensions.DrawFixHelpBox("Incorrect team ID"))
            {
                PlayerSettings.iOS.appleDeveloperTeamID = _settings.SelectedPreferences.iOS.signingTeamId;
            }
        }

        private void DrawSDKsSettings()
        {
            foreach (var sdk in _settings.SelectedPreferences.SDKs)
            {
                if (sdk != null)
                {
                    EditorGUIExtensions.DrawHeader(sdk.ItemName);

                    sdk.DrawComparsion();

                    if (sdk.HasInternalSettings && GUILayout.Button("Open SDK settings"))
                    {
                        sdk.OpenInternalSettings();
                    }
                }
            }
        }
    }
}