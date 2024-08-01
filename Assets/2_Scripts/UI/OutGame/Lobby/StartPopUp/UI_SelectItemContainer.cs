using System;
using System.Collections.Generic;
using System.Linq;
using Cargold.FrameWork.BackEnd;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.StartPopUp
{
    public class UI_SelectItemContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemPrefab;

        private UI_SelectItem mSelectItem;
        private List<UI_SelectItem> mAllItems = new List<UI_SelectItem>();
        
        [SerializeField]
        private TextMeshProUGUI mItemName;
        
        [SerializeField]
        private TextMeshProUGUI mItemDescription;

        [SerializeField] private GameObject mBuyContainer;
        [SerializeField] 
        private Button mBuyButton;

        [SerializeField] 
        private TextMeshProUGUI mPriceText;
        
        [SerializeField]
        private GameObject mUseContainer;

        [SerializeField]
        private Button mUseButton;
        [SerializeField] 
        private TextMeshProUGUI mUseText;
        [SerializeField]
        private TextMeshProUGUI mRemainCountText;
        
        Dictionary<string,bool> mUseItems = new Dictionary<string, bool>();
        private void Start()
        {
            mBuyButton.onClick.AddListener(OnBuyItem);
            mUseButton.onClick.AddListener(OnUseItem);
            
            foreach (var item in BackEndManager.Instance.PublicStoreItems)
            {
                var itemObject = Instantiate(mItemPrefab, transform).GetComponent<UI_SelectItem>();
                itemObject.SetItem(item,OnClickItem);
                mAllItems.Add(itemObject);
            }
        }

        public void UseItems()
        {
            Dictionary<string, int> itemsToConsume = new ();
            foreach (var useItem in mUseItems)
            {
                if (useItem.Value)
                {
                    itemsToConsume.Add(useItem.Key,1);
                    //TODO GAMEMANAGER에 아이템 사용 로직 추가
                    if (useItem.Key.CompareTo("item_1st_instructor") == 0)
                        GameManager.Instance.UseItem(EItemType.Lecturer1st);
                    else if (useItem.Key.CompareTo("item_2st_instructor") == 0)
                        GameManager.Instance.UseItem(EItemType.Lecturer2nd);
                    else if (useItem.Key.CompareTo("item_hp_position") == 0)
                        GameManager.Instance.UseItem(EItemType.HpUp);
                }
            }

            if (itemsToConsume.Count == 0)
                return;
            
            BackEndManager.Instance.UseInventoryItem(itemsToConsume);
        }
        
        private void OnClickItem(UI_SelectItem storeItem)
        {
            if(mSelectItem == storeItem)
                return;
            
            mSelectItem = storeItem;
            mPriceText.text = $"{storeItem.StoreItem.VirtualCurrencyPrices["DI"]}";
            // TODO 아이템 보유중인지 확인
            var itemId = storeItem.StoreItem.ItemId;
            var itemInfo = BackEndManager.Instance.GetStoreItem(itemId);
            mItemName.text = itemInfo.DisplayName;
            mItemDescription.text = itemInfo.Description;
            var item = BackEndManager.Instance.GetInventoryItem(itemId);
            if (item != null)
            {
                DisplayUseContainer(item);
            }
            else
            {
               DisPlayBuyContainer();
            }
        }

        private void OnUseItem()
        {
            bool isUse = mSelectItem.SetSelect();

            mUseText.text = isUse ? "사용해제" : "사용하기";
            mUseItems[mSelectItem.StoreItem.ItemId] = isUse;
            if (isUse)
                RemoveSameClassItem();
        }

        private void RemoveSameClassItem()
        {
            var item = BackEndManager.Instance.GetInventoryItem(mSelectItem.StoreItem.ItemId);
            ItemInstance nonSelectItem = null;

            foreach (var nonSelect in mAllItems)
            {
                if (nonSelect == mSelectItem) continue;
                nonSelectItem = BackEndManager.Instance.GetInventoryItem(nonSelect.StoreItem.ItemId);
                if (nonSelectItem == null) continue;
                if (nonSelectItem.ItemClass.CompareTo(item.ItemClass) != 0 || !nonSelect.IsUseItem) continue;
                nonSelect.SetSelect();
                mUseItems[nonSelect.StoreItem.ItemId] = false;
            }
        }

        private void DisplayUseContainer(ItemInstance item)
        {
            mBuyContainer.SetActive(false);
            mUseContainer.SetActive(true);
            var count = 1;
            if(item != null)
                count = item.RemainingUses.Value;
            
            mRemainCountText.text = $"{count} 보유수량";
            mUseText.text = mSelectItem.IsUseItem ? "사용해제" : "사용하기";
        }
        private void DisPlayBuyContainer()
        {
            mBuyContainer.SetActive(true);
            mUseContainer.SetActive(false);
        }
        
        private void OnBuyItem()
        {
            BackEndManager.Instance.PurchasePopUpItem(mSelectItem.StoreItem, () =>
            {
                DisplayUseContainer(BackEndManager.Instance.GetInventoryItem(mSelectItem.StoreItem.ItemId));
                UI_Toast_Manager.Instance.Activate_WithContent_Func("구매완료");
            }, () =>
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("다이아가 부족합니다.");
            });    
        }
        
        
    }
}