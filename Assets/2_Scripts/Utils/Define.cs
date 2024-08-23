using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Utils
{
    public static class Define
    {
       public enum EAttackType
        {
            Physical,
            Magical,
            TrueDamage
        }

        public enum EInstantKillType
        {
            None,
            Exile,
            Execution,
            Transition
        }

       public static readonly Dictionary<int, string> SpawnEffectDictionary = new Dictionary<int, string>
       {
           { 1, AddressableTable.Default_Open_BoxGift_2 },
           { 2, AddressableTable.Default_Open_BoxGift_3 },
           { 3, AddressableTable.Default_Open_BoxGift_5 },
       };

       public class EnchantMainCharacterEvent
       {
           public MainCharacterData data;
           public MainCharacterInfo infoData;
       }

       public class RewardEvent
       {
           public Sprite sprite;
           public string name;
           public int count;
           public Action rewardEvent;
       }
       
       public static readonly Dictionary<int, int> MainCharacterEnchantAmountTable =
           new Dictionary<int, int>() { { 1, 3 }, { 2, 5 }, { 3, 0 } };
       
       public const int MAX_LEVEL = 6;
       public const float SPAWN_COOL_TIME = 1.5f;
       public const float NEXT_WAVE_TIME = 20.0f;
    }
}