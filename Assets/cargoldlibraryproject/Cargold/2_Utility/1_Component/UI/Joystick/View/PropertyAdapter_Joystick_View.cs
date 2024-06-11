using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.UI.Joystick
{
    public class PropertyAdapter_Joystick_View : Cargold.Example.PropertyAdapter
    {
        [BoxGroup(CargoldLibrary_C.Mandatory), LabelText("그룹 Obj")] public GameObject stickGroupObj = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), LabelText("스틱 Rtrf")] public RectTransform stickRtrf = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), LabelText("배경 Rtrf"), OnValueChanged("CallEdit_BgRtrfChanged_Func")] public RectTransform bgRtrf = null;

        public override string GetLibraryClassType => LibraryRemocon.UtilityClassData.JoystickSystemData.Instance.GetUiClassNameStr_View;
        public float GetRadius => this.bgRtrf.sizeDelta.y * 0.5f;

#if UNITY_EDITOR
        private void CallEdit_BgRtrfChanged_Func()
        {
            if(this.TryGetComponent(out UI_Joystick_Model_Script _modelClass) == true)
            {
                _modelClass.CallEdit_SetRadius_Func(this.GetRadius);
            }
            else
            {
                Debug_C.Error_Func("?");
            }
        }
        public override void CallEdit_AddComponent_Func()
        {
            this.gameObject.AddComponent(System.Type.GetType(LibraryRemocon.UtilityClassData.JoystickSystemData.Instance.GetUiClassNameStr_Model));
            this.gameObject.AddComponent(System.Type.GetType(LibraryRemocon.UtilityClassData.JoystickSystemData.Instance.GetUiClassNameStr_Controller));

            base.CallEdit_AddComponent_Func();
        }
#endif
    }
}