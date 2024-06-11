using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
public class UI조이스틱_Model : Cargold.UI.Joystick.UI_Joystick_Model_Script
{
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            base.Init_Func(i);
        }

        base.Activate_Func();
    }

    protected override void OnDragging_Func(Vector2 _vector2)
    {
        base.OnDragging_Func(_vector2);

        // 상 0, 우 90, 하 180, 좌 270
        float _angle = base.GetJoystickAngle_Func();

        Debug_C.Log_Func("각도 : " + _angle + " / 스틱 Vector : " + _vector2 + " / 스틱 Vector Normalize : " + _vector2.normalized);
    }
}
} // End