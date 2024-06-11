using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.EventSystem;
namespace Cargold.Example {
public class 이벤트시스템매니저 : Cargold.EventSystem.EventSystem_Manager
{
    public static new 이벤트시스템매니저 Instance;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if(_layer == 0)
        {
            Instance = this;
        }
    }

    public override bool IsClearedEvent_Func(string _dataKey)
    {
        throw new System.NotImplementedException(); 
    }

    protected override void OnEventDone_Func(string _eventKey)
    {
        throw new System.NotImplementedException();
    }
}
} // End