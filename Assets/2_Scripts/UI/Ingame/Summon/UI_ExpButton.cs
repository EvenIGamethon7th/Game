using System;
using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ExpButton : MonoBehaviour
    {
        [SerializeField]
        private Button mExpButton;

        [SerializeField]
        private int mExpPrice = 10;

        [SerializeField] private TextMeshProUGUI mText;

        private float holdInterval = 0.5f;
        
        private void Start()
        {
            var pointerDownStream = mExpButton.OnPointerDownAsObservable();
            var pointerUpStream = mExpButton.OnPointerUpAsObservable();

            var handleStream = pointerDownStream.SelectMany(_ =>
                Observable.Interval(TimeSpan.FromSeconds(holdInterval)).TakeUntil(pointerUpStream).RepeatUntilDestroy(this));

            handleStream.Subscribe(_ => OnBuyExp());

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

    }
}