using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
public class 사운드시스템매니저 : Cargold.FrameWork.SoundSystem_Manager
{
    public static new 사운드시스템매니저 Instance;

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