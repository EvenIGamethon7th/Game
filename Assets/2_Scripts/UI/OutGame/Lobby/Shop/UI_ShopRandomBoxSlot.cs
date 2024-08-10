using _2_Scripts.Utils;
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
    public class UI_ShopRandomBoxSlot : SerializedMonoBehaviour
    {
        private Dictionary<string, float> mRandomItemTable = new ()
        {
            {"luck_01",60},
            {"luck_02",24},
            {"luck_03",15},
            {"luck_04",1}
        };

        [SerializeField]
        private ProductDetailsKey mProductDetailsKey;
        [SerializeField]
        private IPurchase mPurchaseCondition;
        
        [SerializeField] private TextMeshProUGUI mPriceText;
        [SerializeField] private Button mPurchaseButton;
        [SerializeField] private Button mInfoButton;
        
        private GameMessage<ProductDetailsData> mProductDetailMessage = new (EGameMessage.ProductDetailPopUp,null);
        private GameMessage<List<Define.RewardEvent>> mRewardEventMessage;
        private List<Define.RewardEvent> mRewardEvents = new ();
        private void Start()
        {
            mPurchaseCondition = GetComponent<IPurchase>();
            mPriceText.text = mPurchaseCondition.GetPriceOrCount();
            mProductDetailMessage.SetValue(mProductDetailsKey.GetData);
            mRewardEventMessage = new GameMessage<List<Define.RewardEvent>>(EGameMessage.RewardOpenPopUp, mRewardEvents);
            mInfoButton.onClick.AddListener(InfoClick);
            mPurchaseButton.onClick.AddListener(Purchase);
        }

        private void Purchase()
        {
            if (!mPurchaseCondition.Purchase())
            {
                return;
            } 
            string pickRandomItem = PickRandomItem();
            var itemContainer = BackEndManager.Instance.CatalogItems
                .FirstOrDefault(x => x.ItemId == pickRandomItem).Container;
            Dictionary<string, int> itemCountDic = new Dictionary<string, int>();
            if (itemContainer.VirtualCurrencyContents != null)
            {
                foreach (var vc in itemContainer.VirtualCurrencyContents)
                {
                    var data = DataBase_Manager.Instance.GetItem.GetDataArr.FirstOrDefault(x => x.code == vc.Key);
                    mRewardEvents.Add(new Define.RewardEvent
                    {
                        name = data.name,
                        sprite = data.Icon,
                        count = (int)vc.Value,
                    });
                }
            }
            if(itemContainer.ItemContents != null)
            {
                foreach (var item in itemContainer.ItemContents)
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
            BackEndManager.Instance.GrantItem(pickRandomItem, () =>
            {
              Dictionary<string, int> itemConsume =  new() { { pickRandomItem, 1 } };
              BackEndManager.Instance.OpenContainerItem(pickRandomItem);
               BackEndManager.Instance.UseInventoryItem(itemConsume);
            });
            MessageBroker.Default.Publish(mRewardEventMessage);
            mRewardEvents.Clear();
        }

        private void InfoClick()
        {
            MessageBroker.Default.Publish(mProductDetailMessage);
        }


        public string PickRandomItem()
        {
            // 전체 가중치 합 계산
            float totalWeight = mRandomItemTable.Values.Sum();

            // 0부터 totalWeight 사이의 랜덤 값 생성
            float randomValue = Random.Range(0f, totalWeight);

            // 랜덤 값에 따라 아이템 선택
            foreach (var item in mRandomItemTable)
            {
                if (randomValue < item.Value)
                {
                    return item.Key;
                }
                randomValue -= item.Value;
            }

            return null; // 이론상 도달하지 않음
        }
    }
}