using System.Collections.Generic;
using _2_Scripts.Utils;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class CharacterRewardBox : MonoBehaviour,ILootBoxReward
    {
        private List<int> rates = new() { 65, 25, 10 };
        private string mRewadMessage;
        public string RewardMessage()
        {
            return mRewadMessage;
        }

        public void Reward()
        {
            var characterData = GetRandomCharacter();
            string characterName = DataBase_Manager.Instance.GetLocalize.GetData_Func(characterData.nameKey).ko;
            mRewadMessage = $"짜잔! {characterData.rank}등급 '{characterName}' 소환!";
            MapManager.Instance.CreateUnit(characterData, spawnAction: (tilePos) =>
            {
                ObjectPoolManager.Instance.CreatePoolingObject(
                Define.SpawnEffectDictionary[characterData.rank], tilePos);
            });
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