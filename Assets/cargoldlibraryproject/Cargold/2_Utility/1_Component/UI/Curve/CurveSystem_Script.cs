using Cargold.DataStructure;
using Cargold.FrameWork;
using Cargold.Infinite;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.CurveSystem
{
    public abstract class CurveSystem_Class<ValueType> : MonoBehaviour, ICurveSystem, GameSystem_Manager.IInitializer
    {
        private Transform curvePivotTrf;                                // 커브 시작지점 트랜스폼
        private Transform curvePointTrf;                                // 커브 지점 트랜스폼

        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("커브 도착 최소 시간")] protected float curveTime_min = 1f;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("커브 도착 최대 시간")] protected float curveTime_max = 2f;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("최소 커브 힘")] protected float pushPower_min = 5f;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("최대 커브 힘")] protected float pushPower_max = 10f;

        public float CurveTime_min { get { return this.curveTime_min; } }       // 커브 시작지점에서부터 도착지점까지 걸리는 최소 시간
        public float CurveTime_max { get { return this.curveTime_max; } }       // 커브 시작지점에서부터 도착지점까지 걸리는 최소 시간
        public float PushPower_min { get { return this.pushPower_min; } }       // 커브 시작 시 밀려나는 힘의 최소값
        public float PushPower_max { get { return this.pushPower_max; } }       // 커브 시작 시 밀려나는 힘의 최대값

        private CirculateQueue<CurveData> circulateQueue;               // 선형큐를 활용한 커브 데이터 풀링

        protected virtual int RandNum { get { return 10; } }                      // 커브 데이터 풀링 개수

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                this.circulateQueue = new CirculateQueue<CurveData>();
                for (int i = 0; i < RandNum; i++)
                {
                    CurveData _curveData = this.GetDataByManager_Func();

                    this.circulateQueue.Enqueue_Func(_curveData);
                }

                this.curvePivotTrf = new GameObject().transform;
                this.curvePivotTrf.SetParent(this.transform);
                this.curvePointTrf = new GameObject().transform;
                this.curvePointTrf.SetParent(this.curvePivotTrf);
            }
        }
        public void OnCurve_Func(ValueType _quantity, int _curveNum, Vector2 _arrivePos, Func<ValueType, CurvedClass> _del, float _interval = 0.05f)
        {
            Coroutine_C.StartCoroutine_Func(OnCurve_Cor(_quantity, _curveNum, _arrivePos, _del, _interval));
        }
        private IEnumerator OnCurve_Cor(ValueType _quantity, int _curveNum, Vector2 _arrivePos, Func<ValueType, CurvedClass> _del, float _interval)
        {
            ValueType _eachQuantity = this.GetJustOneValue_Func();

            if (this.IsEqualOrBig_Func(_curveNum, _quantity) == true)
            {
                _eachQuantity = this.GetDivide_Func(_quantity, _curveNum);
            }
            else
            {
                //_curveNum = _quantity;
                _curveNum = 1;
            }

            for (int i = _curveNum - 1; i > 0; i--)
            {
                _quantity = this.GetSubtract_Func(_quantity, _eachQuantity);

                CurvedClass _curveClass = _del(_eachQuantity);
                this.OnCurve_Func(_curveClass, _arrivePos);

                yield return Coroutine_C.GetWaitForSeconds_Cor(_interval);
            }

            CurvedClass _lastCurveClass = _del(_quantity);
            this.OnCurve_Func(_lastCurveClass, _arrivePos);
        }
        public void OnCurve_Func(CurvedClass _curvedClass, Vector2 _arrviePos)
        {
            Transform _curvedTrf = _curvedClass.CurvedTrf;

            this.curvePivotTrf.position = _curvedTrf.position;

            float _curveAngel_Min = _curvedTrf.localEulerAngles.z - _curvedClass.CurveDirectionAngleRange;
            float _curveAngel_Max = _curvedTrf.localEulerAngles.z + _curvedClass.CurveDirectionAngleRange;

            CurveData _curveData = _curvedClass.CurveData;
            Vector3 _curvePos = this.GetCurvePos_Func(_curveData.PushPower, _curveAngel_Min, _curveAngel_Max);

            StartCoroutine(Curve_Cor(_curvedTrf, _curvePos, _arrviePos, _curveData.CurveTime, _curvedClass.IsKeepCurving, _curvedClass.IsLookAtOnCurved, _curvedClass.ArriveCurveDel));
        }
        private IEnumerator Curve_Cor(Transform _curvedTrf, Vector2 _curvePos, Vector2 _arrivePos, float _curveTime, bool _isKeepCurving, bool _isLookAtOnCurved, Action _arriveDel)
        {
            if (_curvedTrf == null) yield break;

            Vector2 _startPos = _curvedTrf.position;

            if (_isKeepCurving == false)
            {
                yield return Coroutine_C.GetWaitForSeconds_Cor(delegate (float _progressTime)
                {
                    float _progressRate = _progressTime / _curveTime;

                    Vector2 _movePos = Math_C.GetBezier_Func(_startPos, _curvePos, _arrivePos, _progressRate);

                    if (_isLookAtOnCurved == true)
                        _curvedTrf.LookAt_Func(_movePos);

                    _curvedTrf.transform.position = _movePos;
                }, _curveTime);
            }
            else
            {
                float _startTime = Time.time;

                while (true)
                {
                    float _progressRate = (Time.time - _startTime) / _curveTime;

                    Vector2 _movePos = Math_C.GetBezier_Func(_startPos, _curvePos, _arrivePos, _progressRate);

                    if (_isLookAtOnCurved == true)
                        _curvedTrf.LookAt_Func(_movePos);

                    _curvedTrf.transform.position = _movePos;

                    yield return null;
                }
            }

            if (_arriveDel != null)
                _arriveDel();
        }
        private Vector3 GetCurvePos_Func(float _pushPower, float _curveAngle_Min = 0f, float _curveAngle_Max = 360f)
        {
            return Cargold_Library.GetCircumferencePos_Func(curvePointTrf.position, _pushPower, UnityEngine.Random.Range(_curveAngle_Min, _curveAngle_Max));
        }
        public CurveData GetCurveData_Func()
        {
#if UNITY_EDITOR
            return this.GetDataByManager_Func();
#endif

            return circulateQueue.Dequeue_Func();
        }

        public CurveData GetDataByManager_Func()
        {
            float _curvingTime = GetDefaultCurvingTime_Func();
            float _pushPower = GetDefaultPushPower_Func();

            CurveData _data = new CurveData(_curvingTime, _pushPower);

            return _data;
        }
        public float GetDefaultPushPower_Func()
        {
            return UnityEngine.Random.Range(this.PushPower_min, this.PushPower_max);
        }
        public float GetDefaultCurvingTime_Func()
        {
            return UnityEngine.Random.Range(this.CurveTime_min, this.CurveTime_max);
        }

        protected abstract ValueType GetJustOneValue_Func();
        protected abstract bool IsEqualOrBig_Func(int _curveNum, ValueType _right);
        protected abstract ValueType GetDivide_Func(ValueType _left, int _right);
        protected abstract ValueType GetSubtract_Func(ValueType _left, ValueType _right);
    }

    public interface ICurveSystem
    {
        CurveData GetCurveData_Func();
    }

    public class CurveSystemInt : CurveSystem_Class<int>
    {
        protected override bool IsEqualOrBig_Func(int _left, int _right)
        {
            return _left <= _right;
        }
        protected override int GetDivide_Func(int _left, int _right)
        {
            return (_left / (float)_right).ToInt();
        }
        protected override int GetSubtract_Func(int _left, int _right)
        {
            return _left - _right;
        }
        protected override int GetJustOneValue_Func()
        {
            return 1;
        }
    }

    public class CurveSystemInfinite : CurveSystem_Class<Cargold.Infinite.Infinite>
    {
        protected override bool IsEqualOrBig_Func(int _left, Cargold.Infinite.Infinite _right)
        {
            return _left <= _right;
        }
        protected override Infinite.Infinite GetDivide_Func(Infinite.Infinite _left, int _right)
        {
            return _left / _right;
        }
        protected override Infinite.Infinite GetSubtract_Func(Infinite.Infinite _left, Infinite.Infinite _right)
        {
            return _left - _right;
        }
        protected override Infinite.Infinite GetJustOneValue_Func()
        {
            return 1;
        }
    }
}