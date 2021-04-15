using UnityEngine;

namespace AppCore
{
    public enum AnalyticsGameStateEvent
    {
        Start,
        Fail,
        Complete
    }

    public abstract class AnalyticsProvider : MonoBehaviour
    {
        public abstract void SendFirstLaunch();
        public abstract void SendAppLaunch();
        public abstract void SendGameStateEvent(AnalyticsGameStateEvent stateEvent, int level);
        public abstract void SendEvent(string name, string value);
    }
}