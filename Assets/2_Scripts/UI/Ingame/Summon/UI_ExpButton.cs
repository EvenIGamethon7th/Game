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
            
            GameManager.Instance.UserGold.Subscribe(gold =>
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
            }).AddTo(this);
            
            GameManager.Instance.UserLevel.Subscribe(level =>
            {
                if (level >= Define.MAX_LEVEL)
                {
                    mExpButton.interactable = false;
                    mText.color = Color.red;
                }
            }).AddTo(this);
        }


        private bool IsMaxLevel=> GameManager.Instance.UserLevel.Value >= Define.MAX_LEVEL;
        
        private void OnBuyExp()
        {
            if (GameManager.Instance.UserGold.Value < mExpPrice || IsMaxLevel )
                return;

            Tween_C.OnPunch_Func(this.transform);
            GameManager.Instance.AddExp(mExpPrice);
            GameManager.Instance.UpdateMoney(EMoneyType.Gold,-mExpPrice);
        }
        
        private bool mIsButtonPressed = false;
        private float mRepeatRate = 1f; 
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