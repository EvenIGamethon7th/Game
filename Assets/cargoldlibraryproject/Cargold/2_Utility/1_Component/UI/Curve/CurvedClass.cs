using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.CurveSystem
{
    public class CurvedClass
    {
        private Transform curvedTrf;           // 커브할 트랜스폼
        private Action arriveCurveDel;         // 커브의 도착지점에 다다른 후 호출할 함수
        [SerializeField] private bool isLookAtOnCurved = false;           // 커브할 방향을 바라볼 것인가?
        [SerializeField] private bool isKeepCurving = false;              // 커브의 도착 이후에도 계속 커브하며 이동할 것인가?
        [SerializeField] private float curveDirectionAngleRange = 0f;  // 커브 시작각의 범위 (현재 로테이션 z축을 기준으로 랜덤하게 각이 변할 편차값)
        private CurveData curveData;

        public float CurveDirectionAngleRange => this.curveDirectionAngleRange;

        public Transform CurvedTrf { get { return curvedTrf; } }
        public Action ArriveCurveDel { get { return arriveCurveDel; } }
        public bool IsLookAtOnCurved { get { return isLookAtOnCurved; } }
        public bool IsKeepCurving { get { return isKeepCurving; } }
        public CurveData CurveData { get { return curveData; } }

        public CurvedClass(Transform _curvedTrf, Action _arriveCurveDel, ICurveSystem _iCurveSystem)
        {
            this.curvedTrf = _curvedTrf;
            this.arriveCurveDel = _arriveCurveDel;

            if (curveDirectionAngleRange == 0f)
                this.curveDirectionAngleRange = 180f;

            this.curveData = _iCurveSystem.GetCurveData_Func();
        }
        public CurvedClass(Transform _curvedTrf, Action _arriveCurveDel, CurveData _curveData)
        {
            this.curvedTrf = _curvedTrf;
            this.arriveCurveDel = _arriveCurveDel;

            if (curveDirectionAngleRange == 0f)
                this.curveDirectionAngleRange = 180f;

            this.curveData = _curveData;
        }

        public void SetCurveData_Func(CurveData _curveData)
        {
            this.curveData = _curveData;
        }
    } 
}