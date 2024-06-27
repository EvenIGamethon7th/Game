using System;
using Cargold.CurveSystem;
using Cargold.FrameWork;
using Cargold.Infinite;
using Cargold.PoolingSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_CurveElement : MonoBehaviour
    {
        [ShowInInspector,ReadOnly]
        public CurvedClass CurvedClass { get; private set; }

        private WealthType _wealthType;
        private Infinite _quantity;
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            CurvedClass = new CurvedClass(this.transform,CallCurveArrival,CurveSystemManager.Instance);
        }

        public void Activate(WealthType wealthType,Infinite quantity)
        {
            gameObject.SetActive(true);
            _wealthType = wealthType;
            _quantity = quantity;
            
            // Obj Manager?
        }

        private void Deactivate()
        {
            this.gameObject.SetActive(false);
        }

        private void CallCurveArrival()
        {
            UserSystem_Manager.Instance.wealth.TryGetWealthControl_Func(
                Cargold.FrameWork.UserSystem_Manager.WealthControl.Earn, _wealthType, (int)_quantity);
            Deactivate();
        }

    }
}