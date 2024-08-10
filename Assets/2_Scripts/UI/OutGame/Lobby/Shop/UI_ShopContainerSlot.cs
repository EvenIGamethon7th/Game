using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Shop
{
    public class UI_ShopContainerSlot : MonoBehaviour
    {
        [SerializeField] private string mContainerId;
        [SerializeField] private ProductDetailsKey mProductDetailsKey;
        [Tooltip("0이면 무제한 구매 가능")]
        [SerializeField] private int mPurchaseCount;
        [SerializeField]
        private IPurchase mPurchaseCondition;
        [SerializeField]
        private Button mPurchaseButton;
        [SerializeField]
        private Button mInfoButton;
        
        private GameMessage<ProductDetailsData> mProductDetailMessage = new GameMessage<ProductDetailsData>(EGameMessage.ProductDetailPopUp,null);
        // 현재 슬롯 최대 3개라 제한을 풀던 더 만들던 해야할듯!
        private GameMessage<List<Define.RewardEvent>> mRewardEventMessage;
        // 랜덤이건, 고정이건 상관없이 품목 리스트를 담아줘야 팝업 연출에 나옴
        private List<Define.RewardEvent> mRewardEvents = new List<Define.RewardEvent>();
        public void Start()
        {
            mPurchaseCondition = GetComponent<IPurchase>();
            mRewardEventMessage = new GameMessage<List<Define.RewardEvent>>(EGameMessage.RewardOpenPopUp, mRewardEvents);
            var itemData = BackEndManager.Instance.CatalogItems.Find(x => x.ItemId == mContainerId);
            var items = itemData.Container;
            Dictionary<string, int> itemCountDic = new Dictionary<string, int>();
            foreach (var item in items.ItemContents)
            {
                itemCountDic[item]++;
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

        private void Purchase()
        {
            if (!mPurchaseCondition.Purchase())
            {
                return;
            }
            BackEndManager.Instance.GrantItem(mContainerId, () =>
            {
                BackEndManager.Instance.OpenContainerItem(mContainerId);
            });
            MessageBroker.Default.Publish(mRewardEventMessage);
        }

        private void InfoClick()
        {
            throw new System.NotImplementedException();
        }
    }
}