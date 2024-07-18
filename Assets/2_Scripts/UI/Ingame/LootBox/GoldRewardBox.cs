using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class GoldRewardBox : MonoBehaviour , ILootBoxReward
    {
        private Dictionary<int,int> rates = new()
        {
            {0,40},
            {10,50},
            {150,5}
        };
        private string mRewadMessage;
        public string RewardMessage()
        {
            return mRewadMessage;
        }

        public void Reward()
        {
            var reward = GetRandomGoldReward();
            mRewadMessage = reward != 0  ? $"짜잔! {reward} 골드 획득!" : "ㅜㅜ 꽝!";
            GameManager.Instance.UpdateMoney(EMoneyType.Gold,reward);
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