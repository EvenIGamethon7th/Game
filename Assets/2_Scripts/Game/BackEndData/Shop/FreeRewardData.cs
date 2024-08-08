﻿using System;
using UnityEngine;

namespace _2_Scripts.Game.BackEndData.Shop
{
    [Serializable]
    public class FreeRewardData
    {
        public string RewardKey;
        public int RewardCount;
        public int RewardMaxCount;
        public bool isAdReward;
        
        public bool RewardCountUp()
        {
      
            if (RewardCount + 1 > RewardMaxCount)
            {
                return false;
            }
            ++RewardCount;
            //TODO : 광고 송출 
            if (isAdReward)
            {
                //TODO
                Debug.Log("광고 송출 끝!");
            }
            return true;
        }
    }
}