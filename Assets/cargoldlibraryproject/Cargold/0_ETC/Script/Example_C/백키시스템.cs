using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
public class 백키시스템 : Cargold.BackKeySystem.BackKey_Manager
{
    public static new 백키시스템 Instance;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if(_layer == 0)
        {
            Instance = this;
        }
    }

    protected override void OnExitApp_Func()
    {
        Debug_C.Log_Func("앱 종료");
    }
}
} // End