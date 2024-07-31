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

        [SerializeField]
        private Button mButton;

        [SerializeField] private ILootBoxReward mIReward;
        
        
        private void Start()
        {
            mImage = GetComponent<Image>();
        }
        
        public void OnOpenLootBox()
        {
            if (IngameDataManager.Instance.CurrentLuckyCoin <= 0)
            {
                Cargold.UI.UI_Toast_Manager.Instance.Activate_WithContent_Func("열쇠가 부족합니다.");
                return;
            }

            if (!mIReward.CanReward())
            {
                Cargold.UI.UI_Toast_Manager.Instance.Activate_WithContent_Func(mIReward.RewardMessage());
                return;
            }
            IngameDataManager.Instance.UpdateMoney(EMoneyType.GoldKey,-1);
            mImageAnimation.PlayAnimation("open",0.1f,OpenLootBoxAnimation);
            mButton.interactable = false;
        }
        private void OpenLootBoxAnimation()
        {
            mParticleEffect.SetActive(true);
            mIReward.Reward();
            Cargold.UI.UI_Toast_Manager.Instance.Activate_WithContent_Func(mIReward.RewardMessage());
            Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
            {
                CloseLootBoxAnimation();
            }).AddTo(this);
        }
        private void CloseLootBoxAnimation()
        {
            mImage.sprite = defaultImage;
            mParticleEffect.SetActive(false);
            mButton.interactable = true;
        }
        
    }
}