using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
#if Purchase_Unity_C
    public class 인앱결제매니저_유니티인앱 : Cargold.SDK.Purchase.InAppSystem_UnityPurchase
    {
        public static new 인앱결제매니저_유니티인앱 Instance;

        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if (_layer == 0)
            {
                Instance = this;
            }
        }
    } 
#endif
} // End