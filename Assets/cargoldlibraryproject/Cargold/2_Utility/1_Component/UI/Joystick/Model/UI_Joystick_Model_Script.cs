using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.UI.Joystick
{
    public abstract class UI_Joystick_Model_Script : Cargold.UI.UI_Script
    {
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("MVC - View")] protected UI_Joystick_View_Script viewClass;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("자동 이니셜라이즈 여부")] protected bool isAutoInit = true;
        [InfoBox("활성화 시 조이스틱을 조작하지 않아도 UI가 사라지지 않습니다.")]
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("상시 노출 여부")] private bool isAlwaysShow = false;
        [InfoBox("활성화 시 조이스틱의 위치가 고정되어 있습니다.")]
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("위치 고정 여부")] private bool isStaticPos = false;
        [InfoBox("미입력 시 배경 Rtrf의 반지름 크기로 자동 기입됩니다.")]
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("조이스틱 범위")] private float radius = 0f;
        [FoldoutGroup(CargoldLibrary_C.Optional), ShowInInspector, ReadOnly, LabelText("조이스틱 초기 위치")] protected Vector2 stickInitPos = default;
        [FoldoutGroup(CargoldLibrary_C.Optional), ShowInInspector, ReadOnly, LabelText("조작 가능 여부")] protected bool isControlable = true;
        private CoroutineData dragCorData;

        public Vector2 GetStickInitPos => this.stickInitPos;
        public Vector2 GetStickPos => this.viewClass.GetStickPos;

        public override void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                this.viewClass.Init_Func();

                this.isControlable = true;

                if (this.radius <= 0f)
                    this.radius = this.viewClass.GetRadius;
            }

            base.Init_Func(_layer);
        }

        protected override void ActivateDone_Func()
        {
            base.ActivateDone_Func();

            if (this.isAlwaysShow == true)
                this.viewClass.Activate_Func();

            if (this.isStaticPos == true)
                this.stickInitPos = this.viewClass.GetStickPos;
        }

        /// <summary>
        /// 드래그 시작
        /// </summary>
        /// <param name="_inputPos"></param>
        public virtual void OnDragBegin_Func(Vector2 _inputPos)
        {
            if(this.isAlwaysShow == false)
            {
                base.Activate_Func();

                this.viewClass.Activate_Func();
            }

            if(this.isStaticPos == false)
            {
                this.stickInitPos = _inputPos;

                this.viewClass.OnDragBegin_Func(_inputPos);
            }

            this.dragCorData.StartCoroutine_Func(this.OnDrag_Cor());
        }
        private IEnumerator OnDrag_Cor()
        {
            while (true)
            {
                yield return null;

                Vector2 _dir = this.GetJoystickVector_Func();
                this.OnDragging_Func(_dir);
            }
        }
        protected virtual void OnDragging_Func(Vector2 _dir) { }

        /// <summary>
        /// 드래그 중
        /// </summary>
        /// <param name="_inputPos"></param>
        public void OnDrag_Func(Vector2 _inputPos)
        {
            this.OnDrag_Func(_inputPos, out _);
        }

        /// <summary>
        /// 드래그 중
        /// </summary>
        /// <param name="_joyDir">중앙을 기준으로 조이스틱의 Vector</param>
        public void OnDrag_Func(Vector2 _inputPos, out Vector2 _joyDir)
        {
            // 조이스틱 방향 계산
            _joyDir = this.GetInputPosVector_Func(_inputPos);

            this.viewClass.OnDrag_Func(_joyDir + this.stickInitPos);
        }

        /// <summary>
        /// 드래그 끝
        /// </summary>
        public virtual void OnDragEnd_Func()
        {
            this.dragCorData.StopCorountine_Func();

            this.viewClass.OnDragEnd_Func(this.stickInitPos); // 스틱을 원래의 위치로.

            if (this.isAlwaysShow == false)
                this.viewClass.Deactivate_Func();
        }

        /// <summary>
        /// 중앙 기준으로 InputPos의 Normalized Vector
        /// </summary>
        public Vector2 GetInputPosNormalizedVector_Func(Vector2 _inputPos)
        {
            return (_inputPos - this.stickInitPos).normalized;
        }

        /// <summary>
        /// 중앙 기준으로 Stick의 Vector
        /// </summary>
        public Vector2 GetJoystickVector_Func()
        {
            return this.GetInputPosVector_Func(this.viewClass.GetStickPos);
        }
        /// <summary>
        /// 중앙 기준으로 InputPos의 Vector
        /// </summary>
        public Vector2 GetInputPosVector_Func(Vector2 _inputPos)
        {
            Vector2 _joyDir = this.GetInputPosNormalizedVector_Func(_inputPos);

            // 조이스틱의 초기 위치와 현재 내 터치 위치와의 거리를 구한다.
            float _dist = Vector3.Distance(_inputPos, this.stickInitPos);

            // 거리가 반지름보다 작으면...
            if (_dist < this.radius)
                // 방향과 거리를 곱하고 반환
                return _joyDir * _dist;
            else
                // 거리가 반지름보다 크면 방향에 반지름 크기까지만 곱하고 반환
                return _joyDir * this.radius;
        }

        /// <summary>
        /// 중심을 기준으로 Stick의 각도 반환
        /// </summary>
        public float GetJoystickAngle_Func()
        {
            return this.GetInputPosAngle_Func(this.viewClass.GetStickPos);
        }
        /// <summary>
        /// 중심을 기준으로 _inputPos의 각도 반환
        /// </summary>
        public float GetInputPosAngle_Func(Vector2 _inputPos)
        {
            //해당 조이스틱의 각도를 계산
            float _angle = Mathf.Atan2(_inputPos.y - this.stickInitPos.y, _inputPos.x - this.stickInitPos.x) * 180f / Mathf.PI;

            // 0도가 위를 향하게끔 보정
            _angle -= 90f;

            // 음수가 없게끔 보정
            if (_angle < 0) _angle += 360;

            // 시계방향으로 각이 형성되게끔 보정
            _angle = 360f - _angle;
            return _angle;
        }

        public bool IsControlable_Func()
        {
            return this.isControlable == true && base.IsActivate == true || this.isAlwaysShow == false;
        }
        public void SetControlable_Func(bool _isOn)
        {
            this.isControlable = _isOn;
        }
        public void SetInitStickPos_Func(Vector2 _pos)
        {
            this.stickInitPos = _pos;
        }

        protected override bool IsAlwaysShow_Func() => this.isAlwaysShow;

#if UNITY_EDITOR
        public void CallEdit_SetRadius_Func(float _radius)
        {
            this.radius = _radius;
        }
        public void CallEdit_InitByRemocon_Func(UI_Joystick_View_Script _viewClass)
        {
            this.viewClass = _viewClass;
        }
#endif
    }
}