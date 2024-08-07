using Cargold.FrameWork.BackEnd;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class AcquireItemCatalog : MonoBehaviour,IItemAcquisition
    {
        public void AcquireItem(ItemKey itemKey, int amount)
        {
             var findItem = BackEndManager.Instance.CatalogItems.FirstOrDefault(x => x.ItemId == itemKey.GetData.code);
             BackEndManager.Instance.GrantItem(findItem.ItemId);
        }
    }
}