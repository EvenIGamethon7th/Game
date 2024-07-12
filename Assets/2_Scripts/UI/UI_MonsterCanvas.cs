using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_MonsterCanvas : MonoBehaviour
    {
        [SerializeField]
        private Canvas mHpCanvas;
        [SerializeField]
        private Slider mHpSlider;
        [SerializeField]
        private Image mHpColor;
        [SerializeField]
        private TextMeshProUGUI mHpText;

        private float mMaxHp;

        private void Awake()
        {
            mHpCanvas.worldCamera = UICamera.Instance.Camera;
        }

        public void InitHpSlider(float maxHp, bool isBoss)
        {
            mHpSlider.maxValue = maxHp;
            mHpSlider.value = maxHp;
            mMaxHp = maxHp;
            mHpColor.color = Color.green;
            mHpText.gameObject.SetActive(isBoss);
            mHpText.text = $"{mMaxHp} / {mMaxHp}";
        }

        public void SetHpSlider(float currentHp)
        {
            mHpSlider.value = currentHp;

            mHpColor.color = Color.Lerp(Color.red, Color.green, currentHp / mMaxHp);
            mHpText.text = $"{Mathf.Max(currentHp, 0)} / {mMaxHp}";
        }
    }
}