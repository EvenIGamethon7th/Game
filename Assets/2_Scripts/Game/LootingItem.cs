using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class LootingItem : SerializedMonoBehaviour
    {
        [SerializeField] private SpriteRenderer mSpriteRenderer;
        [SerializeField] private TextMeshPro mText;
        
        [Tooltip("이동할 위치 Y값")]
        [SerializeField]
        private float mTargetY;
        
        [SerializeField]
        private Dictionary<EMoneyType,Sprite> mSprites;
        

        [Tooltip("이동 속도")] [SerializeField] private float mMoveDuration;
        [Tooltip("알파 속도")][SerializeField] private float mFadeDuration;
       
        private Tweener[] mAlpahTween = new Tweener[2];

        private void Start()
        {
            mAlpahTween[0] = mSpriteRenderer.DOFade(0, mFadeDuration).SetEase(Ease.Linear).SetAutoKill(false);
            mAlpahTween[1] = mText.DOFade(0, mFadeDuration).SetEase(Ease.Linear).SetAutoKill(false);
        }

        public void CreateItem(EMoneyType moneyType, int value)
        {
            mSpriteRenderer.sprite = mSprites[moneyType];
            mText.text = $"+{value}";
            float targetY = transform.position.y + mTargetY;
            transform.DOMoveY(targetY, mMoveDuration).SetEase(Ease.InOutSine).Play();
            
            foreach (var alphaTween in mAlpahTween)
            {
                alphaTween.Restart();
            }
            DOVirtual.DelayedCall(mFadeDuration, () => gameObject.SetActive(false));
        }

        public void CreateItem(string moneyKey, int value)
        {
            MoneyData money = DataBase_Manager.Instance.GetMoney.GetData_Func(moneyKey);
            CreateItem(money.Type, value);
        }

    }
