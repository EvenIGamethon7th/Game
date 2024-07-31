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
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Level, level =>
            {
                mText.text = $"{level}학년";
            });
        }
    }
}