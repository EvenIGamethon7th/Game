using System;
using Cargold.FrameWork.BackEnd;
using UnityEngine;

namespace _2_Scripts.Game.BackEndData.Enchant
{
    [Serializable]
    public class CharacterEnchantData
    {
        [SerializeField]
        private string classTypeKeyString;

        public EEnchantClassType ClassTypeKey
        {
            get => (EEnchantClassType) Enum.Parse(typeof(EEnchantClassType), classTypeKeyString);
            set => classTypeKeyString = value.ToString();
        }
        public int EnchantLevel;
        public bool isMaxEnchant;
        
        public CharacterEnchantData(EEnchantClassType classTypeKey, int enchantLevel, bool isMaxEnchant)
        {
            ClassTypeKey = classTypeKey;
            EnchantLevel = enchantLevel;
            this.isMaxEnchant = isMaxEnchant;
        }

        public float GetEnchantStat()
        {
            if(EnchantLevel == 0)
                return 1;
            return DataBase_Manager.Instance.GetUnitEnchant.GetData_Func($"Enchant_{EnchantLevel-1}").Enchant_Stat;
        }

        //돈 계산은 알아서하라하고 여긴 Level만 관리
        public void Enchant()
        {
            if(isMaxEnchant == false)
                EnchantLevel++;
            if(DataBase_Manager.Instance.GetUnitEnchant.GetDataArr.Length <= EnchantLevel)
                isMaxEnchant = true;
            
            
        }
    }
}