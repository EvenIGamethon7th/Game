using System;
using _2_Scripts.Utils;
using Cargold;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_Gold : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mText;
        [SerializeField] private GameObject mCoinImageGo;
        private void Start()
        {
            mText.text = $"{GameManager.Instance.UserGold:#,0}";
            MessageBroker.Default.Receive<EGameMessage>().Where(_message => _message == EGameMessage.GoldChange)
                .Subscribe(
                    _ =>
                    {
                        Tween_C.OnPunch_Func(mCoinImageGo.transform);
                        Tween_C.OnPunch_Func(mText.transform);
                        mText.text = $"{GameManager.Instance.UserGold:#,0}";
                    });
        }
    }
}