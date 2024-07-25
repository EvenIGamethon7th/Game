using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacterCoolTime : MonoBehaviour
    {
        private Image mBackground;
        private TextMeshProUGUI mCoolTimeText;

        private void Awake()
        {
            mBackground = GetComponent<Image>();
            mCoolTimeText = GetComponentInChildren<TextMeshProUGUI>(true);

            MessageBroker.Default.Receive<GameMessage<float>>()
                .Where(message => message.Message == EGameMessage.MainCharacterCoolTime)
                .Subscribe(message =>
                {
                    SetCoolTime(message.Value);
                }).AddTo(this);
        }

        public void Init(Sprite sprite)
        {
            //mBackground.sprite = sprite;
            SetCoolTime(0);
        }

        private void SetCoolTime(float time)
        {
            if (time <= 0)
            {
                mCoolTimeText.text = $"��� ����";
            }

            else if (time < 1)
            {
                mCoolTimeText.text = $"{time.ToString("F1")}��";
            }

            else
            {
                mCoolTimeText.text = $"{(int)time}��";
            }
        }
    }
}