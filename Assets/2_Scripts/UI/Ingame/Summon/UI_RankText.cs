using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_RankText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mText;

        public void Start()
        {
            GameManager.Instance.UserLevel.Subscribe(level =>
            {
                mText.text = $"{level}학년";
            }).AddTo(this);
        }
    }
}