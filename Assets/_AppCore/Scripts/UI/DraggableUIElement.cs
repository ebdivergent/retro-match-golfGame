using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class DraggableUIElement : MonoBehaviour, IDragHandler
    {
        RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rt.position += (Vector3)eventData.delta;
        }
    }
}