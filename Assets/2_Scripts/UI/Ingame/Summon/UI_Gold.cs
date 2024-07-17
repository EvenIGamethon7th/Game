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
            GameManager.Instance.UserGold.Subscribe(gold =>
            {
                mText.text = $"{gold:#,0}";
                Tween_C.OnPunch_Func(mCoinImageGo.transform);
            }).AddTo(this);
        }
    }
}