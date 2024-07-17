using System;
using System.Collections.Generic;
using Cargold;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_RerollButton : MonoBehaviour
    {
        [SerializeField]
        private Button mButton;

        [SerializeField]
        private UI_SummonButton[] mSummonButtons;


        [SerializeField] private UI_LockButton mLockButton;
        private void Start()
        {
            mButton.onClick.AddListener(OnClickReRollBtn);
        }

        public void OnClickReRollBtn()
        {
            if(mLockButton.IsLock)
            {
                return;
            }
            Tween_C.OnPunch_Func(transform);
            foreach (var btn in mSummonButtons)
            {
                btn.Reroll();
            }
        }
        
        
    }
}