using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_DefaultMonsterHpSlider : MonoBehaviour, IMonsterHpUI
    {
        [SerializeField]
        private Slider mHpSlider;

        [SerializeField]
        private Image mHpColor;

        private float mMaxHp;

        public bool Active { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public void InitHpUI(float maxHp)
        {
            mHpSlider.maxValue = maxHp;
            mHpSlider.value = maxHp;
            mMaxHp = maxHp;
            mHpColor.color = Color.green;
            gameObject.SetActive(true);
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