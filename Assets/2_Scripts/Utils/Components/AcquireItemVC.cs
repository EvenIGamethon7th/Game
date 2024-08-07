using Cargold.FrameWork.BackEnd;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class AcquireItemVC : MonoBehaviour,IItemAcquisition
    {
        public void AcquireItem(ItemKey itemKey, int amount)
        {
            ECurrency vcKey = global::Utils.GetEnumFromDescription<ECurrency>(itemKey.GetData.code);
            BackEndManager.Instance.AddCurrencyData(vcKey, amount);
        }
    }
}