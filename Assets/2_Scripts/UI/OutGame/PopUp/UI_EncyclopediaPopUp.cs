using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cargold.FrameWork.BackEnd;

namespace _2_Scripts.UI.OutGame.PopUp
{
    public class UI_EncyclopediaPopUp : MonoBehaviour
    {
        private CharacterInfo mInfo;

        [SerializeField]
        private List<Button> mButtons = new();

        [SerializeField]
        private TextMeshProUGUI mName;
        [SerializeField]
        private List<Image> mImages = new();
        [SerializeField]
        private TextMeshProUGUI mAtk;
        [SerializeField]
        private TextMeshProUGUI mAtkSpeed;
        [SerializeField]
        private TextMeshProUGUI mMAtk;
        [SerializeField]
        private TextMeshProUGUI mRank;
        [SerializeField]
        private TextMeshProUGUI mCoolTime;

        [SerializeField]
        private TextMeshProUGUI mDescription;

        private void Start()
        {
            for (int i = 0; i < mButtons.Count; ++i)
            {
                int temp = i + 1;
                mButtons[i].onClick.AddListener(() => SetText(temp));
            }
        }

        public void OnPopUp(CharacterInfo info)
        {
            mInfo = info;
            mName.text = mInfo.CharacterEvolutions[1].GetData.GetCharacterName();
            for (int i = 0; i < mImages.Count; ++i)
            {
                mImages[i].sprite = mInfo.CharacterEvolutions[i + 1].GetData.GetCharacterSprite();
            }
            SetText(1);
        }

        private void SetText(int num)
        {
            EEnchantClassType type = global::Utils.GetEnumFromDescription<EEnchantClassType>(mInfo.CharacterEvolutions[num].GetData.ClassType);
            float enchant = BackEndManager.Instance.GetEnchantData(type).GetEnchantStat();
            mAtk.text = $"공격력: {mInfo.CharacterEvolutions[num].GetData.atk * enchant}";
            mAtkSpeed.text = $"공격속도: {mInfo.CharacterEvolutions[num].GetData.atkSpeed}";
            mMAtk.text = $"마력: {mInfo.CharacterEvolutions[num].GetData.matk * enchant}";
            
            switch (num)
            {
                case 1:
                    mRank.text = $"{num}학년 스킬";
                    mDescription.text = "스킬 잠금";
                    mCoolTime.text = "쿨타임: -";
                    break;

                case 2:
                    mRank.text = $"<color=#D581FF>{num}학년</color> 스킬";
                    SetText();
                    break;

                case 3:
                    mRank.text = $"<color=#FFD54A>{num}학년</color> 스킬";
                    SetText();
                    break;
            }

            void SetText()
            {
                CharacterData data = mInfo.CharacterEvolutions[num].GetData;
                SkillData skillData = mInfo.CharacterEvolutions[num].GetData.GetSkillData(num - 1);
                data.SetSkillDataLoc(num - 1);
                if (data.SkillType.Contains("패"))
                {
                    mCoolTime.text = "패시브";
                }

                else
                {
                    mCoolTime.text = $"쿨타임: {skillData.CoolTime}초";
                }
                mDescription.text = data.SkillDesc;
            }

        }
    }
}