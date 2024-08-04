using System.Collections.Generic;

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

       public static readonly Dictionary<int, int> MainCharacterEnchantAmountTable =
           new Dictionary<int, int>() { { 1, 3 }, { 2, 5 }, { 3, 0 } };
       public const int MAX_LEVEL = 6;
    }
}