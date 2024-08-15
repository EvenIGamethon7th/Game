using _2_Scripts.Game.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class GoldRewardBox : MonoBehaviour , ILootBoxReward
    {
        private Dictionary<int,int> rates = new()
        {
            {10,70},
            {20,20},
            {100,10}
        };
        private string mRewadMessage;
        public string RewardMessage()
        {
            return mRewadMessage;
        }

        public void Reward()
        {
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Chest_Open);
            var reward = GetRandomGoldReward();
            if (reward == 100)
            {
                SoundManager.Instance.Play2DSound(AddressableTable.Sound_Get_Best);
                SoundManager.Instance.Vibrate();
            }
            mRewadMessage = reward != 0  ? $"짜잔! {reward} 골드 획득!" : "ㅜㅜ 꽝!";
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold,reward);
        }

        public bool CanReward()
        {
            return true;
        }

        private int GetRandomGoldReward()
        {
            var rate = Random.Range(0, 100);
            var sum = 0;

            foreach (var kvp in rates)
            {
                sum += kvp.Value;
                if (rate < sum)
                {
                    return kvp.Key;
                }
            }

            return 0; // 기본값으로 0 반환 (안전장치)
        }
    }
}