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
        public MainCharacterData(string key, int rank, bool isEquip, EGetType isGetType)
        {
            this.key = key;
            this.rank = rank;
            this.isEquip = isEquip;
            this.isGetType = isGetType;
        }
    }
}