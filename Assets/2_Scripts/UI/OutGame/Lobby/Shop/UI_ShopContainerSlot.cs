using _2_Scripts.Utils;
using _2_Scripts.Utils.Components;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Shop
{
    public class UI_ShopContainerSlot : SerializedMonoBehaviour
    {
        [SerializeField] private string mContainerId;
        [SerializeField] private ProductDetailsKey mProductDetailsKey;
        [Tooltip("true면 한 번만 구매")]
        [SerializeField] private bool mOnePurchase;
        [SerializeField]
        private IPurchase mPurchaseCondition;
        [SerializeField]
        private Button mPurchaseButton;
        [SerializeField]
        private Button mInfoButton;

        [SerializeField] private TextMeshProUGUI mPriceText;
        private GameMessage<ProductDetailsData> mProductDetailMessage = new GameMessage<ProductDetailsData>(EGameMessage.ProductDetailPopUp,null);
        // 현재 슬롯 최대 3개라 제한을 풀던 더 만들던 해야할듯!
        private GameMessage<List<Define.RewardEvent>> mRewardEventMessage;
        // 랜덤이건, 고정이건 상관없이 품목 리스트를 담아줘야 팝업 연출에 나옴
        private List<Define.RewardEvent> mRewardEvents = new List<Define.RewardEvent>();
        public void Start()
        {
            OnVisible();
            mPurchaseCondition = GetComponent<IPurchase>();
            mPriceText.text = this.mPurchaseCondition.GetPriceOrCount();
            mRewardEventMessage = new GameMessage<List<Define.RewardEvent>>(EGameMessage.RewardOpenPopUp, mRewardEvents);
            var itemData = BackEndManager.Instance.CatalogItems.Find(x => x.ItemId == mContainerId);
            var items = itemData.Container;
            Dictionary<string, int> itemCountDic = new Dictionary<string, int>();
            foreach (var item in items.ItemContents)
            {
                if (itemCountDic.ContainsKey(item))
                {
                    itemCountDic[item]++;
                }
                else
                {
                    itemCountDic[item] = 1;
                }
            }

            foreach (var vc in items.VirtualCurrencyContents)
            {
               var data = DataBase_Manager.Instance.GetItem.GetDataArr.FirstOrDefault(x => x.code == vc.Key);
                mRewardEvents.Add(new Define.RewardEvent
                {
                    name = data.name,
                    sprite = data.Icon,
                    count = (int)vc.Value,
                });
            }

            foreach (var item in itemCountDic)
            {
                var data = DataBase_Manager.Instance.GetItem.GetDataArr.FirstOrDefault(x => x.code == item.Key);
                if (data == null)
                {
                    continue;
                }
                mRewardEvents.Add(new Define.RewardEvent
                {
                    name = data.name,
                    sprite = data.Icon,
                    count = item.Value,
                });
            }
            if (mInfoButton != null && mProductDetailsKey.GetData != null)
            {
                mProductDetailMessage.SetValue(mProductDetailsKey.GetData);
                mInfoButton.onClick.AddListener(InfoClick);
            }
            mPurchaseButton.onClick.AddListener(Purchase);
        }

        private void OnVisible()
        {
            if (mOnePurchase && BackEndManager.Instance.UserInventory.Any(x => x.ItemId == $"{mContainerId}_Lock"))
            {
                gameObject.SetActive(false);
            }
        }

        private void Purchase()
        {
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
            else
            {
                PurchaseItem();
            }
        }
        
        private void PurchaseItem()
        {
            BackEndManager.Instance.GrantItem(mContainerId, () =>
            {
                BackEndManager.Instance.OpenContainerItem(mContainerId);
                OnVisible();
            });
            MessageBroker.Default.Publish(mRewardEventMessage);
        }

        private void InfoClick()
        {
            throw new System.NotImplementedException();
        }
    }
}