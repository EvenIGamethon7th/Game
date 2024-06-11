using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.UI.Joystick
{
    public class UI_Joystick_Controller_InputSystem_Script : UI_Joystick_Controller_Script
    {
        public override ControllerType GetControllerType => ControllerType.InputSystem;

        void Update()
        {
            if (base.modelClass.IsControlable_Func() == true)
            {
                if (Input.GetMouseButtonDown(0) == true)
                {
                    base.modelClass.OnDragBegin_Func(Input.mousePosition);
                }
                else if (Input.GetMouseButton(0) == true)
                {
                    base.modelClass.OnDrag_Func(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0) == true)
                {
                    base.modelClass.OnDragEnd_Func();
                }
            }
        }
    } 
}