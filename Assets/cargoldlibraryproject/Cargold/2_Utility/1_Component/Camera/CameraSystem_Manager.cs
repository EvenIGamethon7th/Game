using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;
#if DoTween_C
using DG.Tweening; 
#endif

namespace Cargold
{
    public abstract class CameraSystem_Manager : MonoBehaviour, GameSystem_Manager.IInitializer // C
    {
        public const string FollowStr = "추적";
        public const string ShakeStr = "흔들기";

        public static CameraSystem_Manager Instance;

        [BoxGroup(FollowStr), SerializeField, LabelText("카메라 Trf")] private Transform cameraTrf = null;
        [BoxGroup(FollowStr), SerializeField, ReadOnly, LabelText("기본 타겟")] private Transform followTargetTrf = null;
        [BoxGroup(FollowStr), SerializeField, ReadOnly, LabelText("추가 타겟")] private Transform exFollowTargetTrf = null;
        [BoxGroup(FollowStr), SerializeField, ReadOnly, LabelText("추가 타겟 데이터")] private List<FollowData> followDataList = null;
        [BoxGroup(ShakeStr), SerializeField] private Transform shakeTrf = null;
        [SerializeField] protected Camera thisCamera = null;
        [DetailedInfoBox("설명", "GetScreenPos 함수 사용 시 Canvas Scaler의 Reference Resolution으로 인해 실제 해상도와 캔버스 해상도의 오차를 추적하여 보정하는 기능입니다.")]
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), SerializeField, LabelText("메인 캔버스")] private Canvas mainCanvas = null;
        private CoroutineData calcFollowCorData;
        private CoroutineData followCorData;
        private CoroutineData smoothPushCorData;
        private CoroutineData shakeCorData;
        [BoxGroup(ShakeStr), ShowInInspector, ReadOnly] private float shakeValue;
        [ReadOnly, ShowInInspector] private Vector2 adjustResolutionRate;
        [ReadOnly, ShowInInspector] protected float defaultZoom;

        public Camera GetCamera => this.thisCamera;
        public float GetDefaultZoom => this.defaultZoom;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                if (this.mainCanvas != null)
                {
                    Coroutine_C.Invoke_Func(() =>
                    {
                        Vector2 _initResolution = this.mainCanvas.GetRtrf_Func().sizeDelta;
                        Debug_C.Log_Func("카라리 - 카메라 이니셜라이즈) _initResolution : " + _initResolution + " / Screen : " + new Vector2(Screen.width, Screen.height));
                        this.adjustResolutionRate.x = _initResolution.x / Screen.width;
                        this.adjustResolutionRate.y = _initResolution.y / Screen.height;
                    }, .1f);
                }
                else
                {
                    this.adjustResolutionRate = Vector2.one;
                }

                this.defaultZoom = this.thisCamera.orthographicSize;

                this.Deactivate_Func(true);
            }
        }

        public virtual void Activate_Func()
        {
            this.thisCamera.transform.localPosition = FrameWork.DataBase_Manager.Instance.GetDefine_C.camera_follow_Offset;

            this.followCorData.StartCoroutine_Func(this.OnFollow_Cor());
        }
        private IEnumerator OnFollow_Cor()
        {
            while (true)
            {
                yield return null;

                if (this.smoothPushCorData.IsActivate == true)
                    continue;

                Vector3 _targetPos = default;
                if (this.followDataList.IsHave_Func() == false)
                    _targetPos = this.followTargetTrf.position;
                else
                    _targetPos = this.exFollowTargetTrf.position;

                float _dist = FrameWork.DataBase_Manager.Instance.GetDefine_C.camera_follow_distCon;
                if (this.cameraTrf.position.CheckDistance_Func(_targetPos, _dist) == true)
                    continue;

                float _lerpValue = Time.deltaTime * FrameWork.DataBase_Manager.Instance.GetDefine_C.camera_follow_Speed;
                this.cameraTrf.position = Vector3.Lerp(this.cameraTrf.position, _targetPos, _lerpValue);
            }
        }

        #region Target
        public void SetCameraPos_Func(Vector2 _targetPos)
        {
            this.cameraTrf.position = _targetPos;
        }
        [Button("Target")]
        public void Subscribe_TargetTrf_Func(Transform _targetTrf, float _duration = 0f, Vector3 _offset = default)
        {
#if UNITY_EDITOR
            if (this.followCorData.IsActivate == false)
                Debug_C.Warning_Func("카메라를 활성화 시켜주세요."); 
#endif

            FollowData _followData = new FollowData(_targetTrf, _offset);
            this.followDataList.Add(_followData);

            this.calcFollowCorData.StartCoroutine_Func(OnCalcTarget_Cor(), CoroutineStartType.StartWhenStop);

            if (0f < _duration)
            {
                Coroutine_C.Invoke_Func(() =>
                {
                    this.Unsubscribe_TargetTrf_Func(_targetTrf);
                }, _duration);
            }
        }
        private IEnumerator OnCalcTarget_Cor()
        {
            while (this.followDataList.IsHave_Func() == true)
            {
                Vector3 _calcPos = default;

                for (int i = 0; i < followDataList.Count; i++)
                    _calcPos += this.followDataList[i].trf.position;

                this.exFollowTargetTrf.position = _calcPos / this.followDataList.Count;

                yield return null;
            }

            this.calcFollowCorData.StopCorountine_Func();
        }
        public void Unsubscribe_TargetTrf_Func(Transform _targetTrf)
        {
            bool _isFind = false;

            for (int i = followDataList.Count - 1; i >= 0; i--)
            {
                if (this.followDataList[i].trf == _targetTrf)
                {
                    _isFind = true;

                    this.followDataList.RemoveAt(i);
                    break;
                }
            }

            if (_isFind == false)
                Debug_C.Error_Func("다음 트랜스폼은 카메라 타겟에 없습니다. : " + _targetTrf.GetPath_Func());
        }
        #endregion
        #region Push
        [Button("Push")]
        public void OnPush_Func(Vector2 _pushValue, float _duration = 0f)
        {
            if (0f < _duration)
                this.smoothPushCorData.StartCoroutine_Func(this.OnPushSmoothly_Cor(_pushValue, _duration));
            else
                this.cameraTrf.localPosition += (Vector3)_pushValue;
        }
        private IEnumerator OnPushSmoothly_Cor(Vector3 _pushValue, float _duration = 0f)
        {
            Vector3 _calcPushValue = _pushValue / _duration;

            yield return Coroutine_C.GetWaitForSeconds_Cor((_t) =>
            {
                this.cameraTrf.localPosition += _calcPushValue * Time.deltaTime;
            }, _duration);

            this.smoothPushCorData.StopCorountine_Func();
        }
        #endregion
        #region Shake
        [Button("Shake")]
        public void OnShake_Func(int _shakeID = 0)
        {
            FrameWork.Define_C _define = FrameWork.DataBase_Manager.Instance.GetDefine_C;

            if (_define.shakeDataArr.TryGetItem_Func(_shakeID, out FrameWork.Define_C.ShakeData _shakeData) == true)
            {
                float _power = _shakeData.power;
                float _duration = _shakeData.duratin;
                float _interval = _shakeData.interval;

                this.shakeCorData.StartCoroutine_Func(this.OnShake_Cor(_power, _duration, _interval));
            }
            else
            {
                Debug_C.Warning_Func("다음 Shake Data ID는 없습니다. : " + _shakeID);
            }
        }
        private IEnumerator OnShake_Cor(float _power, float _duration, float _interval)
        {
            this.shakeValue = _power;
            float _cnt = _duration / _interval;
            float _decreaseValue = _power / _cnt;

            while (0f <= this.shakeValue)
            {
                this.shakeTrf.localPosition = Vector_C.GetRand_Func(this.shakeValue, Vector_C.RandMinType.Minus);

                this.shakeValue -= _decreaseValue;

                yield return Coroutine_C.GetWaitForSeconds_Cor(_interval);
            }

            this.shakeTrf.localPosition = Vector3.zero;

            this.shakeCorData.StopCorountine_Func();
        }
        #endregion
