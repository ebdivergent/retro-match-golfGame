using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AppCore
{
    public class LogSubscriptionToggleButton : MonoBehaviour
#if DEBUG
        , IPointerClickHandler
#endif
    {
        [SerializeField] private Text _statusText;

#if DEBUG
        private void OnEnable()
        {
            UILoggerController.Instance.OnSubscribedEvent += LoggerSubscribedHandler;
            LoggerSubscribedHandler(UILoggerController.Instance.IsSubscribed);
        }

        private void OnDisable()
        {
            UILoggerController.Instance.OnSubscribedEvent -= LoggerSubscribedHandler;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UILoggerController.Instance.IsSubscribed = !UILoggerController.Instance.IsSubscribed;
        }
        private string GetStatusToText(bool status)
        {
            return status ? "Subscribed" : "Unsubscribed";
        }

        private void LoggerSubscribedHandler(bool subscribed)
        {
            _statusText.text = GetStatusToText(subscribed);
        }
#endif
    }
}
