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
            IngameDataManager.Instance.DamageHp += PlayParticle;
        }

        private void PlayParticle(int damage)
        {
            mPaticleImage.Play();
            CameraManager.Instance.DoShake(CameraManager.ECameraType.Main, isUnscale: true);
        }

        private void OnDestroy()
        {
            IngameDataManager.Instance.DamageHp -= PlayParticle;
        }
    }
}