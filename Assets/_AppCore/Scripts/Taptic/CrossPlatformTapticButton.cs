using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class CrossPlatformTapticButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TapticType _tapticType = TapticType.Selection;

        public void OnPointerClick(PointerEventData eventData)
        {
            CrossPlatformTaptic.Call(_tapticType);
        }
    }
}