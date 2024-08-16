using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.DailyReward
{
    public class UI_DailyRewardSlot : SerializedMonoBehaviour
    {
        [SerializeField]
        public ItemKey ItemKey { get; private set; }
        [SerializeField] private Image mItemImage;
        [SerializeField] private GameObject mCheckObject;
        [SerializeField] private TextMeshProUGUI mItemNameText;
        [SerializeField] private TextMeshProUGUI mItemAmountText;
        [SerializeField] public IItemAcquisition ItemAcquisition { get; private set; }
        [SerializeField] public int mAmount { get; private set; }
        [SerializeField]
        public int CurrentDaily { get; private set; }

        private void Start()
        {
            UpdateSlot();
        }

        public void SetItemKey(ItemKey key,IItemAcquisition itemAcquisition,int amount)
        {
            ItemKey = key;
            ItemAcquisition = itemAcquisition;
            mAmount = amount;
        }
        
        public void UpdateSlot()
        {
            if(ItemKey.GetData == null)
            {
                return;
            }
            ItemData item = ItemKey.GetData;
            mItemImage.sprite = item.Icon ;
            mItemNameText.text = item.name;
            mItemAmountText.text = mAmount.ToString();
            if (BackEndManager.Instance.UserDailyReward >= CurrentDaily)
            {
                mCheckObject.SetActive(true);
            }
             
        }
        
        public void OnClick()
        {
            if (BackEndManager.Instance.UserDailyReward >= CurrentDaily)
            {
                return;
            }
            BackEndManager.Instance.UpdateDailyReward();
            ItemAcquisition.AcquireItem(ItemKey,mAmount);
            UI_Toast_Manager.Instance.Activate_WithContent_Func("출석보상을 수령하였습니다.");
        }
        
    }
}