using System.Collections;
using UnityEngine;
using System;

namespace AppCore
{
    public enum LoadSceneResult
    {
        Success,
        Failed,
        Interrupted,
    }

    public interface ISceneLoadPercentChangedListener
    {
        void OnSceneLoadPercentChangedHandler(float percent);
    }

    public interface ISceneLoadStartedListener
    {
        void OnSceneLoadStartedHandler(SceneName sceneName);
    }

    public interface ISceneLoadEndedListener
    {
        void OnSceneLoadEndedHandler(LoadSceneResult loadSceneResult, SceneName sceneName);
    }

    public interface ISceneManager
    {
        SceneName CurrentScene { get; }

        event Action<SceneName> OnSceneLoadStartedEvent;
        event Action<float> OnSceneLoadPercentChangedEvent;
        event Action<LoadSceneResult, SceneName> OnSceneLoadEndedEvent;

        void ChangeScene(SceneName sceneName, bool async = true);
    }

    public class SceneManager : MonoBehaviour, ISceneManager
    {
        private Coroutine _loadRoutine;

        public SceneName CurrentScene { get { return (SceneName)Enum.Parse(typeof(SceneName), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); } }
        static public ISceneManager Instance { get; private set; }

        public event Action<SceneName> OnSceneLoadStartedEvent;
        public event Action<float> OnSceneLoadPercentChangedEvent;
        public event Action<LoadSceneResult, SceneName> OnSceneLoadEndedEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ChangeScene(SceneName sceneName, bool async = true)
        {
            LoadSceneResult result;
            
            if (_loadRoutine != null)
            {
                result = LoadSceneResult.Interrupted;

                StopCoroutine(_loadRoutine);
                OnSceneLoadEndedEvent?.Invoke(result, sceneName);
            }

            OnSceneLoadStartedEvent?.Invoke(sceneName);

            if (async)
            {
                _loadRoutine = StartCoroutine(LoadAsyncScene(sceneName));
            }
            else
            {
                try
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName.ToString());
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                    result = LoadSceneResult.Failed;
                    OnSceneLoadEndedEvent?.Invoke(result, sceneName);
                    return;
                }

                result = LoadSceneResult.Success;
                OnSceneLoadEndedEvent?.Invoke(result, sceneName);
            }
        }

        IEnumerator LoadAsyncScene(SceneName sceneName)
        {
            LoadSceneResult result;
            AsyncOperation asyncLoad;
            try
            {
                asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                result = LoadSceneResult.Failed;
                OnSceneLoadEndedEvent?.Invoke(result, sceneName);
                yield break;
            }

            // Wait until the asynchronous scene fully loads.
            while (!asyncLoad.isDone)
            {
                OnSceneLoadPercentChangedEvent?.Invoke(asyncLoad.progress);
                yield return null;
            }

            result = LoadSceneResult.Success;
            OnSceneLoadEndedEvent?.Invoke(result, sceneName);
        }
    }
}
