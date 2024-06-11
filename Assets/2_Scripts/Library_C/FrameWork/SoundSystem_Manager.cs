using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

public class SoundSystem_Manager : Cargold.FrameWork.SoundSystem_Manager
{
    public static new SoundSystem_Manager Instance;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if(_layer == 0)
        {
            Instance = this;
        }
    }
}
