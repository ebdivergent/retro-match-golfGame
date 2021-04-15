using System;
using UnityEngine;

namespace AppCore
{
    public class TimeManager : MonoBehaviour, ITimeManager
    {
        private bool _isPaused;

        public static ITimeManager Instance { get; private set; }

        public float LastTimeScale { get; private set; }
        public float Timescale
        {
            get { return Time.timeScale; }
            set
            {
                if (_isPaused)
                    LastTimeScale = value;
                else
                    LastTimeScale = Time.timeScale = value;

                OnTimescaleChangedEvent?.Invoke(LastTimeScale);
            }
        }
        public bool IsPaused
        {
            get { return _isPaused; }
            set 
            { 
                _isPaused = value; 

                if (value)
                    Time.timeScale = 0f;
                else
                    Time.timeScale = LastTimeScale;

                OnPauseEvent?.Invoke(value);
            }
        }

        public event Action<bool> OnPauseEvent;
        public event Action<float> OnTimescaleChangedEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _isPaused = (LastTimeScale = Time.timeScale) == 0f;
        }
    }
}