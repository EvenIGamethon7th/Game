using System;
using UnityEngine;
using UnityEngine.Purchasing;
namespace _2_Scripts.Utils.Components
{
    public class PurchaseIAP : MonoBehaviour,IPurchase
    {
        [SerializeField] private int mPrice;
        [SerializeField] private string productId;
        
        private IStoreController mStoreController;
        private IExtensionProvider mExtensionProvider;
        
        public Action OnPurchaseSuccess { get; private set; }
        
        public void PurchaseSuccessCallback(Action action)
        {
            OnPurchaseSuccess = action;
            IAPManager.Instance.AddPurchaseSuccessCallback(productId, OnPurchaseSuccess);
        }

        public bool Purchase()
        {
            return IAPManager.Instance.BuyProduct(productId);
        }

        public string GetPriceOrCount()
        {
            return IAPManager.Instance.GetPriceOrCount(productId, mPrice);
        }
    }
}