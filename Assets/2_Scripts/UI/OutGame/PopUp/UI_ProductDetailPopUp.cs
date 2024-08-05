using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.PopUp
{
    public class UI_ProductDetailPopUp : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mTitleText;
        [SerializeField]
        private TextMeshProUGUI mDescriptionText;
        [SerializeField]
        private Image mProductImage;
        

        public void UpdateContent(ProductDetailsData data)
        {
            mProductImage.sprite = data.Icon;
            mTitleText.text = data.name;
            mDescriptionText.text = data.desc;
        }
        
    }
}