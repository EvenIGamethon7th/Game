using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class PurchaseVC : SerializedMonoBehaviour , IPurchase
    {
        [SerializeField] private ECurrency mCurrency;
        [SerializeField] private int mPrice;
        public bool Purchase()
        {
            if(BackEndManager.Instance.UserCurrency[mCurrency].Value < mPrice)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("재화가 부족합니다.");
                return false;
            }
            BackEndManager.Instance.AddCurrencyData(mCurrency, -mPrice);
            return true;
        }

        public string GetPriceOrCount()
        {
            return mPrice.ToString();
        }
    }
}