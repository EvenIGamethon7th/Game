using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

public class LocalizeSystem_Manager : Cargold.FrameWork.LocalizeSystem_Manager
{
    public static new LocalizeSystem_Manager Instance;

    public override void Init_Func(int _layer)
    {
        if(_layer == 0 && Instance == null)
        {
            Instance = this;
        }
    }
}
