using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;

public class Test_UserData : Cargold.Remocon.Test_UserData_C
{
    protected override UserData_C GetUserOverrideData_Func()
    {
        UserData _userData = new UserData();

        return _userData;
    }
}
