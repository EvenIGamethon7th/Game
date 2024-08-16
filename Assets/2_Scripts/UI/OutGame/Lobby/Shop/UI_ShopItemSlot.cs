using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using _2_Scripts.Utils.Components;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System;
using System.Linq;
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
        
        
        private GameMessage<Define.RewardEvent> mRewardEventMessage;
        private GameMessage<ProductDetailsData> mProductDetailMessage = new GameMessage<ProductDetailsData>(EGameMessage.ProductDetailPopUp,null);
        [SerializeField]
        private bool isOneTimePurchase = false;
        public void Start()
        {
            if (isOneTimePurchase && BackEndManager.Instance.UserInventory.Any(x=>x.ItemId == mItemKey.GetData.code))
            {
                gameObject.SetActive(false);
                return;
            }
            mPurchaseCondition = GetComponent<IPurchase>();
            mItemAcquisition = GetComponent<IItemAcquisition>();
            mRewardEventMessage = new GameMessage<Define.RewardEvent>(EGameMessage.RewardOpenPopUp, new Define.RewardEvent()
            {
                count = mAmount,
                name = mItemKey.GetData.name,
                sprite = mItemKey.GetData.Icon
            });
            if (mInfoButton != null && mProductDetailsKey.GetData != null)
            {
                mProductDetailMessage.SetValue(mProductDetailsKey.GetData);
                mInfoButton.onClick.AddListener(InfoClick);
            }
            mPurchaseButton.onClick.AddListener(Purchase);
            mPriceText.text = mPurchaseCondition.GetPriceOrCount();
        }

        private void Purchase()
        {
            if (mPurchaseCondition is PurchaseFreeReward)
            {
                PurchaseFreeReward purchaseIap = (PurchaseFreeReward) mPurchaseCondition;
                purchaseIap.SetOnPurchaseSuccess(PurchaseItem);
                mPurchaseCondition.Purchase();
                mPriceText.text = mPurchaseCondition.GetPriceOrCount();
                return;
            }
            
            if (!mPurchaseCondition.Purchase())
            {
                return;
            }
            if (mPurchaseCondition is PurchaseIAP)
            {
                PurchaseIAP purchaseIap = (PurchaseIAP) mPurchaseCondition;
                purchaseIap.PurchaseSuccessCallback(PurchaseItem);
                return;
            }
            
            PurchaseItem();
        }

        private void PurchaseItem()
        {
            mItemAcquisition.AcquireItem(mItemKey, mAmount);
            MessageBroker.Default.Publish(mRewardEventMessage);
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Get_Item);
            if (isOneTimePurchase)
            {
                this.gameObject.SetActive(false);
            }
        }
        
        private void InfoClick()
        {
            MessageBroker.Default.Publish(mProductDetailMessage);
        }
    }
}