using _2_Scripts.Game.Sound;
using Cargold;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI
{
    public class UI_RerollButton : MonoBehaviour
    {
        [SerializeField]
        private Button mButton;

        [SerializeField]
        private UI_SummonButton[] mSummonButtons;

        private const int REROOL_COST = 20;

        [SerializeField] private UI_LockButton mLockButton;
        private void Start()
        {
            mButton.onClick.AddListener(OnClickReRollBtn);
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Gold, gold =>
            {
                if (gold < REROOL_COST)
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
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, -REROOL_COST);
            for (int i = 0; i < mSummonButtons.Length; ++i)
            {
                mSummonButtons[i].Reroll();
            }
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_EXP_Reroll_Touch);
        }


    }
}