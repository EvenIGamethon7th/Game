using Cargold;
using Cysharp.Threading.Tasks;
using UniRx;
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

        
        private const int REROOL_COST = 20;

        [SerializeField] private UI_LockButton mLockButton;
        private void Start()
        {
            mButton.onClick.AddListener(OnClickReRollBtn);
            GameManager.Instance.UserGold.Subscribe(gold =>
            {
                if (gold < REROOL_COST)
                {
                    mButton.interactable = false;
                }
                else
                {
                    mButton.interactable = true;
                }
            }).AddTo(this);
        }

        public void OnClickReRollBtn()
        {
            if(mLockButton.IsLock)
            {
                return;
            }
            Tween_C.OnPunch_Func(transform);
            GameManager.Instance.UpdateMoney(EMoneyType.Gold,-REROOL_COST);
            foreach (var btn in mSummonButtons)
            {
                btn.Reroll();
            }
        }
        
        
    }
}