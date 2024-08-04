using System;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_EnchantButtonController : MonoBehaviour
    {
        [SerializeField]
        private UI_TabButton[] mTabButtons;
        private UI_TabButton mSelectedTabButton;
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