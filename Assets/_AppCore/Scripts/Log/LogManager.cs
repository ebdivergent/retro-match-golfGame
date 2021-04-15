using UnityEngine;

namespace AppCore
{
    public class LogManager : MonoBehaviour, ILogManager
    {
        public bool ThrowAllLogsToUI { get; set; }

        public ILogger DefaultLogger { get; private set; }
        public ILogger EditorLogger { get; private set; }

        public static ILogManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            DefaultLogger = new DefaultLogger();
            EditorLogger = new EditorOnlyLogger();
        }
    }
}
