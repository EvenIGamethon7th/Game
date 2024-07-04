using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_SummonButton : SerializedMonoBehaviour
    {
        public enum ESummonButtonState
        {
            Selected,
            Disable
        }
        
        [SerializeField]
        private TextMeshProUGUI mCharacterName;
        [SerializeField]
        private TextMeshProUGUI mCharacterCost;
        
        [SerializeField]
        private SkeletonGraphic mCharacterGraphic;
        
        [SerializeField]
        private Dictionary<ESummonButtonState,GameObject> mViewList = new();

        private ESummonButtonState mCurrentSummonButtonState;



        public void OnSummonButton()
        {
            if (mCurrentSummonButtonState == ESummonButtonState.Disable)
            {
                return;
            }
            
        }
        
        public void UpdateCharacter()
        {
            
        }
        
    }
}