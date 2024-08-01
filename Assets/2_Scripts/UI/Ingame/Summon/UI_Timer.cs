using System;
using _2_Scripts.Utils;
using Cargold;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_Timer : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI text;

        private int mStartTime = 20;

        [SerializeField]
        private Color mOriginColor;

        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<int>>()
                .Where(m => m.Message == EGameMessage.StageChange)
                .Subscribe(m =>
                {
                    StartTimer();
                })
                .AddTo(this);
        }

        private void StartTimer()
        {
            Observable.Interval(System.TimeSpan.FromSeconds(1))
                .TakeWhile(_ => mStartTime >= 1)
                .Subscribe(_ =>
                    {
                        mStartTime--;
                        text.text = $"00:{mStartTime}";
                        if (mStartTime < 5)
                        {
                            text.color = Color.red;
                        }
                        else
                        {
                            text.color = mOriginColor;
                        }
                        Tween_C.OnPunch_Func(this);
                    }, 
                    () =>
                    {
                        mStartTime = 20;
                    })
                .AddTo(this);
        }
    }
}