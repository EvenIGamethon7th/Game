using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class PurchaseIAP : MonoBehaviour,IPurchase
    {
        [SerializeField] private int mPrice;
        public bool Purchase()
        {
            //TODO 
            return false;
        }

        public string GetPriceOrCount()
        {
            //TODO
            return $"KRW \n {mPrice}";
        }
    }
}