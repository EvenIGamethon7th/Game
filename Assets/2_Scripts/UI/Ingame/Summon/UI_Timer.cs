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

        private bool mIsTimer = false;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<int>>()
                .Where(m => m.Message == EGameMessage.StageChange)
                .Subscribe(m =>
                {
                    mIsTimer = true;
                    StartTimer();
                })
                .AddTo(this);
            //최초 실행
            StartTimer();
        }

        private void StartTimer()
        {
            int startTime = 15;
            Observable.Interval(System.TimeSpan.FromSeconds(1))
                .TakeWhile(_ => startTime >= 1)
                .Subscribe(_ =>
                    {
                        startTime--;
                        text.text = $"{startTime} Sec";
                        Tween_C.OnPunch_Func(this);
                    }, 
                    () =>
                    {
                        mIsTimer = false;
                    })
                .AddTo(this);
        }
    }
}