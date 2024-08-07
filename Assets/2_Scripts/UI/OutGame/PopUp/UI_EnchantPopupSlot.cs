using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Enchant
{
    public class UI_EnchantPopupSlot : MonoBehaviour
    {
        [SerializeField] private Image mCharacterImage;
        [SerializeField] private TextMeshProUGUI mAmountText;
        [SerializeField] private GameObject mBlindObject;
        [SerializeField] private GameObject mUpgradeIcon;
        [SerializeField] private int mLevel;
        [SerializeField] private Button mButton;


        
        public void UpdateContent(Sprite sprite, MainCharacterData data,CharacterData characterData,Action<CharacterData> callbackData)
        {
            IconInit();
            mCharacterImage.sprite = sprite;
            mButton.onClick.AddListener(()=>callbackData.Invoke(characterData));
            if(data.isGetType == EGetType.Lock || data.rank < mLevel)
            {
                mBlindObject.SetActive(true);
                mAmountText.text = "미획득";
                return;
            }
            
            if (mLevel < data.rank || (mLevel == data.rank && data.rank == 3))
            {
                mAmountText.text = "획득완료";
            }
            else if(mLevel == data.rank )
            {
                var needAmount = Define.MainCharacterEnchantAmountTable[data.rank];
                if (data.amount >= needAmount)
                {
                    mUpgradeIcon.SetActive(true);
                }
                mAmountText.text = $"{data.amount}/{needAmount}";
            }
        }

        private void IconInit()
        {
            mUpgradeIcon.SetActive(false);
            mBlindObject.SetActive(false);
        }
    }
}