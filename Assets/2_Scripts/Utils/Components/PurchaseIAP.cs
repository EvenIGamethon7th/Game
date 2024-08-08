using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class PurchaseIAP : MonoBehaviour,IPurchase
    {
        [SerializeField] private int mPrice;
        public bool Purchase()
        {
            //TODO 
            UI_Toast_Manager.Instance.Activate_WithContent_Func("준비중 입니다");
            return false;
        }

        public string GetPriceOrCount()
        {
            //TODO
            return $"KRW \n {mPrice}";
        }
    }
}