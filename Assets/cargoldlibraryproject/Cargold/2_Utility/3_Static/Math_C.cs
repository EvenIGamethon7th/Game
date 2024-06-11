using System;
using UnityEngine;

namespace Cargold
{
    public static class Math_C
    {
        public static Quaternion GetLookAt_Func(Vector3 _thisPos, Vector3 _targetPos)
        {
            float angle = _thisPos.GetAngle_Func(_targetPos, true);

            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0f, 0f, angle);
            return rotation;
        }

        public static Vector3 GetBezier_Func(Vector3 _startPos, Vector3 _curvePos, Vector3 _arrivePos, float _time)
        {
            var omt = 1f - _time;
            return _startPos * omt * omt + 2f * _curvePos * omt * _time + _arrivePos * _time * _time;
        }
        public static Vector2 GetBezier_Func(Vector2 _startPos, Vector2 _curvePos, Vector2 _arrivePos, float _time)
        {
            var omt = 1f - _time;
            return _startPos * omt * omt + 2f * _curvePos * omt * _time + _arrivePos * _time * _time;
        }

        public static ReturnValue Random_Func<ReturnValue>(params ReturnValue[] _randValueArr)
        {
            return _randValueArr.GetRandItem_Func();
        }

        public static float Case_Func(float _value, float _min, float _max)
        {
            int _valueInt = (int)(_value * 1000f);
            int _minInt = (int)(_min * 1000f);
            int _maxInt = (int)(_max * 1000f);

            int _caseValue = Math_C.Case_Func(_valueInt, _minInt, _maxInt);
            return _caseValue * 0.001f;
        }
        public static int Case_Func(int _value, int _min, int _max)
        {
            int _caseGap = _max - _min;
            int _remainderValue = 0;

            if (0 < _caseGap)
            {
                Math.DivRem(_value, _caseGap + 1, out _remainderValue);

                return 0 <= _remainderValue
                    ? _min + _remainderValue
                    : _max + 1 + _remainderValue;
            }
            else if (_caseGap == 0)
            {
                return _max;
            }
            else
            {
                Debug.LogError("Max value is lower than Min value");
                return -1;
            }
        }

        // 원의 중심에서 _angle에 해당하는 원 둘레의 좌표 얻어오기
        public static Vector2 GetCircumferencePos_Func(Vector2 _circleCenterPos, float _radius)
        {
            return GetCircumferencePos_Func(_circleCenterPos, _radius, UnityEngine.Random.Range(0f, 360f));
        }
        public static Vector2 GetCircumferencePos_Func(Vector2 _circleCenterPos, float _radius, float _angle)
        {
            float _calcAngle = (_angle * -1f + 90f) * Mathf.Deg2Rad;
            float _cos = _radius * Mathf.Cos(_calcAngle);
            float _sin = _radius * Mathf.Sin(_calcAngle);

            return _circleCenterPos += new Vector2(_cos, _sin);
        }

        // 두 좌표 사이에 각도 구하기
        public static float GetAngle_Func(Vector2 _thisPos, Vector2 _targetPos, bool _isRelativeToRotate = false)
        {
            Vector2 _normalTangent = _targetPos - _thisPos;
            _normalTangent.Normalize();

            float angle = Mathf.Atan2(_normalTangent.x, _normalTangent.y) * Mathf.Rad2Deg;

            float _returnAngle = 0f <= angle ? angle : 360f + angle;

            if (_isRelativeToRotate == false)
                return _returnAngle;
            else
                return _returnAngle * -1f;
        }

        /// <summary>
        /// 타켓포스를 바라보는데 필요한 회전 방향
        /// </summary>
        /// <param name="_thisTrf"></param>
        /// <param name="_targetPos"></param>
        /// <returns></returns>
        public static Vector3 GetRotateDir_Func(Transform _thisTrf, Vector2 _targetPos)
        {
            float _thisAngle = _thisTrf.GetAngle_Func();
            float _targetAngle = Math_C.GetAngle_Func(_thisTrf.position, _targetPos);

            _thisAngle += 360f;
            _targetAngle += 360f;

            if (_thisAngle < _targetAngle)
            {
                if (_targetAngle < (_thisAngle + 180f))
                    return Vector3.back;
                else
                    return Vector3.forward;
            }
            else
            {
                if (_targetAngle < (_thisAngle - 180f))
                    return Vector3.back;
                else
                    return Vector3.forward;
            }
        }

