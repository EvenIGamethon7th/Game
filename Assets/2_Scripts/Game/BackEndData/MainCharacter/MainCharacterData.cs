using _2_Scripts.Utils;
using System;

namespace _2_Scripts.Game.BackEndData.MainCharacter
{
    public enum EGetType
    {
        Lock,
        Unlock,
        Select
    }
    
    [Serializable]
    public class MainCharacterData
    {
        public string key;
        public int rank;
        public bool isEquip;
        public EGetType isGetType;
        public int amount;
        public MainCharacterData(string key, int rank, bool isEquip, EGetType isGetType)
        {
            this.key = key;
            this.rank = rank;
            this.isEquip = isEquip;
            this.isGetType = isGetType;
            this.amount = 0;
        }
        public void AddAmount(int amount)
        {
            this.amount += amount;
        }

        public void EnchantCharacter()
        {
            if (rank >= 3)
                return;
            amount -= Define.MainCharacterEnchantAmountTable[rank++];
        }
    }
}