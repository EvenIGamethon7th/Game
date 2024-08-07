using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Shop
{
    public class UI_ShopItemSlot : SerializedMonoBehaviour
    {
        // UI가 상점 항목마다 달라서 그냥 노가다로.. 해야할듯?
        [SerializeField]
        private ItemKey mItemKey;
        [SerializeField]
        private ProductDetailsKey mProductDetailsKey;
        [SerializeField]
        private int mAmount;
        [SerializeField]
        private IPurchase mPurchaseCondition;
        [SerializeField]
        private IItemAcquisition mItemAcquisition;
        [SerializeField]
        private Button mPurchaseButton;
        [SerializeField]
        private Button mInfoButton;
        [SerializeField]
        private TextMeshProUGUI mPriceText;
        
        private GameMessage<ProductDetailsData> mProductDetailMessage = new GameMessage<ProductDetailsData>(EGameMessage.ProductDetailPopUp,null);
        public void Start()
        {
            if (mInfoButton != null && mProductDetailsKey != null)
            {
                mProductDetailMessage.SetValue(mProductDetailsKey.GetData);
                mInfoButton.onClick.AddListener(InfoClick);
            }
            mPurchaseButton.onClick.AddListener(Purchase);
            mPriceText.text = mPurchaseCondition.GetPriceOrCount();
        }

        private void Purchase()
        {
            if (mPurchaseCondition.Purchase())
            {
                mItemAcquisition.AcquireItem(mItemKey, mAmount);
            }
        }
        private void InfoClick()
        {
            MessageBroker.Default.Publish(mProductDetailMessage);
        }
    }
}