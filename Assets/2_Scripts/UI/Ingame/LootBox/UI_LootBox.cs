using System;
using _2_Scripts.Utils.Components;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame.LootBox
{
    public class UI_LootBox : SerializedMonoBehaviour
    {
        [SerializeField] private Sprite defaultImage;
        
        [SerializeField]
        private UIImageAnimation mImageAnimation;
        
        [SerializeField]
        private GameObject mParticleEffect;
        
        private Image mImage;
        
        private void Start()
        {
            mImage = GetComponent<Image>();
        }
        
        public void OnOpenLootBox()
        {
            mImageAnimation.PlayAnimation("open",0.1f,OpenLootBoxAnimation);
            
        }
        private void OpenLootBoxAnimation()
        {
            mParticleEffect.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                CloseLootBoxAnimation();
            }).AddTo(this);
        }
        private void CloseLootBoxAnimation()
        {
            mImage.sprite = defaultImage;
            mParticleEffect.SetActive(false);
        }
        
    }
}