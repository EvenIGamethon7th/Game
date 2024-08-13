using System;
using UnityEngine;
using UnityEngine.Purchasing;
namespace _2_Scripts.Utils.Components
{
    public class PurchaseIAP : MonoBehaviour,IPurchase,IStoreListener
    {
        [SerializeField] private int mPrice;
        [SerializeField] private string productId;
        
        private IStoreController mStoreController;
        private IExtensionProvider mExtensionProvider;
        
        public Action OnPurchaseSuccess { get; private set; }
        
        public void PurchaseSuccessCallback(Action action)
        {
            OnPurchaseSuccess = action;
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

            // 제품을 추가합니다. (등록한 제품 ID와 맞춰주세요)
            builder.AddProduct(productId, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            return mStoreController != null && mExtensionProvider != null;
        }


        public bool Purchase()
        {
            //TODO 
            if (IsInitialized())
            {
                Product product = mStoreController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    mStoreController.InitiatePurchase(product);
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

        public string GetPriceOrCount()
        {
            if (IsInitialized())
            {
                Product product = mStoreController.products.WithID(productId);
                if (product != null)
                {
                    return $"KRW \n {product.metadata.localizedPriceString}";
                }
            }
            return $"KRW \n {mPrice}";
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError(error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func($"네트워크 문제.{message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (String.Equals(purchaseEvent.purchasedProduct.definition.id, productId, StringComparison.Ordinal))
            {
                Debug.Log($"ProcessPurchase: PASS. Product: '{purchaseEvent.purchasedProduct.definition.id}'");
                // 여기서 실제로 아이템을 지급하거나, 상태를 업데이트합니다.
                UI_Toast_Manager.Instance.Activate_WithContent_Func("구매 완료!");
                OnPurchaseSuccess?.Invoke();
                return PurchaseProcessingResult.Complete;
            }
            else
            {
                Debug.Log($"ProcessPurchase: FAIL. Unrecognized product: '{purchaseEvent.purchasedProduct.definition.id}'");
                return PurchaseProcessingResult.Pending;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func("구매 취소");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            mStoreController = controller;
            mExtensionProvider = extensions;
        }
    }
}