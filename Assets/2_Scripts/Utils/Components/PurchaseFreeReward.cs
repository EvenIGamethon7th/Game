using Cargold.FrameWork.BackEnd;
using System;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class PurchaseFreeReward : MonoBehaviour,IPurchase
    {
        [SerializeField]
        private string mRewardKey;
        
        private Action mOnPurchaseSuccess;
        public void SetOnPurchaseSuccess(Action action)
        {
            mOnPurchaseSuccess = action;
        }
        public bool Purchase()
        {
            var freeRewardData = BackEndManager.Instance.UserFreeRewardData[mRewardKey];
            bool isPurchased = freeRewardData.RewardCountUp();
            if (!isPurchased)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("오늘은 모두 시청하셨습니다");
                return false;
            }
            //TODO : 광고 송출 
            if (freeRewardData.isAdReward && BackEndManager.Instance.UserInventory.All(x => x.ItemId != "Ad_Remover"))
            {
                //TODO
                AdmobManager.Instance.ShowRewardedAd(mOnPurchaseSuccess);
                Debug.Log("광고 송출 끝!");
                return true;
            }
            mOnPurchaseSuccess?.Invoke();
            return true;
        }

        public string GetPriceOrCount()
        {
            return $"{BackEndManager.Instance.UserFreeRewardData[mRewardKey].RewardCount}/{BackEndManager.Instance.UserFreeRewardData[mRewardKey].RewardMaxCount}";
        }
    }
}