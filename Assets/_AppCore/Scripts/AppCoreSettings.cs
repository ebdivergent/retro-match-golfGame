using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using AppCore.EditorUtilities;
#endif

namespace AppCore
{
    [CreateAssetMenu(fileName = "AppCoreSettings", menuName = "AppCore/Settings", order = 0)]
    public class AppCoreSettings : ScriptableObject
    {
        [SerializeField] LoadingCanvas _loadingCanvas;

        public LoadingCanvas LoadingCanvas { get { return _loadingCanvas; } set { _loadingCanvas = value; } }

#if UNITY_EDITOR
        [SerializeField] EnumEditorGroup _scenesEnum;
        [SerializeField] EnumEditorGroup _pagesEnum;
        [SerializeField] EnumEditorGroup _prefsKeyEnum;
        [SerializeField] List<string> _dataContainerUsings = new List<string>() { 
            "AppCore",
        };

        public EnumEditorGroup ScenesEnum { get { return _scenesEnum; } }
        public EnumEditorGroup PagesEnum { get { return _pagesEnum; } }
        public EnumEditorGroup PrefsKeyEnum { get { return _prefsKeyEnum; } }
        public List<string> DataContainerUsings { get { return _dataContainerUsings; } set { _dataContainerUsings = value; } }
#endif

        private static AppCoreSettings _instance;
        public static AppCoreSettings Instance 
        { 
            get
            {
                bool hasToBeCreated = _instance == null;

                if (hasToBeCreated)
                {
                    Debug.Log("Load AppCoreSettings");
                    _instance = Resources.Load<AppCoreSettings>("AppCoreSettings");

#if UNITY_EDITOR
                    if (_instance == null && !EditorApplication.isPlaying)
                    {
                        Debug.Log("Creating AppCoreSettings");
                        _instance = AppCoreSettings.CreateInstance<AppCoreSettings>();
                    }
#endif
                }

                if (_instance == null)
                    throw new NullReferenceException("AppCoreSettings not found!");

                if (hasToBeCreated)
                    _instance.Init();

                return _instance;
            } 
            set
            {
                _instance = value;
            }
        }

        public void Init()
        {
#if UNITY_EDITOR
            if (_scenesEnum == null)
                _scenesEnum = new EnumEditorGroup("SceneName", true);

            if (_pagesEnum == null)
                _pagesEnum = new EnumEditorGroup("PageType", true);

            if (_prefsKeyEnum == null)
                _prefsKeyEnum = new EnumEditorGroup("PrefsKey", true);

            _dataContainerUsings = _dataContainerUsings ?? new List<string>();

            ScenesEnum.OnDrawEndLineGUI = (name) =>
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                {
                    string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);

                    int last_index_after_slash = 0;
                    last_index_after_slash = path.LastIndexOfAny(new char[] { '\\', '/' });

                    if (last_index_after_slash == 0)
                        throw new Exception("Internal error");

                    int last_index_of_dot = path.LastIndexOf('.');
                    int count = last_index_of_dot - last_index_after_slash;

                    if (path.Substring(last_index_after_slash + 1, count - 1) == name)
                        return;
                }

                GUIStyle guistyle = new GUIStyle(EditorStyles.label);
                guistyle.normal.textColor = Color.red;

                GUILayout.Label("<- MISSING IN BUILD SETTINGS", guistyle);
            };

            PrefsKeyEnum.OnDrawEndLineGUI += (key) =>
            {
                PrefsKey prefsKey;
                if (System.Enum.TryParse<PrefsKey>(key, out prefsKey))
                {
                    string value;
                    bool json;
                    if (DataManager.GetKeyValue(prefsKey, out value, out json))
                    {
                        if (json)
                        {
                            if (GUILayout.Button("Copy JSON to clipboard"))
                            {
                                GUIUtility.systemCopyBuffer = value;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField($"Value: '{value}'");
                        }
                    }
                }
            };

            //ScenesEnum.CheckForRewrite();
            //PagesEnum.CheckForRewrite();
            //PrefsKeyEnum.CheckForRewrite();
#endif
        }
    }
}