using System;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_EnchantButtonController : MonoBehaviour
    {
        [SerializeField]
        private UI_TabButton[] mTabButtons;
        private UI_TabButton mSelectedTabButton;


        [SerializeField] private TextMeshProUGUI mDescText;
        private string[] mTabDesc =
        {
            "강화 시 메인 캐릭터의\n<color=#FACB33>외형이 성장</color>하며\n<color=#FACB33>스킬이 강력</color>해집니다.",
            "강화 시 각 <color=#FACB33>클래스 </color>학생들의\n<color=#FACB33>공격력 혹은 마력</color>이 \n10% 상승합니다.(최대 20강)"
            
        };
        public void Start()
        {
            foreach(var btn in mTabButtons)
            {
                btn.InitButton(OnSelectButton);
            }
            mTabButtons[0].OnSelectButton();
        }

        private void OnSelectButton(UI_TabButton tabButton)
        {
            if (mSelectedTabButton != null && mSelectedTabButton == tabButton)
            {
                return;
            }
            mDescText.text = mTabDesc[Array.IndexOf(mTabButtons, tabButton)];
            mSelectedTabButton = tabButton;
            foreach (var btn in mTabButtons)
            {
                if (btn == tabButton)
                {
                    btn.OnSelectButton();
                }
                else
                {
                    btn.OnDeselectButton();
                }
            }
        }
    }
}