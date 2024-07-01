using System;
using _2_Scripts.Utils;
using Rito.Attributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_PlayerHPSlider : MonoBehaviour
    {
        [GetComponent]
        private Slider mSlider;

        [SerializeField] private TextMeshProUGUI mHpText;
        private float mMaxHp;
        public void Start()
        {
            mMaxHp = GameManager.Instance.UserHp;
            mHpText.text = $"{mMaxHp}/{mMaxHp}";
            mSlider.value = 1f;
            
            MessageBroker.Default.Receive<GameMessage<float>>()
                .Where(message => message.Message == EGameMessage.PlayerDamage)
                .Subscribe(message =>
                {
                    float damagePercentage = message.Value / mMaxHp;
                    mSlider.value -= damagePercentage;
                    mHpText.text = $"{Mathf.RoundToInt(mSlider.value * mMaxHp)}/{mMaxHp}";
                }).AddTo(this);
        }
    }
}