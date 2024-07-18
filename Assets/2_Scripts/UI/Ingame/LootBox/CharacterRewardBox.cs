using System.Collections.Generic;
using _2_Scripts.Utils;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class CharacterRewardBox : MonoBehaviour,ILootBoxReward
    {
        private List<int> rates = new() { 65, 25, 10 };
        public void Reward()
        {
            var characterData = GetRandomCharacter();
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