#if DoTween_C
        public void OnZoom_Func(float _changedValue, float _duration, bool _isContainDefault = true, System.Action _zommDoneDel = null)
        {
            float _zoomValue = _changedValue + (_isContainDefault == true ? this.defaultZoom : 0f);
            if (0f < _duration)
            {
                if(_zommDoneDel == null)
                    this.thisCamera.DOOrthoSize(_zoomValue, _duration);
                else
                    this.thisCamera.DOOrthoSize(_zoomValue, _duration).OnComplete(()=> _zommDoneDel());
            }
            else
                this.thisCamera.orthographicSize = _zoomValue;
        }
        public void OnZoomDefault_Func(float _duration)
        {
            if (0f < _duration)
                this.thisCamera.DOOrthoSize(this.defaultZoom, _duration);
            else
                this.thisCamera.orthographicSize = this.defaultZoom;
        }
#endif

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                
            }

            this.calcFollowCorData.StopCorountine_Func();
            this.followCorData.StopCorountine_Func();
            this.smoothPushCorData.StopCorountine_Func();
            this.shakeCorData.StopCorountine_Func();
        }

        public Vector2 GetWorldPos_Func(Vector2 _screenPos)
        {
            return this.thisCamera.ScreenToWorldPoint(_screenPos);
        }
        public Vector2 GetScreenPos_Func(Vector2 _worldPos, bool _isAdjustResolution = true)
        {
            Vector3 _screenPos = this.thisCamera.WorldToScreenPoint(_worldPos);

            if (_isAdjustResolution == true)
                _screenPos *= this.adjustResolutionRate;

            return _screenPos;
        }
        public Vector2 GetViewportToScreenPos_Func(Vector2 _viewport)
        {
            return this.thisCamera.ViewportToScreenPoint(_viewport);
        }
        public Vector2 GetViewportToWorldPos_Func(Vector2 _viewport)
        {
            return this.thisCamera.ViewportToWorldPoint(_viewport);
        }
        public Vector2 GetScreenPosToViewport_Func(Vector2 _screenPos)
        {
            return this.thisCamera.ScreenToViewportPoint(_screenPos);
        }
        public Vector2 GetWorldPosToViewport_Func(Vector2 _worldPos)
        {
            return this.thisCamera.WorldToViewportPoint(_worldPos);
        }
        public Ray GetRay_Func(Vector2 _screenPos)
        {
            return this.thisCamera.ScreenPointToRay(_screenPos);
        }
        public Vector2 GetCameraPos_Func()
        {
            return this.thisCamera.transform.position;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            this.CallEdit_Catching_Func();
        }
        [Button("캐싱 ㄱㄱ ~")]
        private void CallEdit_Catching_Func()
        {
            if (this.cameraTrf == null)
                this.cameraTrf = new GameObject("Follow_Pivot").transform;
            this.cameraTrf.SetParent(this.transform);

            if (this.shakeTrf == null)
                this.shakeTrf = new GameObject("Shake_Pivot").transform;
            this.shakeTrf.SetParent(this.cameraTrf);

            if (this.thisCamera == null)
                this.thisCamera = FindObjectOfType<Camera>();
            this.thisCamera.transform.SetParent(this.shakeTrf);

            if (this.followTargetTrf == null)
                this.followTargetTrf = new GameObject("Target_Pivot").transform;
            this.followTargetTrf.SetParent(this.transform);

            if (this.exFollowTargetTrf == null)
                this.exFollowTargetTrf = new GameObject("ExTarget_Pivot").transform;
            this.exFollowTargetTrf.SetParent(this.transform);
        } 
#endif

        [System.Serializable]
        public struct FollowData
        {
            public Transform trf;
            public Vector2 offset;

            public FollowData(Transform _trf, Vector2 _offset)
            {
                this.trf = _trf;
                this.offset = _offset;
            }
        }
    }

    public static partial class Extention_C
    {
        public static void SetScreenPos_Func(this RectTransform _rtrf, Vector2 _worldPos, bool _isAdjustResolution = true)
        {
            if (Cargold.CameraSystem_Manager.Instance == null)
            {
                Debug_C.Warning_Func("카라리 - 카메라 시스템이 없습니다.");
                return;
            }

            Vector2 _screenPos = Cargold.CameraSystem_Manager.Instance.GetScreenPos_Func(_worldPos, _isAdjustResolution);
            _rtrf.anchoredPosition = _screenPos;
        }
    }
}