        // 시작점에서 목표점까지 직진할 때 먼저 닿는 사각형의 모서리 위치
        public static Vector2 GetEdgePosInSquareArea_Func(Vector2 _startPos, Vector2 _targetPos, Vector2 _areaPosMin, Vector2 _areaPosMax, Vector2 _areaPosCenter)
        {
            _areaPosMin += _areaPosCenter;
            _areaPosMax += _areaPosCenter;

            Vector2 _targetDir = (_targetPos - _startPos).normalized;
            ReachFieldEdgeDir _reachFieldEdgeDir = 0f < _targetDir.x
                ? 0f < _targetDir.y
                    ? ReachFieldEdgeDir.Right_Up
                    : ReachFieldEdgeDir.Right_Down
                : 0f < _targetDir.y
                    ? ReachFieldEdgeDir.Left_Up
                    : ReachFieldEdgeDir.Left_Down;

            float _remainDistanceX = 0f;
            float _remainDistanceY = 0f;
            float _distanceX = 0f;
            float _distanceY = 0f;

            switch (_reachFieldEdgeDir)
            {
                case ReachFieldEdgeDir.Right_Up:
                    _remainDistanceX = _areaPosMax.x - _targetPos.x;
                    _remainDistanceY = _areaPosMax.y - _targetPos.y;
                    break;

                case ReachFieldEdgeDir.Right_Down:
                    _remainDistanceX = _areaPosMax.x - _targetPos.x;
                    _remainDistanceY = _targetPos.y - _areaPosMin.y;
                    break;

                case ReachFieldEdgeDir.Left_Up:
                    _remainDistanceX = _targetPos.x - _areaPosMin.x;
                    _remainDistanceY = _areaPosMax.y - _targetPos.y;
                    break;

                case ReachFieldEdgeDir.Left_Down:
                    _remainDistanceX = _targetPos.x - _areaPosMin.x;
                    _remainDistanceY = _targetPos.y - _areaPosMin.y;
                    break;

                default:
                    Debug_C.Error_Func("_reachFieldEdgeDir : " + _reachFieldEdgeDir);
                    break;
            }

            _remainDistanceX = Mathf.Abs(_remainDistanceX);
            _remainDistanceY = Mathf.Abs(_remainDistanceY);

            _distanceX = _remainDistanceX / _targetDir.x;
            _distanceY = _remainDistanceY / _targetDir.y;

            _distanceX = Mathf.Abs(_distanceX);
            _distanceY = Mathf.Abs(_distanceY);

            bool _isClosePosX = _distanceX < _distanceY;

            switch (_reachFieldEdgeDir)
            {
                case ReachFieldEdgeDir.Right_Up:
                    _targetPos = _isClosePosX == true
                        ? _targetPos = new Vector2(_targetPos.x + _remainDistanceX, _targetPos.y + (_targetDir.y * _distanceX))
                        : _targetPos = new Vector2(_targetPos.x + (_targetDir.x * _distanceY), _targetPos.y + _remainDistanceY);
                    break;

                case ReachFieldEdgeDir.Right_Down:
                    _targetPos = _isClosePosX == true
                        ? _targetPos = new Vector2(_targetPos.x + _remainDistanceX, _targetPos.y + (_targetDir.y * _distanceX))
                        : _targetPos = new Vector2(_targetPos.x + (_targetDir.x * _distanceY), _targetPos.y - _remainDistanceY);
                    break;

                case ReachFieldEdgeDir.Left_Up:
                    _targetPos = _isClosePosX == true
                        ? _targetPos = new Vector2(_targetPos.x - _remainDistanceX, _targetPos.y + (_targetDir.y * _distanceX))
                        : _targetPos = new Vector2(_targetPos.x + (_targetDir.x * _distanceY), _targetPos.y + _remainDistanceY);
                    break;

                case ReachFieldEdgeDir.Left_Down:
                    _targetPos = _isClosePosX == true
                        ? _targetPos = new Vector2(_targetPos.x - _remainDistanceX, _targetPos.y + (_targetDir.y * _distanceX))
                        : _targetPos = new Vector2(_targetPos.x + (_targetDir.x * _distanceY), _targetPos.y - _remainDistanceY);
                    break;

                default:
                    Debug_C.Error_Func("_reachFieldEdgeDir : " + _reachFieldEdgeDir);
                    break;
            }

            return _targetPos;
        }
        private enum ReachFieldEdgeDir
        {
            None = 0,

            Right_Up,
            Right_Down,
            Left_Up,
            Left_Down,
        }

        public static bool CheckDistance_Func(ref Vector2 _leftPos, ref Vector2 _rightPos, ref float _innerDist)
        {
            float xDiff = _leftPos.x - _rightPos.x;
            float yDiff = _leftPos.y - _rightPos.y;

            return (xDiff * xDiff + yDiff * yDiff) <= (_innerDist * _innerDist);
        }
        public static bool CheckDistance_Func(ref Vector3 _leftPos, ref Vector3 _rightPos, ref float _innerDist)
        {
            float xDiff = _leftPos.x - _rightPos.x;
            float yDiff = _leftPos.y - _rightPos.y;
            float zDiff = _leftPos.z - _rightPos.z;

            return (xDiff * xDiff + yDiff * yDiff + zDiff * zDiff) <= (_innerDist * _innerDist);
        }

        /// <summary>
        /// 몫과 나머지를 계산하는 함수
        /// </summary>
        /// <param name="_value">나눠질 값</param>
        /// <param name="_divider">나눌 값</param>
        /// <param name="_quotient">몫</param>
        /// <param name="_remainder">나머지</param>
        public static void GetDiv_Func(float _value, float _divider, out int _quotient, out float _remainder)
        {
            float _result = _value / _divider;

            _quotient = (int)_result;
            _remainder = _value - (_divider * _quotient);
        }
    }
}