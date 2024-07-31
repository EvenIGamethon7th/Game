using System;

namespace _2_Scripts.Game.BackEndData.Enchant
{
    [Serializable]
    public class CharacterEnchantData
    {
        public string CharacterKey;
        public int EnchantLevel;
        public bool isMaxEnchant;
        
        public CharacterEnchantData(string characterKey, int enchantLevel, bool isMaxEnchant)
        {
            CharacterKey = characterKey;
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