using UnityEngine;

namespace AppCore
{
    public class DefaultLogger : ILogger
    {
        public void Error(string text)
        {
            Debug.LogError(text);
        }

        public void Message(string text)
        {
            Debug.Log(text);
        }

        public void Warning(string text)
        {
            Debug.LogWarning(text);
        }
    }
}
