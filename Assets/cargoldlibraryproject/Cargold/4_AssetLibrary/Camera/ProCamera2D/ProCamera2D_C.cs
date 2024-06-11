using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.ExternalAsset.ProCamera2D
{
#if ProCamera2D_C
    using Com.LuisPedroFonseca.ProCamera2D;
    using System;

    [System.Serializable]
    public class ProCamera2D_C
    {
        [SerializeField] protected ProCamera2D mainClass = null;
        [SerializeField] protected ProCamera2DPanAndZoom panAndZoomClass = null;
        [SerializeField] protected ProCamera2DNumericBoundaries numericBoundariesClass = null;
        [ShowInInspector, ReadOnly] private CoroutineData cameraStayCorData;

        protected Camera GetCamera => this.mainClass.GameCamera;
        protected float GetPanArriveDist => .1f;
        public virtual Transform GetPanTrf => throw new System.Exception("ProCamera2DPanAndZoom의 _panTarget을 Public으로 바꾸고 반환 ㄱㄱ");
        public Vector2 GetPanPos => this.GetPanTrf.position;
        protected virtual float GetCameraStayCallbackConditionTime => 1f;

        public void Init_Func()
        {
            this.Deactivate_Func(true);
        }

        public void Activate_Func()
        {

        }

        public float GetScrollBoundary_Func(DirectionType _directionType)
        {
            switch (_directionType)
            {
                case DirectionType.Left:    return this.numericBoundariesClass.LeftBoundary;
                case DirectionType.Bottom:  return this.numericBoundariesClass.BottomBoundary;
                case DirectionType.Top:     return this.numericBoundariesClass.TopBoundary;
                case DirectionType.Right:   return this.numericBoundariesClass.RightBoundary;

                case DirectionType.None:
                default:
                    {
                        Debug_C.Error_Func("_directionType : " + _directionType);
                        return default;
                    }
            }
        }

        public void OnEnableCameraDrag_Func(bool _isOn)
        {
            if (this.panAndZoomClass == null)
                Debug_C.Error_Func("Pan 없는데용?");

            this.panAndZoomClass.AllowPan = _isOn;
        }
        public void SetScrollValue_Func(float _minScrollValue, Vector2 _scrollPower)
        {
            this.panAndZoomClass.MinPanAmount = _minScrollValue;
            this.panAndZoomClass.DragPanSpeedMultiplier = _scrollPower;
        }
        public void SetScrollBoundary_Func(DirectionType _directionType, float _value)
        {
            switch (_directionType)
            {
                case DirectionType.Left:
                    this.numericBoundariesClass.LeftBoundary = _value;
                    break;

                case DirectionType.Bottom:
                    this.numericBoundariesClass.BottomBoundary = _value;
                    break;

                case DirectionType.Top:
                    this.numericBoundariesClass.TopBoundary = _value;
                    break;

                case DirectionType.Right:
                    this.numericBoundariesClass.RightBoundary = _value;
                    break;

                case DirectionType.None:
                default:
                    Debug_C.Error_Func("_directionType : " + _directionType);
                    break;
            }
        }
        [Button]
        public virtual void SetCameraPosImmediately_Func(float _posX)
        {
            this.MoveCameraInstantlyToPositionX_Func(_posX);
        }

        /// <summary>
        /// 카메라를 원하는 장소로 스무스하게 이동
        /// </summary>
        /// <param name="_pos">도착 지점</param>
        /// <param name="_space">좌표계</param>
        /// <param name="_callbackDel">카메라가 10프레임 동안 가만히 있을 시 콜백</param>
        [Button]
        public void SetPanCameraPos_Func(Vector2 _pos, Space _space = Space.World, Action _callbackDel = null)
        {
            this.GetPanTrf.SetPos_Func(_pos, _space);

            if (_callbackDel != null)
                this.cameraStayCorData.StartCoroutine_Func(this.OnCameraStay_Cor(_callbackDel));
        }
        [Button]
        public void SetPanCameraPosX_Func(float _posX, Space _space = Space.World, Action _callbackDel = null)
        {
            this.GetPanTrf.SetPosX_Func(_posX, _space);

            if (_callbackDel != null)
                this.cameraStayCorData.StartCoroutine_Func(this.OnCameraStay_Cor(_callbackDel));
        }
        [Button]
        public void SetPanCameraPosY_Func(float _posY, Space _space = Space.World, Action _callbackDel = null)
        {
            this.GetPanTrf.SetPosY_Func(_posY, _space);

            if (_callbackDel != null)
                this.cameraStayCorData.StartCoroutine_Func(this.OnCameraStay_Cor(_callbackDel));
        }
        private IEnumerator OnCameraStay_Cor(Action _callbackDel)
        {
            float _stayTime = 0f;
            Vector3 _cameraPos = default;
            float _dist = FrameWork.DataBase_Manager.Instance.GetDefine_C.camera_follow_distCon;
            while (_stayTime < GetCameraStayCallbackConditionTime)
            {
                if(_cameraPos.CheckDistance_Func(this.GetCamera.transform.position, _dist) == false)
                {
                    _stayTime = 0f;
                    _cameraPos = this.GetCamera.transform.position;
                }
                else
                {
                    _stayTime += Time.deltaTime;
                }

                yield return null;
            }

            _callbackDel.Invoke();
        }

        [Button]
        public void SetCameraPosByNumbericBoundaryRateX_Func(float _boundaryRateX, bool _isImmediately = false)
        {
            float _maxBoundary = Mathf.Abs(this.numericBoundariesClass.RightBoundary) + Mathf.Abs(this.numericBoundariesClass.LeftBoundary);
            float _cameraMoveRange = _maxBoundary - this.mainClass.ScreenSizeInWorldCoordinates.x;
            float _targetPosX = (_cameraMoveRange * _boundaryRateX) - _cameraMoveRange * .5f;

            this.GetPanTrf.SetPosX_Func(_targetPosX, Space.World);

            if (_isImmediately == true)
                this.MoveCameraInstantlyToPositionX_Func(_targetPosX);
        }

        protected virtual void MoveCameraInstantlyToPositionX_Func(float _targetPosX)
        {
            throw new System.Exception("아래의 코드를 ProCamera2D 클래스 안에 선언 ㄱㄱ");

            //public void MoveCameraInstantlyToPositionX(float _cameraPosX)
            //{
            //    this.MoveCameraInstantlyToPosition(new Vector2(_cameraPosX, _transform.localPosition.y));
            //}
        }

        public void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        }

#if UNITY_EDITOR
        [Button(CargoldLibrary_C.CatchingStr)]
        private void CallEdit_Catching_Func()
        {
            ProCamera2D _proCamera2D = GameObject.FindObjectOfType<ProCamera2D>();
            this.mainClass = _proCamera2D;

            _proCamera2D.TryGetComponent(out this.panAndZoomClass);
            _proCamera2D.TryGetComponent(out this.numericBoundariesClass);
        }
#endif
    }
#endif

    public enum DirectionType
    {
        Left = -2,
        Bottom = -1,
        None = 0,
        Top = 1,
        Right = 2,
    }
}