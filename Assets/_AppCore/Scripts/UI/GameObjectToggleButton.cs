using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class GameObjectToggleButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject _goToActivate;

        public void OnPointerClick(PointerEventData eventData)
        {
            _goToActivate.SetActive(!_goToActivate.activeSelf);
        }
    }
}