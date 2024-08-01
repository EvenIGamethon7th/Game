using System;
using System.Collections.Generic;
using _2_Scripts.Utils;
using AssetKits.ParticleImage;
using Coffee.UIExtensions;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_DamageParticle : MonoBehaviour
    {
        [SerializeField]
        private UI_PlayerHPSlider mPlayerHPSlider;

        private ParticleImage mPaticleImage;

        public void Start()
        {
            mPaticleImage = GetComponent<ParticleImage>();

            IngameDataManager.Instance.DamageHp += UpdateHpBar;
        }

        private void UpdateHpBar(int damage)
        {
            mPaticleImage.rateOverTime = damage;
            mPaticleImage.Play();
            // 하나의 파티클로 플레이만 해주기 때문에 Action Callback 타이밍을 맞추기가 어려움
            // 해서 lifeTime (2초) 뒤에 그냥 실행되게끔 함
            DOVirtual.DelayedCall(mPaticleImage.lifetime.constantMax, () =>
            {
                mPlayerHPSlider.OnHealthBarUpdate(damage);
            });
        }

        private void OnDestroy()
        {
            IngameDataManager.Instance.DamageHp -= UpdateHpBar;
        }

    }
}