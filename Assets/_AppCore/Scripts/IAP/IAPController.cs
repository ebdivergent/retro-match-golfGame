using System;
using UnityEngine;
using EasyMobile;

namespace AppCore
{
    public interface IIAPController
    {
        bool IsInitialized { get; }
        bool IsActionInProgress { get; }

        event Action OnInitializedEvent;
        event Action OnRestoreStartedEvent;
        event Action<bool> OnRestoredEvent;
        event Action OnPurchaseStartedEvent;
        event Action<IAPProduct, PurchaseResult> OnPurchasedEvent;

        bool TryToInitialize();
        bool GetProductLocalizedPrice(string productName, out string price);
        bool GetProductLocalizedName(string productName, out string name);
#if EM_UIAP
        UnityEngine.Purchasing.ProductMetadata GetProductLocalizedData(string productName);
#endif
        IAPProduct GetProduct(string productName);
        bool IsOwned(string productName);

        bool RestorePurchases(Action<bool> onCompleteHandler);
        bool Purchase(string productName, Action<IAPProduct, PurchaseResult> onCompleteHandler);
    }

    public enum PurchaseResult
    {
        Deferred,
        Failed,
        Completed,
    }

    public class IAPController : MonoBehaviour, IIAPController
    {
        public static IIAPController Instance { get; private set; }

        public bool IsInitialized => InAppPurchasing.IsInitialized();
        public bool IsActionInProgress { get; private set; } = false;

        public event Action<bool> ActionInProgressEvent;
        public event Action AnyActionStartedEvent;
        public event Action AnyActionCompleteEvent;

        public event Action OnInitializedEvent;

        public event Action OnRestoreStartedEvent;
        public event Action<bool> OnRestoredEvent;

        public event Action OnPurchaseStartedEvent;
        public event Action<IAPProduct, PurchaseResult> OnPurchasedEvent;

        private Action<IAPProduct, PurchaseResult> _purchaseHandler;
        private Action<bool> _restoreHandler;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void SetActionState(bool complete)
        {
            IsActionInProgress = complete;

            ActionInProgressEvent?.Invoke(complete);

            (complete ? AnyActionCompleteEvent : AnyActionStartedEvent)?.Invoke();
        }

        private void OnEnable()
        {
            if ((IIAPController)this != Instance)
                return;

            InAppPurchasing.InitializeSucceeded += OnInitalizedHandler;
            InAppPurchasing.InitializeFailed += OnInitializeFailedHandler;

            InAppPurchasing.PurchaseCompleted += OnPurchaseCompletedHandler;
            InAppPurchasing.PurchaseDeferred += OnPurchaseDeferredHandler;
            InAppPurchasing.PurchaseFailed += OnPurchaseFailedHandler;
            InAppPurchasing.PromotionalPurchaseIntercepted += OnPromotionalPurchaseInterceptedHandler;

            InAppPurchasing.RestoreCompleted += OnRestoreCompleteHandler;
            InAppPurchasing.RestoreFailed += OnRestoreFailedHandler;
        }

        private void OnDisable()
        {
            if ((IIAPController)this != Instance)
                return;

            InAppPurchasing.InitializeSucceeded -= OnInitalizedHandler;
            InAppPurchasing.InitializeFailed -= OnInitializeFailedHandler;

            InAppPurchasing.PurchaseCompleted -= OnPurchaseCompletedHandler;
            InAppPurchasing.PurchaseDeferred -= OnPurchaseDeferredHandler;
            InAppPurchasing.PurchaseFailed -= OnPurchaseFailedHandler;
            InAppPurchasing.PromotionalPurchaseIntercepted -= OnPromotionalPurchaseInterceptedHandler;

            InAppPurchasing.RestoreCompleted -= OnRestoreCompleteHandler;
            InAppPurchasing.RestoreFailed -= OnRestoreFailedHandler;
        }

        public bool TryToInitialize()
        {
            if (EM_Settings.InAppPurchasing.IsAutoInit || InAppPurchasing.IsInitialized() || IsActionInProgress)
                return false;

            SetActionState(true);

            Debug.Log("AppCore IAP -> manual attempt to initialize IAP.");
            
            InAppPurchasing.InitializePurchasing();

            return true;
        }

