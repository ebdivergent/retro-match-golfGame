using UnityEngine;

namespace AppCore
{
    public class EditorOnlyLogger : ILogger
    {
        public void Message(string text)
        {
#if UNITY_EDITOR
            Debug.Log(text);
#endif
        }

        public void Warning(string text)
        {
#if UNITY_EDITOR
            Debug.LogWarning(text);
#endif
        }

        public void Error(string text)
        {
#if UNITY_EDITOR
            Debug.LogError(text);
#endif
        }
    }
}
