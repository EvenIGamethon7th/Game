using System;
using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ExpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Button mExpButton;

        [SerializeField]
        private int mExpPrice = 10;

        [SerializeField] private TextMeshProUGUI mText;
        private IDisposable buttonHoldSubscription;
        private void Start()
        {
            mExpButton.onClick.AddListener(OnBuyExp);

            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Gold, gold =>
            {
                if (gold < mExpPrice)
                {
                    mExpButton.interactable = false;
                }
                else
                {
                    mExpButton.interactable = true;
                    mText.color = Color.white;
                }
            });

            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Level, level =>
            {
                if (level >= Define.MAX_LEVEL)
                {
                    mExpButton.interactable = false;
                    mText.color = Color.red;
                }
            });
        }


        private bool IsMaxLevel=> IngameDataManager.Instance.CurrentLevel >= Define.MAX_LEVEL;
        
        private void OnBuyExp()
        {
            if (IngameDataManager.Instance.CurrentGold < mExpPrice || IsMaxLevel )
                return;

            Tween_C.OnPunch_Func(this.transform);
            IngameDataManager.Instance.AddExp(mExpPrice);
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold,-mExpPrice);
        }
        
        private bool mIsButtonPressed = false;
        private float mRepeatRate = 0.5f; 
        public void OnPointerDown(PointerEventData eventData)
        {
            if (buttonHoldSubscription == null)
            {
                mIsButtonPressed = true;
                buttonHoldSubscription = Observable.EveryUpdate()
                    .Where(_ => mIsButtonPressed)
                    .ThrottleFirst(System.TimeSpan.FromSeconds(mRepeatRate))
                    .Subscribe(_ => OnBuyExp())
                    .AddTo(this);
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buttonHoldSubscription != null)
            {
                mIsButtonPressed = false;
                buttonHoldSubscription.Dispose();
                buttonHoldSubscription = null;
            }
        }
    }
}