        private void OnInitalizedHandler()
        {
            SetActionState(false);

            Debug.Log("AppCore IAP -> IAP initialized successfully.");

            OnInitializedEvent?.Invoke();
        }

        private void OnInitializeFailedHandler()
        {
            SetActionState(false);

            Debug.Log("AppCore IAP -> IAP failed to initialize.");

            TryToInitialize();
        }

        private void OnPurchaseInternalResultHandler(IAPProduct product, PurchaseResult purchaseResult)
        {
            SetActionState(false);

            _purchaseHandler?.Invoke(product, purchaseResult);

            OnPurchasedEvent?.Invoke(product, purchaseResult);

            Debug.Log($"AppCore IAP -> purchase result for product with id '{product.Id}' and name '{product.Name}' is '{purchaseResult.ToString()}'");
        }

        private void OnRestoreInternalResultHandler(bool result)
        {
            SetActionState(false);

            _restoreHandler?.Invoke(result);

            OnRestoredEvent?.Invoke(result);

            Debug.Log($"AppCore IAP -> restore result '{result}'");
        }

        private void OnPurchaseCompletedHandler(IAPProduct product)
        {
            OnPurchaseInternalResultHandler(product, PurchaseResult.Completed);
        }

        private void OnPurchaseFailedHandler(IAPProduct product)
        {
            OnPurchaseInternalResultHandler(product, PurchaseResult.Failed);
        }

        private void OnPurchaseDeferredHandler(IAPProduct product)
        {
            OnPurchaseInternalResultHandler(product, PurchaseResult.Deferred);
        }

        private void OnPromotionalPurchaseInterceptedHandler(IAPProduct product)
        {
            Debug.Log("AppCore IAP -> promotional purchase of product " + product.Name + " has been intercepted.");

            InAppPurchasing.ContinueApplePromotionalPurchases();
        }

        private void OnRestoreCompleteHandler()
        {
            OnRestoreInternalResultHandler(true);
        }

        private void OnRestoreFailedHandler()
        {
            OnRestoreInternalResultHandler(false);
        }

        public bool RestorePurchases(Action<bool> onCompleteHandler)
        {
            if (IsActionInProgress)
                return false;

            try
            {
                InAppPurchasing.RestorePurchases();
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                return false;
            }

            SetActionState(true);
            _restoreHandler = onCompleteHandler;

            OnRestoreStartedEvent?.Invoke();

            Debug.Log("AppCore IAP -> attempt to restore purchases");

            return true;
        }

		public bool GetProductLocalizedPrice(string productName, out string price)
		{
#if EM_UIAP
            var data = InAppPurchasing.GetProductLocalizedData(productName);
            price = data?.localizedPriceString ?? "";
            return data == null;
#else
            price = "";
            return false;
#endif
        }

        public bool GetProductLocalizedName(string productName, out string name)
        {
#if EM_UIAP
            var data = InAppPurchasing.GetProductLocalizedData(productName);
            name = data?.localizedTitle ?? "";
            return data == null;
#else
            name = "";
            return false;
#endif
        }

#if EM_UIAP
        public UnityEngine.Purchasing.ProductMetadata GetProductLocalizedData(string productName)
        {
            return InAppPurchasing.GetProductLocalizedData(productName);
        }
#endif

        public IAPProduct GetProduct(string productName)
        {
            return InAppPurchasing.GetIAPProductByName(productName);
        }

        public bool IsOwned(string productName)
        {
            return InAppPurchasing.IsProductOwned(productName);
        }

        public bool Purchase(string productName, Action<IAPProduct, PurchaseResult> onCompleteHandler)
        {
            if (IsActionInProgress)
                return false;

            try
            {
                InAppPurchasing.Purchase(productName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                return false;
            }

            SetActionState(true);
            _purchaseHandler = onCompleteHandler;

            OnPurchaseStartedEvent?.Invoke();

            Debug.Log($"AppCore IAP -> attempt to purchase product with name '{productName}'");

            return true;
        }
    }
}
