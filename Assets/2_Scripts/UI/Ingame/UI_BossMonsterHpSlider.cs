using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.Ingame
{
    public class UI_BossMonsterHpSlider : MonoBehaviour, IMonsterHpUI
    {
        [SerializeField]
        private UI_SlicedFilledImage mHpSlider;

        [SerializeField]
        private TextMeshProUGUI mHpText;

        private float mMaxHp;

        public bool Active { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public void InitHpUI(float maxHp)
        {
            mHpSlider.fillAmount = 1;
            mMaxHp = maxHp;
            mHpText.text = maxHp.ToString("F1");
            gameObject.SetActive(true);
        }

        public void SetHpUI(float currentHp)
        {
            mHpSlider.fillAmount = currentHp / mMaxHp;

            mHpText.text = currentHp.ToString("F1");
        }

        public void UpdatePos(Vector3 pos)
        {
            transform.localPosition = pos + Vector3.up * 2;
        }
    }
}