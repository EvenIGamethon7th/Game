using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;
namespace Cargold.Example {
public class 테스트유저데이터 : Cargold.Remocon.Test_UserData_C
{
    protected override UserData_C GetUserOverrideData_Func()
    {
        유저데이터 _userData = new 유저데이터();

        return _userData;
    }
}
} // End