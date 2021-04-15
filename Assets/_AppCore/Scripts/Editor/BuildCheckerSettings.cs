using System;
using UnityEditor;
using UnityEngine;

namespace AppCore
{
    public enum BuildPreferencesType
    {
        Internal,
        Test,
        Release,
    }

    [CreateAssetMenu(fileName = "BuildCheckerSettings", menuName = "AppCore/Build/Build Checker Settings", order = 0)]
    public class BuildCheckerSettings : ScriptableObject
    {
        public BuildTarget selectedBuildTarget = BuildTarget.Android;
        public BuildPreferencesType selectedPrefType = BuildPreferencesType.Release;

        public static readonly string Path = "Assets/_AppCore/Scripts/Editor/BuildCheckerSettings.asset";
        public static readonly string FacebookSettingsPath = "Assets/_AppCore/Scripts/SDKModules/Facebook/Editor/BuildCheckerSettings.asset";

        public BuildPreferences internalPreferences;
        public BuildPreferences testPreferences;
        public BuildPreferences releasePreferences;

        public BuildPreferences SelectedPreferences => GetPreferences(selectedPrefType);

        //public FacebookPreferences facebook;

        private void Init()
        {
            //facebook = AssetDatabase.LoadAssetAtPath<FacebookPreferences>(FacebookSettingsPath);
        }

        public void SetPreferences(BuildPreferencesType type, BuildPreferences value)
        {
            switch (type)
            {
                case BuildPreferencesType.Internal:
                    internalPreferences = value;
                    break;

                case BuildPreferencesType.Test:
                    testPreferences = value;
                    break;

                case BuildPreferencesType.Release:
                    releasePreferences = value;
                    break;
            }

            return;
        }

        public BuildPreferences GetPreferences(BuildPreferencesType type)
        {
            BuildPreferences preferences = null;

            switch (type)
            {
                case BuildPreferencesType.Internal:
                    preferences = internalPreferences;
                    break;

                case BuildPreferencesType.Test:
                    preferences = testPreferences;
                    break;

                case BuildPreferencesType.Release:
                    preferences = releasePreferences;
                    break;
            }

            return preferences;
        }

        private static BuildCheckerSettings _instance;
        public static BuildCheckerSettings Instance
        {
            get
            {
                bool created = false;

                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<BuildCheckerSettings>(Path);

                    if (_instance == null)
                    {
                        _instance = CreateInstance<BuildCheckerSettings>();

                        AssetDatabase.CreateAsset(_instance, Path);

                        created = true;

                        Debug.Log("Created Build Checker Settings");

                        if (_instance == null)
                        {
                            throw new NullReferenceException("BuildPreferences not found!");
                        }
                    }

                    _instance.Init();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}