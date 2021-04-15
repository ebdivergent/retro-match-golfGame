using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace AppCore
{
    public sealed class CoreServices : ILoadableService
    {
        private static readonly string _EVENT_SYSTEM_PATH = "Prefabs/EventSystem";
        private EventSystem _eventSystem;
        private LoadingCanvas _loadingCanvas;

#if !UNITY_EDITOR && DEBUG
        private static readonly string _LOGGER_CANVAS_PATH = "Prefabs/Canvas_Logger";
        private UILoggerController _uiLoggerController;
#endif

        public void StartLoad(MonoBehaviour caller)
        {
            new GameObject("LogManager").AddComponent<LogManager>();
            new GameObject("MonoHelpers").AddComponent<MonoHelpers>();
            new GameObject("TimeManager").AddComponent<TimeManager>();
            new GameObject("DataManager").AddComponent<DataManager>();
            new GameObject("SceneManager").AddComponent<SceneManager>();
            new GameObject("AudioController").AddComponent<AudioController>();
            new GameObject("AudioGroupManager").AddComponent<AudioGroupManager>();
            new GameObject("AudioSettingsManager").AddComponent<AudioSettingsManager>();
            new GameObject("AudioStreamManager").AddComponent<AudioStreamManager>();
            new GameObject("InputController").AddComponent<InputController>();
            new GameObject("NetworkManager").AddComponent<NetworkManager>();
            new GameObject("AnalyticsManager").AddComponent<AnalyticsManager>();

            if (EasyMobile.EM_Settings.IsIAPModuleEnable)
            {
                new GameObject("IAPController").AddComponent<IAPController>();
            }

            caller.StartCoroutine(LoadRoutine(caller));
        }

        public bool IsLoaded()
        {
            return SceneManager.Instance != null
                && _loadingCanvas != null
#if !UNITY_EDITOR && DEBUG
                && _uiLoggerController != null
#endif
                && _eventSystem != null
                ;
        }

        private IEnumerator LoadRoutine(MonoBehaviour caller)
        {
            var eventSystemAsyncOperation = Resources.LoadAsync<GameObject>(_EVENT_SYSTEM_PATH);

            var settings = Resources.LoadAsync<AppCoreSettings>("AppCoreSettings");

            yield return new WaitUntil(() => settings.isDone);
            AppCoreSettings.Instance = (AppCoreSettings)settings.asset;

#if !UNITY_EDITOR && DEBUG
            var uiLoggerAsyncOperation = Resources.LoadAsync<GameObject>(_LOGGER_CANVAS_PATH);
#endif
            _loadingCanvas = ((GameObject)Object.Instantiate(AppCoreSettings.Instance.LoadingCanvas.gameObject)).GetComponent<LoadingCanvas>();

            yield return new WaitUntil(() => eventSystemAsyncOperation.isDone);
            _eventSystem = ((GameObject)Object.Instantiate(eventSystemAsyncOperation.asset)).GetComponent<EventSystem>();

            Object.DontDestroyOnLoad(_eventSystem.gameObject);

#if !UNITY_EDITOR && DEBUG
            yield return new WaitUntil(() => uiLoggerAsyncOperation.isDone);
            _uiLoggerController = ((GameObject)Object.Instantiate(uiLoggerAsyncOperation.asset)).GetComponent<UILoggerController>();
#endif
        }
    }
}