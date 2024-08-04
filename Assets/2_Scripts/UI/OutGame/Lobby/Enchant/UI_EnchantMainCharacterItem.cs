using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_EnchantMainCharacterItem : MonoBehaviour
    {
        [SerializeField] private Image mBorderImage;
        [SerializeField] private Sprite[] mBorderSpriteArr;
        [SerializeField] private Image mCharacterImage;
        [SerializeField] private TextMeshProUGUI mCharacterLevelText;
        [SerializeField] private Button mButton;
        [SerializeField] private TextMeshProUGUI mButtonText;
        [SerializeField] private GameObject mEnchantIcon;
        [SerializeField] private GameObject mBlindObject;

        private MainCharacterData mMainCharacterData;
        private MainCharacterInfo mMainCharacterInfo;
        
        private void Start()
        {
            mButton.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            if (mMainCharacterData == null || mMainCharacterData.isGetType == EGetType.Lock)
                return;
            if (mMainCharacterData.rank >= 3)
            {
                return;
            }

            if (mMainCharacterData.amount >= Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank])
            {
                mMainCharacterData.EnchantCharacter();
                UpdateSlot();
            }
            else
                UI_Toast_Manager.Instance.Activate_WithContent_Func("강화 재료가 부족합니다.");

        }

        public void InitItem(MainCharacterInfo data)
        {
            mMainCharacterInfo = data;
            BackEndManager.Instance.UserMainCharacterData.TryGetValue(data.name, out mMainCharacterData);
            UpdateSlot();
        }

        private void UpdateSlot()
        {
            mCharacterImage.sprite = mMainCharacterInfo.CharacterEvolutions[mMainCharacterData.rank].GetData.Icon;
            mCharacterLevelText.text = $"LV.{mMainCharacterData.rank}";
            mBorderImage.sprite = mBorderSpriteArr[mMainCharacterData.rank - 1];
            string buttonText = "강화완료";
            if (mMainCharacterData.rank < 3)
            {
                buttonText = $"{mMainCharacterData.amount}/{Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank]}";
            }
            if (mMainCharacterData.isGetType == EGetType.Lock)
            {
                mBlindObject.SetActive(true);
                buttonText = "미획득";
                mButton.interactable = false;
            }

            if (mMainCharacterData.amount >= Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank])
            {
                mEnchantIcon.SetActive(true);
            }
            mButtonText.text = buttonText;
        }


        // 다시 열었을 때 Update
        public void OnEnable()
        {
            
        }
    }
}