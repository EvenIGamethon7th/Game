using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using UnityEngine.EventSystems;

namespace Cargold.UI.Joystick
{
    public class UI_Joystick_Controller_PointerHandler_Script : UI_Joystick_Controller_Script, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        public override ControllerType GetControllerType => ControllerType.PointerHandler;

        void IPointerDownHandler.OnPointerDown(PointerEventData _eventData)
        {
            base.modelClass.OnDragBegin_Func(_eventData.position);
        }
        void IPointerMoveHandler.OnPointerMove(PointerEventData _eventData)
        {
            base.modelClass.OnDrag_Func(_eventData.position);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData _eventData)
        {
            base.modelClass.OnDragEnd_Func();
        }

#if UNITY_EDITOR
        public override void CallEdit_InitByRemocon_Func(UI_Joystick_Model_Script _modelClass)
        {
            base.CallEdit_InitByRemocon_Func(_modelClass);

            if (this.TryGetComponent(out UnityEngine.UI.Graphic _uiGraphic) == false)
            {
                this.gameObject.AddComponent<UnityEngine.UI.Text>();
                RectTransform _rtrf = this.transform as RectTransform;
                _rtrf.SetStretch_Func();
            }
        } 
#endif
    }
}