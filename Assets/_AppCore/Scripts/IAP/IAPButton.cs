using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EasyMobile;

namespace AppCore
{
    public class IAPProductButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] string _productName;
        [SerializeField] Text _nameText;
        [SerializeField] Text _priceText;
        [SerializeField] CanvasGroup _controlInputCanvasGroup;

        private bool _initialized;
        private bool _subscribed = false;

        private void SubscribeToIAPInitialization(bool value)
        {
            if (_subscribed != value)
            {
                if (_subscribed = value)
                {
                    IAPController.Instance.OnInitializedEvent += OnInitialized;
                }
                else
                {
                    IAPController.Instance.OnInitializedEvent -= OnInitialized;
                }
            }
        }

        private void OnEnable()
        {
            if (!_initialized)
            {
                var initializedIAP = IAPController.Instance.IsInitialized;

                SetIAPEnabled(initializedIAP);

                if (initializedIAP)
                {
                    OnInitialized();
                }
                else
                {
                    SubscribeToIAPInitialization(true);
                }
            }
        }

        private void OnDisable()
        {
            if (!_initialized)
            {
                SubscribeToIAPInitialization(false);
            }
        }

        private void OnInitialized()
        {
            _initialized = true;

            SubscribeToIAPInitialization(false);

            UpdateItem();
        }

        protected virtual void SetIAPEnabled(bool enabled)
        {
            _controlInputCanvasGroup.alpha = enabled ? 1f : 0.5f;
            _controlInputCanvasGroup.interactable = enabled;
        }

        protected virtual void OnPurchaseResultHandler(IAPProduct product, PurchaseResult purchaseResult)
        {

        }

        protected virtual void OnPurchaseStartedHandler()
        {

        }

        protected virtual bool CanPurchase()
        {
            return !ProductOwned();
        }

        protected virtual void UpdateItem()
        {
#if EM_UIAP
            var data = IAPController.Instance.GetProductLocalizedData(_productName);

            if (data != null)
            {
                _nameText.text = data.localizedTitle;
                _priceText.text = data.localizedPriceString;
            }
#endif
        }

        protected virtual bool ProductOwned()
        {
            return IAPController.Instance.IsOwned(_productName);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanPurchase())
            {
                return;
            }

            if (IAPController.Instance.Purchase(_productName, OnPurchaseResultHandler))
            {
                OnPurchaseStartedHandler();
            }
        }
    }
}