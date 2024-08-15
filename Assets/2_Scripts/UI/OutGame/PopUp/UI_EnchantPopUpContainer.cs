using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Spine;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Enchant
{
    public class UI_EnchantPopUpContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mNameText;
        [SerializeField] private TextMeshProUGUI mSkillNameText;
        [SerializeField] private TextMeshProUGUI mAttackText;
        [SerializeField] private TextMeshProUGUI mSkillCoolTimeText;
        [SerializeField] private TextMeshProUGUI mDescText;
        [SerializeField] private Button mEnchantButton;
        
        [SerializeField] private UI_EnchantPopupSlot[] mEnchantPopupSlots;
        
        private MainCharacterData mMainCharacterData;
        private MainCharacterInfo mMainCharacterInfo;

        private GameMessage<MainCharacterData> mMainCharacterLevelUpMessage;
        public void Start()
        {
            mMainCharacterLevelUpMessage =
                new GameMessage<MainCharacterData>(EGameMessage.MainCharacterLevelUp, mMainCharacterData);
            mEnchantButton.onClick.AddListener(OnEnchantButton);
        }

        private void OnEnchantButton()
        {
            if (mMainCharacterData == null || mMainCharacterData.isGetType == EGetType.Lock)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("캐릭터 미보유!");
                return;
            }
               
            if (mMainCharacterData.rank >= 3)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 모두 강화했습니다");
                return;
            }

            if (mMainCharacterData.amount >= Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank])
            {
                SoundManager.Instance.Play2DSound(AddressableTable.Sound_EggMon_Upgrade);
                mMainCharacterData.EnchantCharacter();
                UI_Toast_Manager.Instance.Activate_WithContent_Func("강화 성공!!");
                OnSelectSlot(mMainCharacterInfo.CharacterEvolutions[mMainCharacterData.rank].GetData);
                MessageBroker.Default.Publish(mMainCharacterLevelUpMessage);
                OnPopUp(new Define.EnchantMainCharacterEvent
                {
                    data = mMainCharacterData,
                    infoData = mMainCharacterInfo
                });
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
            mSkillCoolTimeText.text = $"쿨타임: {skillData.CoolTime}초";
            mDescText.text = skillData.Description;
        }
    }
}