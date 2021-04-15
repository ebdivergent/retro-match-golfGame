using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace AppCore
{
    [CustomEditor(typeof(BuildPreferences))]
    public class BuildPreferencesDrawer : Editor
    {
        private BuildPreferences _target;

        public override void OnInspectorGUI()
        {
            _target = (BuildPreferences)target;

            DrawUnityPro();
            DrawCompany();
            DrawIcon();
            DrawOrientations();
            DrawSplash();
            DrawAndroidPreferences();
            DrawIOSPreferences();
            DrawSDKs();

            EditorUtility.SetDirty(_target);
        }

        private bool DrawRequirable(Requirable requirable)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUIExtensions.DrawHeader(requirable.ItemName);

            var value = EditorGUILayout.Toggle("Required", requirable.Required);

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                requirable.Required = value;
            }

            return value;
        }

        private void DrawUnityPro()
        {
            if (DrawRequirable(_target.unityPro))
            {
                //
            }
        }

        private void DrawCompany()
        {
            if (DrawRequirable(_target.company))
            {
                EditorGUI.BeginChangeCheck();

                var value = EditorGUILayout.TextField("Company name", _target.company.name);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Edited Build Preferences");

                    _target.company.name = value;
                }

                if (string.IsNullOrEmpty(value) || value.Length <= 2)
                {
                    EditorGUILayout.HelpBox("Company name is too short, isn't it?", MessageType.Warning);
                }
            }
        }

        private void DrawIcon()
        {
            if (DrawRequirable(_target.icon))
            {
                //
            }
        }

        private void DrawOrientations()
        {
            if (DrawRequirable(_target.deviceOrientation))
            {
                DrawOrientation(UIOrientation.LandscapeLeft);
                DrawOrientation(UIOrientation.LandscapeRight);
                DrawOrientation(UIOrientation.Portrait);
                DrawOrientation(UIOrientation.PortraitUpsideDown);
            }
        }

        private void DrawOrientation(UIOrientation uIOrientation)
        {
            EditorGUI.BeginChangeCheck();

            bool enabled = _target.deviceOrientation.allowed.Contains(uIOrientation);

            var value = EditorGUILayout.Toggle(uIOrientation.ToString(), enabled);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                if (enabled)
                {
                    if (_target.deviceOrientation.allowed.Count > 1)
                    {
                        _target.deviceOrientation.allowed.Remove(uIOrientation);
                    }
                    else
                    {
                        Debug.LogError("You cannot remove all orientations");
                    }
                }
                else
                {
                    _target.deviceOrientation.allowed.Add(uIOrientation);
                }
            }
        }

        private void DrawSplash()
        {
            if (DrawRequirable(_target.splash))
            {
                DrawSplashLogos();
                DrawSplashBackgroundColor();
            }
        }

        private void DrawSplashLogos()
        {
            var logos = _target.splash.logos;

            for (int i = 0; i < logos.Count; i++)
            {
                var logo = logos[i];

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                var newLogo = EditorGUIExtensions.SpriteField($"Logo {i}", logo);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Edited Build Preferences");

                    logos[i] = newLogo;
                }

                if (!newLogo)
                {
                    EditorGUILayout.HelpBox("Logo is null", MessageType.Warning);
                }

                if (GUILayout.Button("Remove"))
                {
                    Undo.RecordObject(_target, "Edited Build Preferences");

                    logos.RemoveAt(i--);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add logo"))
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                logos.Add(null);
            }

            if ((logos == null || logos.Count <= 0))
            {
                EditorGUILayout.HelpBox("No logos defined.", MessageType.Warning);
            }
        }

        private void DrawSplashBackgroundColor()
        {
            EditorGUI.BeginChangeCheck();

            var newColor = EditorGUILayout.ColorField("Background Color", _target.splash.backgroundColor);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.splash.backgroundColor = newColor;
            }
        }

        private void DrawAndroidPreferences()
        {
            EditorGUIExtensions.DrawHeader("Android");

            DrawMinAPIVersion();
            DrawTargetAPIVersion();
            DrawArchitecturesAndroid();
            DrawScriptingImplementationAndroid();
            DrawSplitAPKs();
            DrawManifestIsDebuggable();
        }

        private void DrawMinAPIVersion()
        {
            EditorGUI.BeginChangeCheck();

            var minApiVersion = EditorGUILayout.IntSlider("Min API level", _target.android.minApiVersionInt, 16, 30);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.minApiVersionInt = minApiVersion;
            }
        }

        private void DrawTargetAPIVersion()
        {
            EditorGUI.BeginChangeCheck();

            var targetApiVersion = EditorGUILayout.IntSlider("Target API level", _target.android.targetApiVersionInt, 16, 30);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.targetApiVersionInt = targetApiVersion;
            }
        }

        private void DrawArchitecturesAndroid()
        {
            EditorGUI.BeginChangeCheck();

            var architecture = (AndroidArchitecture)EditorGUILayout.EnumFlagsField("Architecture", _target.android.androidArchitectures);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.androidArchitectures = architecture;
            }
        }

        private void DrawScriptingImplementationAndroid()
        {
            EditorGUI.BeginChangeCheck();

            var scriptingImpl = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting implementation", _target.android.scriptingImplementation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.scriptingImplementation = scriptingImpl;
            }
        }

        private void DrawSplitAPKs()
        {
            EditorGUI.BeginChangeCheck();

            var splitApks = EditorGUILayout.Toggle("Split apks", _target.android.splitAPKs);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.splitAPKs = splitApks;
            }
        }

        private void DrawManifestIsDebuggable()
        {
            EditorGUI.BeginChangeCheck();

            var debuggable = EditorGUILayout.Toggle("Is Debuggable", _target.android.manifestPreferences.debuggable);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.android.manifestPreferences.debuggable = debuggable;
            }
        }

        private void DrawIOSPreferences()
        {
            EditorGUIExtensions.DrawHeader("iOS");

            DrawScriptingImplementationIOS();
            DrawAppleTeamId();
            DrawAutomaticallySign();
        }

        private void DrawScriptingImplementationIOS()
        {
            EditorGUI.BeginChangeCheck();

            var scriptingImpl = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting implementation", _target.iOS.scriptingImplementation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.iOS.scriptingImplementation = scriptingImpl;
            }
        }

        private void DrawAppleTeamId()
        {
            EditorGUI.BeginChangeCheck();

            var teamId = EditorGUILayout.TextField("Signing Team ID", _target.iOS.signingTeamId);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.iOS.signingTeamId = teamId;
            }
        }

        private void DrawAutomaticallySign()
        {
            EditorGUI.BeginChangeCheck();

            var autoSign = EditorGUILayout.Toggle("Auto sign", _target.iOS.automaticallySign);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.iOS.automaticallySign = autoSign;
            }
        }

        private void DrawSDKs()
        {
            for (int i = 0; i < _target.SDKs.Count; i++)
            {
                var sdk = _target.SDKs[i];

                EditorGUI.BeginChangeCheck();

                bool removed;

                EditorGUILayout.BeginHorizontal();

                var newSDK = (SDKBuildPreferences)EditorGUILayout.ObjectField(sdk, typeof(SDKBuildPreferences), false);

                removed = GUILayout.Button("Remove");

                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Edited Build Preferences");

                    _target.SDKs[i] = newSDK;
                }

                if (removed)
                {
                    Undo.RecordObject(_target, "Edited Build Preferences");

                    _target.SDKs.RemoveAt(i);
                    i--;
                    continue;
                }

                if (newSDK)
                {
                    EditorGUIExtensions.DrawHeader(newSDK.ItemName);
                    newSDK.DrawPreferences();
                }
            }

            if (GUILayout.Button("Add SDK"))
            {
                Undo.RecordObject(_target, "Edited Build Preferences");

                _target.SDKs.Add(null);
            }
        }
    }
}