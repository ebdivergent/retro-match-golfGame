using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class PageButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] PageType _page;

        public event Action<bool> OnClickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            bool result = UIController.Instance.SetPage(_page);

            OnClickEvent?.Invoke(result);
        }
    }
}