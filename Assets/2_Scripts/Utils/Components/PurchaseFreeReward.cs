using Cargold.FrameWork.BackEnd;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class PurchaseFreeReward : MonoBehaviour,IPurchase
    {
        [SerializeField]
        private string mRewardKey;
        public bool Purchase()
        {
            bool isPurchased = BackEndManager.Instance.UserFreeRewardData[mRewardKey].RewardCountUp();
            if (!isPurchased)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("오늘은 모두 시청하셨습니다");
            }

            return isPurchased;
        }

        public string GetPriceOrCount()
        {
            return $"{BackEndManager.Instance.UserFreeRewardData[mRewardKey].RewardCount}/{BackEndManager.Instance.UserFreeRewardData[mRewardKey].RewardMaxCount}";
        }
    }
}