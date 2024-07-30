using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_MonsterCanvas : UI_Base, IMonsterHpUI
    {
        [SerializeField]
        private Slider mHpSlider;

        [SerializeField]
        private Image mHpColor;

        private float mMaxHp;

        public bool Active { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public void InitHpUI(float maxHp, bool isBoss)
        {
            if (isBoss) 
            { 
                gameObject.SetActive(false);
                return;
            }

            mHpSlider.maxValue = maxHp;
            mHpSlider.value = maxHp;
            mMaxHp = maxHp;
            mHpColor.color = Color.green;
        }

        public void SetHpUI(float currentHp)
        {
            mHpSlider.value = currentHp;

            mHpColor.color = Color.Lerp(Color.red, Color.green, currentHp / mMaxHp);
        }

        public void UpdatePos(Vector3 pos)
        {
            transform.localPosition = pos + Vector3.up;
        }
    }
}