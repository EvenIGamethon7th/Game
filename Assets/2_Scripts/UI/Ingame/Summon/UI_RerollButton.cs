using _2_Scripts.Game.BackEndData.Stage;
using _2_Scripts.Game.Sound;
using Cargold;
using TMPro;
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

        private int mRerollCost = 20;

        [SerializeField] private UI_LockButton mLockButton;

        [SerializeField] private TextMeshProUGUI mText;
        
        private void Start()
        {
            
            mButton.onClick.AddListener(OnClickReRollBtn);
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Gold, gold =>
            {
                if (gold < mRerollCost)
                {
                    mButton.interactable = false;
                }
                else
                {
                    mButton.interactable = true;
                }
            });
        }

        public void OnClickReRollBtn()
        {
            //UI_Toast_Manager.Instance.Activate_WithContent_Func("Touch Reroll", isIgnoreTimeScale: true);
            if (mLockButton.IsLock)
            {
                return;
            }
            Tween_C.OnPunch_Func(transform);
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, -mRerollCost);
            if (GameManager.Instance.CurrentStageData.StageType == StageType.Survive)
            {
                mRerollCost += 10;
            }
            for (int i = 0; i < mSummonButtons.Length; ++i)
            {
                mSummonButtons[i].Reroll();
            }
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_EXP_Reroll_Touch);
            mText.text = $"{mRerollCost}¿ø\n ´Ù½Ã »Ì±â";
        }


    }
}