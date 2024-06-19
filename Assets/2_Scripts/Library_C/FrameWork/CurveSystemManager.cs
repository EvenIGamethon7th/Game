using _2_Scripts.UI;
using UnityEngine;

namespace Cargold.FrameWork
{
    public class CurveSystemManager : CurveSystem.CurveSystemInfinite
    {
        public static CurveSystemManager Instance;
        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);
            if (_layer == 0)
            {
                Instance = this;
            }
        }
        /// <summary>
        /// UI 베지어 곡선을 통해 커브 연출과 함께 돈 증가
        /// </summary>
        /// <param name="gold"></param>
        public void AddGoldCurve(Vector2 startPos,Infinite.Infinite gold)
        {
            CurveSystemManager.Instance.OnCurveWealth(WealthType.Gold,gold,startPos);
        }
        public void OnCurveWealth(WealthType wealthType, Infinite.Infinite quantity, Vector2 startPos)
        {
            base.OnCurve_Func(quantity,10,Vector2.zero, (eachQuantity) =>
            {
                ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Demo_Coin, 10, true);
                UI_CurveElement curved = ObjectPoolManager.Instance
                    .CreatePoolingObject(AddressableTable.Demo_Coin, 
                        startPos,true).GetComponent<UI_CurveElement>();
                
                curved.Activate(wealthType,eachQuantity);
                return curved.CurvedClass;
            });
        }

    }
}