using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }

        private List<AnalyticsProvider> _providers = new List<AnalyticsProvider>();

        private void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            var providers = Resources.LoadAll<AnalyticsProvider>("");

            foreach (var provider in providers)
            {
                var instance = Instantiate(provider);
                DontDestroyOnLoad(instance.gameObject);
                _providers.Add(instance);
            }

            SendAppLaunchEvent();
        }

        private void SendAppLaunchEvent()
        {
            if (PlayerPrefs.GetInt("FirstLaunch", 0) == 0)
            {
                foreach (var provider in _providers)
                {
                    provider.SendFirstLaunch();
                }

                PlayerPrefs.SetInt("FirstLaunch", 1);
                Debug.Log("FirstLaunchEvent");
            }

            foreach (var provider in _providers)
            {
                provider.SendAppLaunch();
            }

            Debug.Log("SendAppLaunchEvent");
        }

        public void SendLevelCompleteEvent(int level)
        {
            foreach (var provider in _providers)
            {
                provider.SendGameStateEvent(AnalyticsGameStateEvent.Complete, level);
            }

            Debug.Log("SendLevelCompleteEvent");
        }

        public void SendLevelStartEvent(int level)
        {
            foreach (var provider in _providers)
            {
                provider.SendGameStateEvent(AnalyticsGameStateEvent.Start, level);
            }

            Debug.Log("SendLevelStartEvent");
        }

        public void SendLevelFailEvent(int level)
        {
            foreach (var provider in _providers)
            {
                provider.SendGameStateEvent(AnalyticsGameStateEvent.Fail, level);
            }

            Debug.Log("SendLevelFailEvent");
        }
    }
}