using System.Collections.Generic;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class CharacterRewardBox : MonoBehaviour,ILootBoxReward
    {
        private List<int> rates = new() { 50, 40, 10 };
        private string mRewadMessage;
        public string RewardMessage()
        {
            return mRewadMessage;
        }

        public void Reward()
        {
            var characterData = GetRandomCharacter();
            if (characterData.rank == 3)
            {
                SoundManager.Instance.Vibrate();
            }
            string characterName = DataBase_Manager.Instance.GetLocalize.GetData_Func(characterData.nameKey).ko;
            mRewadMessage = $"짜잔! {characterData.rank}등급 '{characterName}' 소환!";
            MapManager.Instance.CreateUnit(characterData, spawnAction: (tilePos) =>
            {
                ObjectPoolManager.Instance.CreatePoolingObject(
                Define.SpawnEffectDictionary[characterData.rank], tilePos);
            });
        }

        public bool CanReward()
        {
            bool canCreateUnit = MapManager.Instance.CanCreateUnit();
            if (!canCreateUnit)
            {
                mRewadMessage = $"현재 유닛 슬롯 칸이 모두 꽉 찼습니다.";
            }

            return canCreateUnit;
        }

        private CharacterData GetRandomCharacter()
        {
            var characterInfo = GameManager.Instance.RandomCharacterCardOrNull(); 
            var rate = Random.Range(0, 100);
            var sum = 0;
            for (var i = 0; i < rates.Count; i++)
            {
                sum += rates[i];
                if (rate < sum)
                {
                    // 0 Normal 1 Rare 2 Epic
                    return characterInfo.CharacterEvolutions[i+1].GetData;
                }
            }

            return null;
        }
    }
}