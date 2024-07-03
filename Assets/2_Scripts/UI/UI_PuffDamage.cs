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
            MessageBroker.Default.Receive<GameMessage<float>>()
                .Where(message => message.Message == EGameMessage.PlayerDamage)
                .Subscribe(message =>
                {
                    mPaticleImage.Play();
                }).AddTo(this);
        }
    }
}