using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI
{
    public class Tutorial_Reroll : MonoBehaviour
    {
        [SerializeField]
        private Button mButton;

        [SerializeField]
        private UI_SummonButton[] mSummonButtons;

        [SerializeField]
        private CharacterInfo[] mInfos;

        private const int REROOL_COST = 20;

        [SerializeField] private UI_LockButton mLockButton;

        void Start()
        {
            mButton.onClick.AddListener(OnClickReRollBtn);
        }

        private void OnClickReRollBtn()
        {
            if (mLockButton.IsLock)
            {
                return;
            }
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_EXP_Reroll_Touch);
            Tween_C.OnPunch_Func(transform);
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, -REROOL_COST);
            for (int i = 0; i < mSummonButtons.Length; ++i)
            {
                if (i == 1) mSummonButtons[i].Reroll(mInfos[i], 3);
                else mSummonButtons[i].Reroll(mInfos[i], 1);
            }

            MessageBroker.Default.Publish(new GameMessage<bool>(EGameMessage.TutorialProgress, true));
        }
    }
}