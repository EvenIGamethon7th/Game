using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Spine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Enchant
{
    public class UI_EnchantPopUpContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mNameText;
        [SerializeField] private TextMeshProUGUI mSkillNameText;
        [SerializeField] private TextMeshProUGUI mAttackText;
        [SerializeField] private TextMeshProUGUI mDescText;
        [SerializeField] private Button mEnchantButton;
        
        [SerializeField] private UI_EnchantPopupSlot[] mEnchantPopupSlots;
        
        private MainCharacterData mMainCharacterData;
        private MainCharacterInfo mMainCharacterInfo;

        public void Start()
        {
            mEnchantButton.onClick.AddListener(OnEnchantButton);
            //Add Liteners
        }

        private void OnEnchantButton()
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
                UI_Toast_Manager.Instance.Activate_WithContent_Func("강화 성공!!");
            }
            else
                UI_Toast_Manager.Instance.Activate_WithContent_Func("강화 재료가 부족합니다.");
        }

        public void OnPopUp(Define.EnchantMainCharacterEvent data)
        {
            mMainCharacterData = data.data;
            mMainCharacterInfo = data.infoData;
            for (int i = 0; i < mEnchantPopupSlots.Length; i++)
            {
                CharacterData characterData = mMainCharacterInfo.CharacterEvolutions[i + 1].GetData;
                mEnchantPopupSlots[i].UpdateContent(characterData.Icon,mMainCharacterData,characterData,OnSelectSlot);
            }
            OnSelectSlot(mMainCharacterInfo.CharacterEvolutions[mMainCharacterData.rank].GetData);
        }

        private void OnSelectSlot(CharacterData data)
        {
            CharacterData currentCharacterData = mMainCharacterInfo.CharacterEvolutions[data.rank].GetData;
            mNameText.text = currentCharacterData.GetCharacterName();
            SkillData skillData = DataBase_Manager.Instance.GetSkill.GetData_Func(currentCharacterData.Skill1);
            mSkillNameText.text = $"Lv.{data.rank} {skillData.Name}";
            mAttackText.text = $"공격력: {currentCharacterData.atk}";
            mDescText.text = skillData.Description;
        }
    }
}