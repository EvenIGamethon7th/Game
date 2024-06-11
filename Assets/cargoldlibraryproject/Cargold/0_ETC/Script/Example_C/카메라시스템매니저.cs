using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
public class 카메라시스템매니저 : Cargold.CameraSystem_Manager
{
    public static new 카메라시스템매니저 Instance;

    public override void Init_Func(int _layer)
    {
        if(_layer == 0)
        {
            Instance = this;
        }

        base.Init_Func(_layer);
    }
}
} // End