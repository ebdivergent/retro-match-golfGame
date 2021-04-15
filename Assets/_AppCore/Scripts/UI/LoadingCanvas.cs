using UnityEngine;
using UnityEngine.UI;

namespace AppCore
{
    public class LoadingCanvas : UIElement, ILoadingCanvas
    {
        #region Properties
        public static ILoadingCanvas Instance { get; private set; }
        #endregion

        #region Overrides
        protected override void OnAwakeHandler()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.Instance.OnSceneLoadStartedEvent += OnSceneLoadStartedHandler;
            SceneManager.Instance.OnSceneLoadPercentChangedEvent += OnSceneLoadPercentChangedHandler;
            SceneManager.Instance.OnSceneLoadEndedEvent += OnSceneLoadEndedHandler;
        }

        protected override void OnDestroyHandler()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.OnSceneLoadStartedEvent -= OnSceneLoadStartedHandler;
                SceneManager.Instance.OnSceneLoadPercentChangedEvent -= OnSceneLoadPercentChangedHandler;
                SceneManager.Instance.OnSceneLoadEndedEvent -= OnSceneLoadEndedHandler;
            }
        }

        protected virtual void OnSceneLoadStarted(SceneName sceneName)
        {

        }

        protected virtual void OnSceneLoadPercentChanged(float percent)
        {

        }

        protected virtual void OnSceneLoadEnded(LoadSceneResult loadSceneResult, SceneName sceneName)
        {

        }
        #endregion

        #region SceneManager handlers
        private void OnSceneLoadStartedHandler(SceneName sceneName)
        {
            OnSceneLoadStarted(sceneName);

            Show(true);
        }

        private void OnSceneLoadPercentChangedHandler(float percent)
        {
            OnSceneLoadPercentChanged(percent);
        }

        private void OnSceneLoadEndedHandler(LoadSceneResult loadSceneResult, SceneName sceneName)
        {
            OnSceneLoadEnded(loadSceneResult, sceneName);

            Show(false);
        }
        #endregion
    }
}