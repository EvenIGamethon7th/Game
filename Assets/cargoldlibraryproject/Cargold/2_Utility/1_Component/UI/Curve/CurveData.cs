using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.CurveSystem
{
    [System.Serializable]
    public struct CurveData
    {
        [SerializeField] private float curvingTime;     // 시작지점부터 도착지점까지 이동에 걸리는 총 시간
        [SerializeField] private float pushPower;       // 커브 시작 시 밀려나는 힘

        public float CurveTime { get { return curvingTime; } }
        public float PushPower { get { return pushPower; } }

        public CurveData(float _curvingTime, float _pushPower)
        {
            this.curvingTime = _curvingTime;
            this.pushPower = _pushPower;
        }
    } 
}