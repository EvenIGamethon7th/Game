using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _2_Scripts.UI.OutGame.PopUp
{
    public class UI_EncyclopediaPopUp : MonoBehaviour
    {
        private CharacterInfo mInfo;

        [SerializeField]
        private List<Button> mButtons = new();
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
                mButtons[temp].onClick.AddListener(() => SetText(temp));
            }
        }

        public void OnPopUp(CharacterInfo info)
        {
            mInfo = info;
        }

        private void SetText(int num)
        {
            mAtk.text = mInfo.CharacterEvolutions[num].GetData.atk.ToString();
            mAtkSpeed.text = mInfo.CharacterEvolutions[num].GetData.atkSpeed.ToString();
            mMAtk.text = mInfo.CharacterEvolutions[num].GetData.matk.ToString();
            mRank.text = num.ToString();
            mCoolTime.text = mInfo.ActiveSkillList[num - 1].CoolTime.ToString();
        }
    }
}