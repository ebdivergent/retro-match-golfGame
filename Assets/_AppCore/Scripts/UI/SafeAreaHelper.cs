using UnityEngine;

namespace AppCore
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] RectTransform _screenRect;

        RectTransform _rt;
        Vector2Int _lastScreenSize;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _lastScreenSize = Vector2Int.zero;
        }

        private void Start()
        {
            RecalculateCheck();

#if !(UNITY_EDITOR || !(UNITY_IOS || UNITY_ANDROID))
            Destroy(this);
#endif
        }

#if UNITY_EDITOR || !(UNITY_IOS || UNITY_ANDROID)
        private void Update()
        {
            RecalculateCheck();
        }
#endif

        private void RecalculateCheck()
        {
            if (_lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
            {
                _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

                Recalculate();
            }
        }

        private void Recalculate()
        {
            Vector2 multiplier = _rt.rect.size;

            multiplier.x /= (float)Screen.width;
            multiplier.y /= (float)Screen.height;

            Rect rect = Screen.safeArea;

            _screenRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,
                rect.yMin * multiplier.y,
                (Screen.height - ((Screen.height - (int)rect.yMax) + rect.yMin)) * multiplier.y
                );
        }
    }
}
