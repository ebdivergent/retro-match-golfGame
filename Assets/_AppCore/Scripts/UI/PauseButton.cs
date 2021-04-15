using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class PauseButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] bool _isPaused;

        public void OnPointerClick(PointerEventData eventData)
        {
            TimeManager.Instance.IsPaused = _isPaused;
        }
    }
}
