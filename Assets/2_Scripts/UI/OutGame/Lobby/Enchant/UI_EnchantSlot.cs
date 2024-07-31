using System;
using System.ComponentModel;
using _2_Scripts.Game.BackEndData.Enchant;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Enchant
{
    public class UI_EnchantSlot : SerializedMonoBehaviour 
    {

        
        [SerializeField]
        private TextMeshProUGUI mEnchantLevelText;
        [SerializeField]
        private TextMeshProUGUI mEnchantPriceText;
        [SerializeField]
        private Button mEnchantButton;
        [SerializeField]
        private EEnchantClassType mEnchantClassType;

        private int mEnchantCost = 0;
        
        private CharacterEnchantData mEnchantData;
        private void Start()
        {
            mEnchantData = BackEndManager.Instance.GetEnchantData(mEnchantClassType);
            UpdateDisplayText();
            mEnchantButton.onClick.AddListener(OnEnchantButton);
        }

        private void OnEnchantButton()
        {
            if (BackEndManager.Instance.UserCurrency[ECurrency.Diamond].Value < mEnchantCost)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("돈이 부족합니다.");
                return;
            }
            UI_Toast_Manager.Instance.Activate_WithContent_Func("강화 성공!");
            BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond,-mEnchantCost);
            mEnchantData.Enchant();
            UpdateDisplayText();
        }
        
        private void UpdateDisplayText()
        {
            var enchantLevel = mEnchantData.EnchantLevel;
            if (mEnchantData.isMaxEnchant)
            {
                mEnchantButton.interactable = false;
                enchantLevel = DataBase_Manager.Instance.GetUnitEnchant.GetDataArr.Length - 1;
            }
            
            var enchantTableData =DataBase_Manager.Instance.GetUnitEnchant.GetData_Func($"Enchant_{enchantLevel}");
            mEnchantCost = enchantTableData.Enchant_Price;
            mEnchantLevelText.text = $"Lv.{enchantTableData.Enchant_Step}";
            mEnchantPriceText.text = $"{mEnchantCost:#,0}";
        }
    }
}