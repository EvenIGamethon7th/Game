using System;

namespace _2_Scripts.Game.BackEndData.Enchant
{
    [Serializable]
    public class CharacterEnchantData
    {
        public string ClassTypeKey;
        public int EnchantLevel;
        public bool isMaxEnchant;
        
        public CharacterEnchantData(string classTypeKey, int enchantLevel, bool isMaxEnchant)
        {
            ClassTypeKey = classTypeKey;
            EnchantLevel = enchantLevel;
            this.isMaxEnchant = isMaxEnchant;
        }

        //돈 계산은 알아서하라하고 여긴 Level만 관리
        public void Enchant()
        {
            if(isMaxEnchant == false)
                EnchantLevel++;
            
        }
    }
}