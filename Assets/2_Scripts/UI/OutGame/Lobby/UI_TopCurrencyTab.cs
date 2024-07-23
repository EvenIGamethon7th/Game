using System;
using Cargold.FrameWork.BackEnd;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_TopCurrencyTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mFeatherText;
        [SerializeField] private TextMeshProUGUI mDiamondText;
        [SerializeField] private Button mTestButton; 
        private void Start()
        {
            BackEndManager.Instance.UserCurrency[ECurrency.Father].Subscribe(father =>
            {
                mFeatherText.text = $"{father}/10";
            }).AddTo(this);
            BackEndManager.Instance.UserCurrency[ECurrency.Diamond].Subscribe(Diamond =>
            {
                mDiamondText.text = $"{Diamond}";
            }).AddTo(this);
            
            mTestButton.onClick.AddListener(()=>BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond,100));
        }
    }
}