using System;
using UnityEngine;

namespace _2_Scripts.Game.BackEndData.Mission
{
    [Serializable]
    public class SpawnMission
    {
        public string CharacterKey;
        public int SpawnCount;
        public bool IsGet;
        public bool IsEquip;
        
        public void AddSpawnCount(int count)
        {
            SpawnCount =  Math.Min(SpawnCount + count,100);
            IsGet = SpawnCount >= 100;
        }
    }
}