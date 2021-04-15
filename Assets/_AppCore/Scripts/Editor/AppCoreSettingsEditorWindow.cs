using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental;

namespace AppCore.EditorUtilities
{
    public class AppCoreSettingsEditorWindow : EditorWindow
    {
        private static readonly string CONSTS_FILE_NAME = "Consts";

        private static readonly string CONSTS_FILE_PATH = "Assets/_AppCore/Scripts/Consts.cs";

        private static readonly string SETTINGS_FILE_PATH = "Assets/_AppCore/Resources/AppCoreSettings.asset";

        private static readonly string FacebookPreferencesPath = "Assets/_AppCore/Scripts/SDKModules/AppCoreFacebookPreferencesModule.unitypackage";
        private static readonly string GAPreferencesPath = "Assets/_AppCore/Scripts/SDKModules/AppCoreGameAnalyticsPreferencesModule.unitypackage";

        private static readonly string FacebookSDKPath = "Assets/_AppCore/Scripts/SDKModules/ClearSDKs/facebook-unity-sdk-8.0.0.unitypackage";
        private static readonly string GASDKPath = "Assets/_AppCore/Scripts/SDKModules/ClearSDKs/GA_SDK_UNITY.unitypackage";

        Vector2 _scrollPos;
        Texture2D _logoTexture;

        private void Awake()
        {
            _logoTexture = _logoTexture ?? EditorResources.Load<Texture2D>("Assets/_AppCore/Sprites/Editor/settings.png");
        }

        private void OnLostFocus()
        {
            if (!Application.isPlaying)
                SaveAsset();
        }

        private void SaveAsset()
        {
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(AppCoreSettings.Instance)))
            {
                AssetDatabase.CreateAsset(AppCoreSettings.Instance, SETTINGS_FILE_PATH);
            }
            else
            {
                AssetDatabase.ForceReserializeAssets(new string[] { SETTINGS_FILE_PATH }, ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
            }
        }

        private void OnProjectChange()
        {
            AppCoreSettings.Instance.PagesEnum.CheckForRewrite();
            AppCoreSettings.Instance.ScenesEnum.CheckForRewrite();
        }

        [MenuItem("AppCore/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<AppCoreSettingsEditorWindow>("AppCore Settings", true,
                typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
        }

        private static string SceneNameByIndex(int index)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(index);
            string sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
            return sceneName;
        }
        
