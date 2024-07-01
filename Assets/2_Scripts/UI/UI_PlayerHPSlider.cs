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
        private global::Utils.ReadonlyNumber<float> mMaxHp;
        public void Start()
        {
            mMaxHp = new global::Utils.ReadonlyNumber<float>(GameManager.Instance.UserHp);
            mHpText.text = $"{mMaxHp.Value}/{mMaxHp.Value}";
            mSlider.value = 1f;
            
            MessageBroker.Default.Receive<GameMessage<float>>()
                .Where(message => message.Message == EGameMessage.PlayerDamage)
                .Subscribe(message =>
                {
                    float damagePercentage = message.Value / mMaxHp.Value;
                    mSlider.value -= damagePercentage;
                    mHpText.text = $"{Mathf.RoundToInt(mSlider.value * mMaxHp.Value)}/{mMaxHp.Value}";
                }).AddTo(this);
        }
    }
}