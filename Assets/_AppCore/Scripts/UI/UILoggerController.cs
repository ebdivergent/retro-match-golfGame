using AppCore.Pooling;
using DG.Tweening;
using System;
using UnityEngine;

namespace AppCore
{
    public class UILoggerController : MonoBehaviour, IUILoggerController
    {
        [SerializeField] Pool _uiTextsPool;
        [SerializeField] float _defaultShowDelay;

        bool _subscribed = false;

        public event Action<bool> OnSubscribedEvent;

        public bool IsSubscribed 
        { 
            get { return _subscribed; } 
            set
            {
                if (value != _subscribed)
                {
                    _subscribed = value;

                    if (value)
                        Application.logMessageReceived += OnMessageReceived;
                    else
                        Application.logMessageReceived -= OnMessageReceived;

                    OnSubscribedEvent?.Invoke(value);
                }
            }
        }

        public static IUILoggerController Instance { get; private set; }

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

        private void OnDestroy()
        {
            IsSubscribed = false;
        }

        private void OnMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    Message(condition);
                    break;
                case LogType.Warning:
                    Warning(condition);
                    break;
                case LogType.Error:
                    Error(condition);
                    break;
                case LogType.Exception:
                    Error(condition);
                    break;
                case LogType.Assert:
                    Error(condition);
                    break;
            }
        }

        private void ShowText(string message, Color color, float delay = 0f)
        {
            var logElement = _uiTextsPool.Get<UILogElement>();

            logElement.Setup(message, color, DateTime.Now.ToString("HH:mm:ss"), color);
            logElement.transform.SetAsFirstSibling();

            delay = Mathf.Abs(delay);

            if (delay > Mathf.Epsilon)
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    _uiTextsPool.Return(logElement.gameObject);
                });
            }
        }

        private void Error(string text)
        {
            ShowText(text, Color.red, _defaultShowDelay);
        }

        private void Message(string text)
        {
            ShowText(text, Color.gray, _defaultShowDelay);
        }

        private void Warning(string text)
        {
            ShowText(text, Color.yellow, _defaultShowDelay);
        }
    }
}