        private void OnGUI()
        {
            if (AppCoreSettings.Instance == null)
                return;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            AppCoreSettings.Instance.ScenesEnum.OnGUI();

            if (AppCoreSettings.Instance.ScenesEnum.Draw)
            {
                //EditorGUILayout.HelpBox("Button below only adds scenes to the list above from Build Settings", MessageType.Info);

                if (GUILayout.Button("Add scenes from Build Settings"))
                {
                    var elements = AppCoreSettings.Instance.ScenesEnum.EnumInfo.Elements;

                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                    {
                        var name = SceneNameByIndex(i);

                        if (!elements.Contains(name))
                            elements.Add(name);
                    }
                }
            }

            AppCoreSettings.Instance.PagesEnum.OnGUI();
            AppCoreSettings.Instance.PrefsKeyEnum.OnGUI();

            if (AppCoreSettings.Instance.PrefsKeyEnum.Draw)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Usings");
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < AppCoreSettings.Instance.DataContainerUsings.Count; i++)
                {
                    AppCoreSettings.Instance.DataContainerUsings[i] = GUILayout.TextField(AppCoreSettings.Instance.DataContainerUsings[i]);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    AppCoreSettings.Instance.DataContainerUsings.Add("");
                }
                if (AppCoreSettings.Instance.DataContainerUsings.Count > 0 && GUILayout.Button("-"))
                {
                    AppCoreSettings.Instance.DataContainerUsings.RemoveAt(AppCoreSettings.Instance.DataContainerUsings.Count - 1);
                }
                if (GUILayout.Button("Clear"))
                {
                    AppCoreSettings.Instance.DataContainerUsings.Clear();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Generate DataContainer.cs"))
                {
                    List<string> body = new List<string>();

                    body.Add("IDataManager _dataManager;\n\npublic DataContainer(IDataManager dataManager)\n{\n _dataManager = dataManager;\n}\n");

                    var elements = AppCoreSettings.Instance.PrefsKeyEnum.EnumInfo.Elements;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        string keyEnumName = elements[i];
                        string propertyName = "";
                        string prefsManagerName = "";
                        string typeString = "";

                        if (!DataManager.ParseKeyString(keyEnumName, out propertyName, out typeString, out prefsManagerName))
                            continue;

                        switch (typeString)
                        {
                            case "Int":
                            case "int":
                                body.Add($"public int {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return {prefsManagerName}.GetInt(PrefsKey.{keyEnumName}, 0); }}");
                                body.Add($"    set {{ {prefsManagerName}.SetInt(PrefsKey.{keyEnumName}, value); }}");
                                body.Add("}");
                                break;

                            case "bool":
                            case "Bool":
                                body.Add($"public bool {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return {prefsManagerName}.GetInt(PrefsKey.{keyEnumName}, 0) == 1; }}");
                                body.Add($"    set {{ {prefsManagerName}.SetInt(PrefsKey.{keyEnumName}, value ? 1 : 0); }}");
                                body.Add("}");
                                break;

                            case "Float":
                            case "float":
                                body.Add($"public float {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return {prefsManagerName}.GetFloat(PrefsKey.{keyEnumName}, 0.0f); }}");
                                body.Add($"    set {{ {prefsManagerName}.SetFloat(PrefsKey.{keyEnumName}, value); }}");
                                body.Add("}");
                                break;

                            case "String":
                            case "string":
                                body.Add($"public string {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return {prefsManagerName}.GetString(PrefsKey.{keyEnumName}, \"\"); }}");
                                body.Add($"    set {{ {prefsManagerName}.SetString(PrefsKey.{keyEnumName}, value); }}");
                                body.Add("}");
                                break;

                            case "enum":
                            case "Enum":
                                body.Add($"public {typeString} {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return ({typeString}){prefsManagerName}.GetInt(PrefsKey.{keyEnumName}, 0); }}");
                                body.Add($"    set {{ {prefsManagerName}.SetInt(PrefsKey.{keyEnumName}, (int)value); }}");
                                body.Add("}");
                                break;

                            default:
                                var type = TypeUtility.GetTypeByName(typeString);

                                if (type == null)
                                    continue;

                                if (type.IsEnum)
                                    goto case "enum";

                                body.Add($"public {typeString} {propertyName}");
                                body.Add("{");
                                body.Add($"    get {{ return {prefsManagerName}.Get<{typeString}>(PrefsKey.{keyEnumName}, default); }}");
                                body.Add($"    set {{ {prefsManagerName}.Set<{typeString}>(PrefsKey.{keyEnumName}, value); }}");
                                body.Add("}");
                                break;
                        }
                    }

                    ClassfileGenerator.CreateClass(new ClassInfo()
                    {
                        name = "DataContainer",
                        //nspace = "AppCore",
                        body = body.ToArray(),
                        usings = AppCoreSettings.Instance.DataContainerUsings.ToArray(),
                    });
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Loading Canvas Prefab");
            AppCoreSettings.Instance.LoadingCanvas = 
                (LoadingCanvas)EditorGUILayout.ObjectField(AppCoreSettings.Instance.LoadingCanvas, 
                    typeof(LoadingCanvas), 
                    false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Facebook SDK"))
            {
                AssetDatabase.ImportPackage(FacebookSDKPath, false);
                AssetDatabase.ImportPackage(FacebookPreferencesPath, false);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add GameAnalytics SDK"))
            {
                AssetDatabase.ImportPackage(GASDKPath, false);
                AssetDatabase.ImportPackage(GAPreferencesPath, false);
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();

            EditorGUI.EndDisabledGroup();
        }
    }
}