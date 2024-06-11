using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.UI.Joystick
{
    public abstract class UI_Joystick_Controller_Script : MonoBehaviour
    {
        [BoxGroup(CargoldLibrary_C.Mandatory), LabelText("MVC - Model"), SerializeField] protected UI_Joystick_Model_Script modelClass;

        public abstract ControllerType GetControllerType { get; }

        public virtual void Init_Func()
        {
            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func()
        {

        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        }

#if UNITY_EDITOR
        public virtual void CallEdit_InitByRemocon_Func(UI_Joystick_Model_Script _modelClass)
        {
            this.modelClass = _modelClass;
        } 
#endif

        public enum ControllerType
        {
            None = 0,

            InputSystem,
            PointerHandler,
        }
    }
}