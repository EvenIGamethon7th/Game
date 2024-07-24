using System;
using _2_Scripts.Utils;
using AssetKits.ParticleImage;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_PuffDamage : MonoBehaviour
    {       
        private ParticleImage mPaticleImage;
        private void Start()
        {
            mPaticleImage = GetComponent<ParticleImage>();
            GameManager.Instance.DamageHp += PlayParticle;
        }

        private void PlayParticle(float damage)
        {
            mPaticleImage.Play();
            CameraManager.Instance.DoShake(CameraManager.ECameraType.Main, isUnscale: true);
        }

        private void OnDestroy()
        {
            GameManager.Instance.DamageHp -= PlayParticle;
        }
    }
}