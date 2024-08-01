using System;
using _2_Scripts.Utils;
using DG.Tweening;
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
            mMaxHp = new global::Utils.ReadonlyNumber<float>(IngameDataManager.Instance.MaxHp);
            mHpText.text = $"{mMaxHp.Value}/{mMaxHp.Value}";
            mSlider.value = 1f;
            IngameDataManager.Instance.HealHp += OnHealthBarUpdate;
        }

        public void OnHealthBarUpdate(int value)
        {
            float damagePercentage = (float)value / mMaxHp.Value;
            float newValue = mSlider.value - damagePercentage;
            mSlider.DOValue(newValue, 1f).OnUpdate(() =>
            { 
                mHpText.text = $"{Mathf.RoundToInt(mSlider.value * mMaxHp.Value)}/{mMaxHp.Value}";
            });
        }

        private void OnDestroy()
        {
            IngameDataManager.Instance.HealHp -= OnHealthBarUpdate;
        }
    }
}