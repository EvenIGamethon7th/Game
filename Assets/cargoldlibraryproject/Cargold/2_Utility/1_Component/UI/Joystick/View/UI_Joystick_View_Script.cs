using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Example;

namespace Cargold.UI.Joystick
{
    public class UI_Joystick_View_Script : MonoBehaviour, Example.IPropertyAdapter
    {
        public const string StickGroupStr = "StickGroup";

        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("MVC - Model")] protected UI_Joystick_Model_Script modelClass = null;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("MVC - Control")] protected UI_Joystick_Controller_Script controlClass = null;
        [SerializeField, HideIf("@propertyAdapterClass != null")] private PropertyAdapter_Joystick_View propertyAdapterClass;

        [ShowInInspector] protected RectTransform stickRtrf { get => propertyAdapterClass.stickRtrf; set => propertyAdapterClass.stickRtrf = value; }
        [ShowInInspector] protected GameObject stickGroupObj { get => propertyAdapterClass.stickGroupObj; set => propertyAdapterClass.stickGroupObj = value; }
        [ShowInInspector] protected RectTransform bgRtrf { get => propertyAdapterClass.bgRtrf; set => propertyAdapterClass.bgRtrf = value; }
        public float GetRadius => this.propertyAdapterClass.GetRadius;

        public Vector2 GetStickPos => this.stickRtrf.position;

        public virtual void Init_Func()
        {
            this.modelClass.SetInitStickPos_Func(this.stickRtrf.transform.position);

            if (this.bgRtrf == null)
                Debug_C.Error_Func("조이스틱) 배경 Rtrf가 없습니다.");

            if (this.stickRtrf == null)
                Debug_C.Error_Func("조이스틱) 스틱 Rtrf가 없습니다.");

            this.controlClass.Init_Func();

            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func()
        {
            this.stickGroupObj.SetActive(true);

            this.controlClass.Activate_Func();
        }
        public virtual void OnDragBegin_Func(Vector2 _inputPos)
        {
            this.stickGroupObj.transform.position = _inputPos;
        }
        public void OnDrag_Func(Vector2 _stickPos)
        {
            this.stickRtrf.position = _stickPos;
        }
        public void OnDragEnd_Func(Vector2 _initPos)
        {
            this.stickRtrf.position = _initPos; // 스틱을 원래의 위치로.
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.stickGroupObj.SetActive(false);
        }

#if UNITY_EDITOR
        void IPropertyAdapter.CallEdit_AddComponent_Func<T>(T _exampleData)
        {
            this.propertyAdapterClass = _exampleData as PropertyAdapter_Joystick_View;

            if(this.TryGetComponent(out UI_Joystick_Model_Script _modelClass) == true)
            {
                this.modelClass = _modelClass;
                _modelClass.CallEdit_InitByRemocon_Func(this);
            }
            else
            {
                Debug_C.Error_Func("조이스틱 Model 없음");
            }

            if (this.TryGetComponent(out UI_Joystick_Controller_Script _controllerClass) == true)
            {
                this.controlClass = _controllerClass;
                _controllerClass.CallEdit_InitByRemocon_Func(_modelClass);
            }
            else
            {
                Debug_C.Error_Func("조이스틱 Controller 없음");
            }
        } 
#endif
    } 
}