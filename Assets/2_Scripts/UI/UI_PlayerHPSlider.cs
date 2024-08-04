using System;
using System.Threading;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
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
        private CancellationTokenSource mCts = new();

        public void Start()
        {
            mMaxHp = new global::Utils.ReadonlyNumber<float>(IngameDataManager.Instance.MaxHp);
            mHpText.text = $"{mMaxHp.Value}/{mMaxHp.Value}";
            mSlider.maxValue = mMaxHp.Value;
            mSlider.value = mMaxHp.Value;
            IngameDataManager.Instance.HealHp += OnHealthBarUpdate;
        }

        public void OnHealthBarUpdate(int value)
        {
            //float damagePercentage = (float)value / mMaxHp.Value;
            //float newValue = mSlider.value - damagePercentage;
            UpdateHealthAsync(value, isHeal: value < 0).Forget();
            //mSlider.DOValue(newValue, 1f).OnUpdate(() =>
            //{ 
            //    mHpText.text = $"{Mathf.RoundToInt(mSlider.value * mMaxHp.Value)}/{mMaxHp.Value}";
            //});
        }

        private async UniTask UpdateHealthAsync(float damage, float time = 1, bool isHeal = false)
        {
            mHpText.text = $"{mSlider.value - damage}/{mMaxHp.Value}";
            damage = Mathf.Abs(damage);
            float waitTime = time / damage;
            while (damage > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), ignoreTimeScale: true, cancellationToken: mCts.Token);
                --damage;

                mSlider.value += isHeal ? 1 : -1;
            }
        }

        private void OnDestroy()
        {
            IngameDataManager.Instance.HealHp -= OnHealthBarUpdate;
        }
    }
}