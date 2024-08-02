using System;
using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        private Color mOriginColor;

        private int mCount;

        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<int>>()
                .Where(m => m.Message == EGameMessage.StageChange)
                .Subscribe(m =>
                {
                    StartTimerAsync(20).Forget();
                })
                .AddTo(this);
        }

        private async UniTask StartTimerAsync(int val)
        {
            ++mCount;
            float temp = val;
            int standard = val - 1;
            text.text = $"00:{val}";
            Tween_C.OnPunch_Func(this);
            while (temp > 0)
            {
                await UniTask.DelayFrame(1);
                if (mCount > 1 && temp < 15) break;

                if (text == null) break;

                temp -= Time.deltaTime;
                text.text = $"00:{(int)temp}";

                if (temp < 5)
                {
                    text.color = Color.red;
                }
                else
                {
                    text.color = mOriginColor;
                }

                if ((int)temp != standard)
                {
                    standard = (int)temp;
                    Tween_C.OnPunch_Func(this);
                }
            }

            --mCount;
        }
    }
}