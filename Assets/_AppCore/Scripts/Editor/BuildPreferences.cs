using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace AppCore
{
    [Serializable]
    public class ManifestPreferences
    {
        public bool debuggable = false;
    }

    [Serializable]
    public class PlatformPreferences
    {
        public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
    }

    [Serializable]
    public class AndroidBuildPreferences : PlatformPreferences
    {
        [Range(16, 30)] public int minApiVersionInt = 16;
        [Range(16, 30)] public int targetApiVersionInt = 30;
        public bool splitAPKs = true;
        [ApCrEnumFlags] public AndroidArchitecture androidArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        public ManifestPreferences manifestPreferences;

        public AndroidSdkVersions MinApiVersion => (AndroidSdkVersions)minApiVersionInt;
        public AndroidSdkVersions TargetApiVersion => (AndroidSdkVersions)targetApiVersionInt;
    }

    [Serializable]
    public class iOSBuildPreferences : PlatformPreferences
    {
        public bool automaticallySign = true;
        public string signingTeamId = "";
    }

    [Serializable]
    public class Requirable : IRequirable
    {
        [SerializeField] string _itemName;
        [SerializeField] bool _required;

        public string ItemName
        {
            get => _itemName;
            set => _itemName = value;
        }

        public bool Required
        {
            get => _required;
            set => _required = value;
        }
    }

    public interface IRequirable
    {
        string ItemName { get; set; }
        bool Required { get; set; }
    }

    [Serializable]
    public class UnityProPreferences : Requirable { }

    [Serializable]
    public class IconPreferences : Requirable { }

    [Serializable]
    public class CompanyPreferences : Requirable
    {
        public string name;
    }

    [Serializable]
    public class OrientationPreferences : Requirable
    {
        public List<UIOrientation> allowed;

        public UIOrientation Orientation
        {
            get
            {
                if (AutoRotation)
                {
                    return UIOrientation.AutoRotation;
                }
                else if (allowed.Count > 0)
                {
                    return allowed[0];
                }
                else
                {
                    Debug.LogError("OrientationPreferences 'allowed' list is empty");
                    return UIOrientation.Portrait;
                }
            }
        }

        public bool AutoRotation { get { return allowed.Count > 1; } }
    }

    [Serializable]
    public class SplashPreferences : Requirable
    {
        public List<Sprite> logos;
        public Color backgroundColor;
    }

    [CreateAssetMenu(fileName = "AppBuildPreferences", menuName = "AppCore/Build/Preferences/App", order = 0)]
    public class BuildPreferences : ScriptableObject
    {
        public UnityProPreferences unityPro =
            new UnityProPreferences() { Required = true, ItemName = "Unity Pro", };
        public CompanyPreferences company =
            new CompanyPreferences() { Required = true, name = "Mobi Screen Apps", ItemName = "Company", };
        public IconPreferences icon =
            new IconPreferences() { Required = true, ItemName = "Icon", };
        public OrientationPreferences deviceOrientation =
            new OrientationPreferences() { Required = true, ItemName = "Orientation", allowed = new List<UIOrientation>() { UIOrientation.Portrait, }, };
        public SplashPreferences splash =
            new SplashPreferences() { Required = true, ItemName = "Splash", backgroundColor = Color.white, logos = new List<Sprite>(), };

        public AndroidBuildPreferences android
            = new AndroidBuildPreferences();
        public iOSBuildPreferences iOS
            = new iOSBuildPreferences();

        public List<SDKBuildPreferences> SDKs;
    }

    [Serializable]
    public abstract class SDKBuildPreferences : ScriptableObject, IRequirable
    {
        [SerializeField] bool _required;

        public bool Required { get => _required; set => _required = value; }
        public abstract string ItemName { get; set; }
        public abstract bool HasInternalSettings { get; }

        public abstract void DrawPreferences();
        public abstract void DrawComparsion();
        public abstract void OpenInternalSettings();
    }

    [CustomEditor(typeof(SDKBuildPreferences), true)]
    public class SDKBuildPreferencesDrawer : Editor
    {
        SDKBuildPreferences _target;

        public override void OnInspectorGUI()
        {
            _target = (SDKBuildPreferences)target;

            EditorGUIExtensions.DrawHeader(_target.ItemName);

            _target.DrawPreferences();
        }
    }
}