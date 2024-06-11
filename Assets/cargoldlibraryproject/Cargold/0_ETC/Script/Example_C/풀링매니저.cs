using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;
namespace Cargold.Example {
public class 풀링매니저 : Cargold.FrameWork.PoolingSystem_Manager
{
    public static new 풀링매니저 Instance;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if(_layer == 0)
        {
            Instance = this;
        }
    }
}
} // End