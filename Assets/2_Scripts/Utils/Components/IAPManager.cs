using System.Collections.Generic;

namespace _2_Scripts.Utils.Components
{
    using System;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public class IAPManager : Singleton<IAPManager>, IStoreListener
    {
        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;
        private Dictionary<string,Action> mPurchaseSuccessCallbackList = new Dictionary<string, Action>();

        public void AddPurchaseSuccessCallback(string productId, Action action)
        {
            if (mPurchaseSuccessCallbackList.ContainsKey(productId))
            {
                mPurchaseSuccessCallbackList[productId] = action;
                return;
            }
            mPurchaseSuccessCallbackList.Add(productId, action);
        }
        private void Start()
        {
            InitializePurchasing();
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Product Catalog를 사용하여 모든 제품을 한 번에 등록
            ProductCatalog catalog = ProductCatalog.LoadDefaultCatalog();
            foreach (var product in catalog.allProducts)
            {
                builder.AddProduct(product.id, product.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }
        private bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        public bool BuyProduct(string productId)
        {
            //TODO 
            
            if (IsInitialized())
            {
                Product product = _storeController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    _storeController.InitiatePurchase(product);
                    return true;
                }
                else
                {
                    UI_Toast_Manager.Instance.Activate_WithContent_Func("상품 정보가 없습니다.");
                }
            }
            else
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("네트워크 시스템이 이상합니다.");   
            }
            return false;
        }

        public string GetPriceOrCount(string productId,int defaultPrice)
        {
            if (IsInitialized())
            {
                Product product = _storeController.products.WithID(productId);
                if (product != null)
                {
                    return $"KRW \n {product.metadata.localizedPriceString}";
                }
            }
            return $"KRW \n {defaultPrice}";
            
        }
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
            Debug.Log("IAP Manager initialized.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
          UI_Toast_Manager.Instance.Activate_WithContent_Func($"네트워크 문제.{error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func($"네트워크 문제.{message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"ProcessPurchase: PASS. Product: '{purchaseEvent.purchasedProduct.definition.id}'");
            UI_Toast_Manager.Instance.Activate_WithContent_Func("구매 완료!");
            mPurchaseSuccessCallbackList[purchaseEvent.purchasedProduct.definition.id]?.Invoke();
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func("구매 취소");
        }

    }

}