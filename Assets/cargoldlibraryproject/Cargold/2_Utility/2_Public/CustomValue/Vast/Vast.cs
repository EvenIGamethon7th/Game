using Cargold.Infinite;
using Cargold.Vast;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 1.0.0 ('20.03.30)
// Developed By Cargold
namespace Cargold.Vast
{
    [System.Serializable]
    public sealed class Vast : IDisposable, IEquatable<Vast>, IEqualityComparer<Vast>
    {
        #region Variable
		private static StringBuilder StringBuilder = new StringBuilder(128);
        private static decimal Thousand = new decimal(1000);

        [SerializeField] private int[] values;
		private bool isDispose = false;

        public int CurrentDigit
        {
            get
            {
                for (int _digit = values.Length - 1; 0 <= _digit ; _digit--)
                {
                    if (0 < values[_digit])
                        return _digit;
                }

                return -1;
            }
        }
        public bool IsMaxValue
        {
            get
            {
                for (int i = this.values.Length - 1; 0 <= i; i--)
                {
                    if (this.values[i] < 999)
                        return false;
                }

                return true;
            }
        }
        #endregion

        #region Constructor & Destructor
        public Vast()
        {
            this.Initialize();
        }
        public Vast(params int[] values)
        {
            this.Initialize();

            this.SetValue(false, values);
        }
        //public Vast(params byte[] values)
        //{
        //    this.Initialize();

        //    this.SetValue(false, values);
        //}
        public Vast(int[] values, float pointValue, bool isQuickCalculating = false)
        {
            this.Initialize();

            this.SetValue(false, values, pointValue, isQuickCalculating);
        }
        public Vast(float value, bool isQuickCalculating = false)
        {
            this.Initialize();

            this.SetValue(false, value, isQuickCalculating);
        }
        public Vast(Vast value)
        {
            this.Initialize();

            this.SetValue(true, value);
        }
        public Vast(string value)
        {
            this.Initialize();

            Vast _castingValue = value.ToVast();

            this.SetValue(false, _castingValue);
        }
        ~Vast()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying == false)
                return;
#endif
            if (isDispose)
                return;

            VastSystemManager.Despawn(this);
            isDispose = true;
        }
        #endregion
        #region Implicit & Explicit Casting
        public static implicit operator Vast(int[] values)
        {
            Vast _vast = VastSystemManager.Spawn();

            _vast.SetValue(false, values);

            return _vast;
        }
        public static implicit operator Vast(List<int> value)
        {
            Vast _vast = VastSystemManager.Spawn();

            _vast.SetValue(false, value.ToArray());

            return _vast;
        }
        public static implicit operator Vast(int value)
        {
            Vast _vast = VastSystemManager.Spawn();
            _vast.SetValue(false, value);
            return _vast;
        }
        public static implicit operator Vast(float value)
        {
            Vast _vast = VastSystemManager.Spawn();

            _vast.SetValue(false, value, true);

            return _vast;
        }
        public static implicit operator string(Vast value)
        {
            return value.GetString();
        }

        public static explicit operator int(Vast value)
        {
            return value.GetInt();
        }
        public static explicit operator float(Vast value)
        {
            return value.GetFloat();
        }
        public static explicit operator Vast(string value)
        {
            if(value.Contains(",") == true)
                return Vast.ToVast(value, true);
            else
                return Vast.ToVast(value, false);
        }
        #endregion

        #region Calculate Group
        public static Vast operator +(Vast left, Vast right)
        {
            return Vast.Addition(left, right, true);
        }
        public static Vast operator +(Vast left, int right)
        {
            Vast _result = VastSystemManager.Spawn(left);
            
            Vast.Addition(_result, right, 1);
            
            return _result;
        }
        public static Vast operator +(Vast left, float right)
        {
            Vast _result = VastSystemManager.Spawn(left);

            int _quotientValue, _reminderValue;
            Vast.ExtractPointValue(right, out _quotientValue, out _reminderValue, true);

            _result = Vast.Addition(_result, _reminderValue, 0);
            _result = Vast.Addition(_result, _quotientValue, 1);

            return _result;
        }
        public static Vast Addition(Vast left, Vast right, bool isQuickCalculating = false)
        {
            Vast _result = VastSystemManager.Spawn(left);

            int _rightCurrentDigit = right.CurrentDigit;

            int _accuracy = isQuickCalculating == true
                    ? VastSystemManager.Instance.DefaultDigitOperationAccuracy
                    : _rightCurrentDigit;

            // 마지막 Digit부터 첫번째 Digit까지 역순으로 계산
            for (int _calcDigit = _rightCurrentDigit; 0 <= _calcDigit && 0 <= _accuracy; _calcDigit--, _accuracy--)
            {
                int _rightValue = right.GetValue(_calcDigit);
                _result = Vast.Addition(_result, _rightValue, _calcDigit);
            }

            return _result;
        }

        private static Vast Addition(Vast resultVast, int value, int digit)
        {
            if (digit < resultVast.values.Length)
            {
                if (0 <= digit)
                {
                    if (1000 <= value)
                    {
                        int _quotientValue = value / 1000;
                        Vast.Addition(resultVast, _quotientValue, digit + 1);

                        value = value - (_quotientValue * 1000); // 나머지
                    }
                }
                else if (digit == -1)
                {
                    if (1000 <= value)
                    {
                        digit = 0;

                        value = value / 1000;

                        // _isRoundOff가 True이면, Addition 함수를 호출하는 쪽에서 최종적으로 반올림을 해줘야 한다.
                        //int _quotientValue = value / 1000;
                        //value = _quotientValue;
                        //int _reminderValue = value - (_quotientValue * 1000);
                        //bool _isRoundOff = 500 <= _reminderValue;
                    }
                    else if (500 <= value)
                    {
                        // 반올림

                        digit = 0;

                        value = 1;
                    }
                    else
                    {
                        return resultVast;
                    }
                }
                else
                {
                    return resultVast;
                }

                resultVast.values[digit] += value;

                // Digit의 덧셈값이 1천 이상인가?
                if (1000 <= resultVast.values[digit])
                {
                    int _quotientValue = resultVast.values[digit] / 1000;

                    resultVast.values[digit] %= 1000;

                    Vast.Addition(resultVast, _quotientValue, digit + 1);
                }

                // Digit의 덧셈값이 1천 미만인가?
                else
                {

                }
            }
            else
            {
                resultVast.SetValueFull();
            }
            
            return resultVast;
        }
        
        public static Vast operator -(Vast left, Vast right)
        {
            return Subtraction(left, right, true);
        }
        public static Vast operator -(Vast left, int right)
        {
            Vast _result = VastSystemManager.Spawn(left);

            bool _isClearedValue = false;
            _result = Vast.Subtraction(_result, right, 1, out _isClearedValue);
            
            return _result;
        }
        public static Vast operator -(Vast left, float right)
        {
            Vast _result = VastSystemManager.Spawn(left);

            int _quotientValue, _reminderValue;
            Vast.ExtractPointValue(right, out _quotientValue, out _reminderValue, true);

            bool _isClearedValue;
            _result = Vast.Subtraction(_result, _reminderValue, 0, out _isClearedValue);

            if(_isClearedValue == false)
                _result = Vast.Subtraction(_result, _quotientValue, 1, out _isClearedValue);

            return _result;
        }
        public static Vast Subtraction(Vast left, Vast right, bool isQuickCalculating = false)
        {
            Vast _result = VastSystemManager.Spawn();

            int _leftCurrentDigit = left.CurrentDigit;
            int _rightCurrentDigit = right.CurrentDigit;
            bool _isLeftBigger = false;

            // left의 Digit가 더 큰가?
            if (_rightCurrentDigit < _leftCurrentDigit)
            {
                _isLeftBigger = true;
            }
            // right의 Digit가 더 큰가?
            else if (_leftCurrentDigit < _rightCurrentDigit)
            {
                _isLeftBigger = false;
            }
            // left, right의 Digit가 동일한가?
            else
            {
                _isLeftBigger = right < left;
            }

            if (_isLeftBigger == true)
            {
                _result.SetValue(false, left);

                int _accuracy = isQuickCalculating == true
                    ? VastSystemManager.Instance.DefaultDigitOperationAccuracy
                    : _rightCurrentDigit;

                // 마지막 Digit부터 첫번째 Digit까지 역순으로 계산
                for (int _calcDigit = _rightCurrentDigit; 0 <= _calcDigit && 0 <= _accuracy; _calcDigit--, _accuracy--)
                {
                    int _calcValue = right.GetValue(_calcDigit);
                    bool _isClearedValue = false;
                    _result = Vast.Subtraction(_result, _calcValue, _calcDigit, out _isClearedValue, _leftCurrentDigit);

                    if (_isClearedValue == true)
                        break;
                }
            }

            return _result;
        }
        private static Vast Subtraction(Vast resultVast, int value, int digit, out bool isClearedValue, int currentDigit = -1)
        {
            // Digit 내부값이 뺄셈값보다 많은가?
            if (value <= resultVast.values[digit])
            {
                // Digit 내부값 빼기
                resultVast.values[digit] -= value;

                isClearedValue = false;
            }

            // Digit에 남은 값이 뺄셈값보다 작은가?
            else if (resultVast.values[digit] < value)
            {
                if (currentDigit == -1)
                    currentDigit = resultVast.CurrentDigit;

                // 더 높은 Digit에 값이 있는가?
                if (digit < currentDigit)
                {
                    isClearedValue = false;

                    // 값이 존재하는 Digit를 찾아서 값 가져오기
                    for (int i = digit + 1; i <= currentDigit; i++)
                    {
                        // Digit에 값이 0보다 많다면
                        if (0 < resultVast.values[i])
                        {
                            // 해당 Digit에 값을 1 낮춤
                            resultVast.values[i]--;

                            break;
                        }

                        // Digit에 값이 0 이라면
                        else if (resultVast.values[i] == 0)
                        {
                            // 내림으로 인해 999값 삽입

                            resultVast.values[i] = 999;
                        }

                        // Digit에 값이 0보다 작다면
                        else if (resultVast.values[i] < 0)
                        {
#if UNITY_EDITOR
                            Debug.LogError(i + "번째 Digit에 '" + resultVast.values[i] + "' 마이너스 값이 존재합니다.");
#endif
                        }
                    }

                    // (상위 Digit 내림값 + 기존값)과 뺄셈값을 계산한 뒤 해당 Digit에 삽입
                    resultVast.values[digit] = (1000 + resultVast.values[digit]) - value;
                }

                // 더 높은 Digit에 값이 없는가?
                else
                {
                    // 현재 보유값보다 뺄셈값이 더 큼. 하지만 음수는 허용하지 않으므로 0으로 초기화

                    isClearedValue = true;

                    resultVast.Clear();
                }
            }
            else
            {
                isClearedValue = false;

#if UNITY_EDITOR
                Debug.LogError("이 경우는 발생할 수 없음");
#endif
            }

            return resultVast;
        }

        public static Vast operator *(Vast left, Vast right)
        {
            return Vast.Multiplication(left, right, true);
        }
        public static Vast operator *(Vast left, int right)
        {
            if(0 < right)
            {
                return Vast.Multiplication(left, right, true);
            }
            else
            {
                return VastSystemManager.Spawn();
            }
        }
        public static Vast Multiplication(Vast originVast, Vast calcVast, bool isQuickCalculating = false)
        {
            Vast _resultVast = VastSystemManager.Spawn();

            int _originVastCurrentDigit = originVast.CurrentDigit;
            int _calcVastCurrentDigit = calcVast.CurrentDigit;

            if (_originVastCurrentDigit + _calcVastCurrentDigit - 1 < _resultVast.values.Length)
            {
                int _operateDigit = 0;
                if (isQuickCalculating == true)
                {
                    int _accuracy = VastSystemManager.Instance.DefaultDigitOperationAccuracy;
                    _operateDigit = _accuracy < _calcVastCurrentDigit
                        ? _calcVastCurrentDigit - _accuracy
                        : 0;
                }

                for (int _calcDigit = _operateDigit; _calcDigit <= _calcVastCurrentDigit; _calcDigit++)
                {
                    int _calcValue = calcVast.GetValue(_calcDigit);
                    Vast.Multiplication(_resultVast, originVast, _originVastCurrentDigit, _calcDigit, _calcValue, isQuickCalculating);
                }
            }
            else
            {
                _resultVast.SetValueFull();
            }

            return _resultVast;
        }
        private static void Multiplication(Vast resultVast, Vast originVast, int originVastCurrentDigit, int calcDigit, int calcValue, bool isQuickCalculating)
        {
            int _operateDigit = 0;
            if (isQuickCalculating == true)
            {
                int _accuracy = VastSystemManager.Instance.DefaultDigitOperationAccuracy;
                _operateDigit = _accuracy < originVastCurrentDigit
                    ? originVastCurrentDigit - _accuracy
                    : 0;
            }

            for (int _digit = _operateDigit; _digit <= originVastCurrentDigit; _digit++)
            {
                int _originValue = originVast.GetValue(_digit);
                if(0 < _originValue)
                {
                    int _multipledValue = _originValue * calcValue;

                    int _multipledDigit = _digit + (calcDigit - 1);

                    resultVast = Vast.Addition(resultVast, _multipledValue, _multipledDigit);
                }
            }
        }
        private void Multiplication_Digit_Func(int multipleDigit)
        {
            if (0 < multipleDigit)
            {
                Vast _originVast = VastSystemManager.Spawn(this);
                int _currentDigit = _originVast.CurrentDigit;

                for (int i = _currentDigit - 1; 0 <= i; i--)
                {
                    if (i + multipleDigit < this.values.Length)
                    {
                        this.values[i + multipleDigit] = _originVast.values[i];
                    }
                    else
                    {
                        this.SetValueFull();
                        break;
                    }
                }

                _originVast.Dispose();
            }
            else
            {

            }
        }

        public static Vast operator /(Vast left, Vast right)
        {
            return Vast.Division(left, right, true);
        }
        public static Vast operator /(Vast left, int right)
        {
            if (0 < right)
                return Vast.Division(left, left.CurrentDigit, 3, right, true);
            else
                return VastSystemManager.Spawn();
        }
        public static Vast Division(Vast originVast, Vast calcVast, bool isQuickCalculating = false)
        {
            int _calcVastCurrentDigit = calcVast.CurrentDigit;
            if(0 <= _calcVastCurrentDigit)
            {
                int _originVastCurrentDigit = originVast.CurrentDigit;
                
                int _divideValue = calcVast.GetInt(_calcVastCurrentDigit - 2); // 나눌 값 가져오기

                return Vast.Division(originVast, _originVastCurrentDigit, _calcVastCurrentDigit, _divideValue, isQuickCalculating);
            }
            else
            {
                return VastSystemManager.Spawn();
            }
        }
        private static Vast Division(Vast originVast, int originDigit, int calcDigit, int divideValue, bool isQuickCalculating)
        {
            Vast _resultVast = VastSystemManager.Spawn();

            int _operateDigit = 0;
            if (isQuickCalculating == true)
            {
                int _accuracy = VastSystemManager.Instance.DefaultDigitOperationAccuracy;
                _operateDigit = _accuracy < originDigit
                    ? originDigit - _accuracy
                    : 0;
            }

            long _remainValue = 0;
            for (int _digit = originDigit; _operateDigit <= _digit; _digit--)
            {
                int _dividedDigit = _digit - calcDigit;
                if (2 <= calcDigit) _dividedDigit++;
                else if (calcDigit <= 0) _dividedDigit--;

                if (_dividedDigit < -1) break;

                long _originValue = originVast.GetValue(_digit);

                for (int j = 0; j < 2; j++)
                    _originValue *= 1000;

                _originValue += _remainValue;
                _remainValue = 0;

                long _quotientValue = 0 < divideValue ? _originValue / divideValue : _originValue;

                // 의역 : 허용 가능한 소수점인가? (직역 : 0 이상의 디지트인가?)
                if (-1 <= _dividedDigit)
                {
                    if (0 < _quotientValue)
                        _resultVast = Vast.Addition(_resultVast, (int)_quotientValue, _dividedDigit);

                    long _reminderValue = _originValue - (divideValue * _quotientValue);

                    if (0f < _reminderValue)
                        _remainValue = _reminderValue * 1000;
                }
            }

            return _resultVast;
        }
        #endregion
        #region Compare Group
        public static bool operator ==(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;
            
            for (int i = 0; i < left.values.Length; i++)
                if (left.values[i] != right.values[i]) return false;

            return true;
        }
        public static bool operator !=(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            for (int i = left.values.Length - 1; 0 <= i; i--)
            {
                if (left.values[i] <= 0 && right.values[i] <= 0) continue;
                if (left.values[i] != right.values[i]) return true;
            }

            return false;
        }
        public static bool operator <(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            for (int i = left.values.Length - 1; 0 <= i; i--)
            {
                if (left.values[i] <= 0 && right.values[i] <= 0) continue;
                if (left.values[i] == right.values[i]) continue;

                if (left.values[i] < right.values[i]) return true;
                return false;
            }

            return false;
        }
        public static bool operator >(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            for (int i = left.values.Length - 1; 0 <= i; i--)
            {
                if (left.values[i] <= 0 && right.values[i] <= 0) continue;
                if (left.values[i] == right.values[i]) continue;

                if (left.values[i] > right.values[i]) return true;
                return false;
            }

            return false;
        }
        public static bool operator <=(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            for (int i = left.values.Length - 1; 0 <= i; i--)
            {
                if (left.values[i] <= 0 && right.values[i] <= 0) continue;
                if (left.values[i] == right.values[i]) continue;

                if (left.values[i] <= right.values[i]) return true;
                else return false;
            }

            return true;
        }
        public static bool operator >=(Vast left, Vast right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            for (int i = left.values.Length - 1; 0 <= i; i--)
            {
                if (left.values[i] <= 0 && right.values[i] <= 0) continue;
                if (left.values[i] == right.values[i]) continue;

                if (left.values[i] >= right.values[i]) return true;
                else return false;
            }

            return true;
        }
        #endregion

        #region Private Method
        private void Initialize()
        {
            this.values = new int[VastSystemManager.Instance.DigitSize + 2];
			isDispose = false;
        }
        private int GetInt(int _startDigit = 1)
        {
            if (_startDigit < 0) _startDigit = 0; // 방어 코드

            int _returnValue = 0;

            bool _isMaxValue = Vast.IsBiggerThanMaxInt(this, _startDigit);
            if (_isMaxValue == false)
            {
                for (int _digit = _startDigit; _digit < (_startDigit + 4) && _digit < VastSystemManager.Instance.DigitSize; _digit++)
                {
                    int _addValue = this.values[_digit];
                    if(0 < _addValue)
                    {
                        for (int _multipleCnt = _startDigit; _multipleCnt < _digit; _multipleCnt++)
                        {
                            _addValue *= 1000;
                        }

                        _returnValue += _addValue;
                    }
                }

                return _returnValue;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Int Max 값을 초과합니다");
#endif

                return int.MaxValue;
            }
        }
        private float GetFloat()
        {
            float _returnValue = 0f;

            _returnValue += this.values[0] * 0.001f;
            _returnValue += this.values[1];
            _returnValue += this.values[2] * 1000f;

            return _returnValue;
        }
        private string GetString(int currentDigit = -1, int point = -1)
        {
            if (currentDigit == -1)
                currentDigit = this.CurrentDigit;

            if (point == -1)
                point = VastSystemManager.Instance.DefaultPoint;

            if (0 <= currentDigit)
            {
                Vast.StringBuilder.Clear();

                int _digitValue = this.GetValue(currentDigit);
                Vast.StringBuilder.Append(_digitValue);

                if(0 < point)
                {
                    int _pointValue = this.GetValue(currentDigit - 1);
                    if (0 < _pointValue)
                    {
                        string _pointString = "";
                        _pointString = GetPointString(_pointValue, point);
                        Vast.StringBuilder.Append(".").Append(_pointString);
                    }
                }

                if (2 <= currentDigit)
                {
                    string _digitUnit = VastSystemManager.Instance.DigitUnits[currentDigit - 2];
                    Vast.StringBuilder.Append(_digitUnit);
                }

                return Vast.StringBuilder.ToString();
            }
            else if(0 == currentDigit)
            {
                return (this.GetValue(currentDigit) * 0.001f).ToString();
            }
            else
            {
                // 현 Vast는 비어있음

                return "0";
            }
        }
        private string GetStringFull(int point = -1, bool isHaveComma = true, int currentDigit = -1)
        {
            Vast.StringBuilder.Clear();

            if (point == -1)
                point = VastSystemManager.Instance.DefaultPoint;
            
            if(currentDigit == -1)
                currentDigit = this.CurrentDigit;

            if (0 < currentDigit)
            {
                int _digitValue = this.GetValue(currentDigit);
                Vast.StringBuilder.Append(_digitValue.ToString());

                currentDigit--;
            }
            else
                Vast.StringBuilder.Append("0");

            for (; 0 <= currentDigit; currentDigit--)
            {
                // 정수값
                if (0 < currentDigit)
                {
                    int _digitValue = this.GetValue(currentDigit);

                    if (isHaveComma == true)
                    {
                        Vast.StringBuilder.AppendFormat(",{0:000}", _digitValue);
                    }
                    else
                    {
                        Vast.StringBuilder.AppendFormat("{0:000}", _digitValue);
                    }
                }

                // 실수값
                else
                {
                    if (0 < point)
                    {
                        int _pointValue = this.GetValue(currentDigit);

                        if (0 < _pointValue)
                        {
                            string _pointString = "";
                            _pointString = GetPointString(_pointValue, point);
                            Vast.StringBuilder.Append(".").Append(_pointString);
                        }
                    }
                }
            }

            return Vast.StringBuilder.ToString();
        }
        private string GetStringLong(int longDigit = -1, int longValue = -1, int point = -1, bool isHaveComma = true, int currentDigit = -1)
        {
            if (longDigit == -1)
                longDigit = VastSystemManager.Instance.LongDataValue.longDigit;

            if (longValue == -1)
                longValue = VastSystemManager.Instance.LongDataValue.longValue;

            if (currentDigit == -1)
                currentDigit = this.CurrentDigit;

            if (currentDigit < longDigit)
                return this.GetStringFull(point, isHaveComma, currentDigit);
            else if(longDigit < currentDigit)
                return this.GetString(currentDigit);
            else
            {
                int _digitValue = this.GetValue(currentDigit);

                if (_digitValue <= longValue)
                    return this.GetStringFull(point, isHaveComma, currentDigit);
                else
                    return this.GetString(currentDigit);
            }
        }
        private void SetValue(bool isClear, int[] values, float pointValue, bool isQuickCalculating)
        {
            if (isClear == true) this.Clear();

            if (values.Length <= this.values.Length)
            {
                this.SetValue(false, pointValue, isQuickCalculating);
                this.SetValue(false, values);
            }
            else
            {
                this.SetValueFull();
            }
        }
        private void SetValue(bool isClear, float pointValue, bool isQuickCalculating)
        {
            if (isClear == true) this.Clear();

            int _quotientValue = 0;
            int _reminderValue = 0;
            Vast.ExtractPointValue(pointValue, out _quotientValue, out _reminderValue, isQuickCalculating);
            
            Vast.Addition(this, _reminderValue, 0);
            Vast.Addition(this, _quotientValue, 1);
        }
        private void SetValue(bool isClear, params int[] values)
        {
            if (isClear == true) this.Clear();

            if (values.Length <= this.values.Length)
            {
                for (int _digit = 1; _digit <= values.Length; _digit++)
                    Vast.Addition(this, values[values.Length - _digit], _digit);
            }
            else
            {
                this.SetValueFull();
            }
        }
        private void SetValue(bool isClear, Vast value)
        {
            if(isClear == true) this.Clear();

            for (int i = 0; i < this.values.Length; i++)
                this.values[i] = value.values[i];
        }

        private static void ExtractPointValue(float pointValue, out int _quotientValue, out int _reminderValue, bool isQuickCalculating)
        {
            if(isQuickCalculating == true)
            {
                _quotientValue = (int)pointValue;

                float _value = pointValue - _quotientValue;
                _reminderValue = (int)(_value * 1000f);
            }
            else
            {
                decimal _pointValueDecimal = new Decimal(pointValue);

                decimal _quotientValueDecimal = Decimal.Truncate(_pointValueDecimal);
                _quotientValue = Decimal.ToInt32(_quotientValueDecimal);

                decimal _reminderValueDecimal = Decimal.Multiply(_pointValueDecimal - _quotientValueDecimal, Vast.Thousand);
                _reminderValue = Decimal.ToInt32(_reminderValueDecimal);
            }
        }
        private static bool IsBiggerThanMaxInt(Vast vast, int digitPivot = 1)
        {
            VastSystemManager.MaxIntVast.SetValue(int.MaxValue, digitPivot, false);

            return VastSystemManager.MaxIntVast <= vast;
        }
        private static string GetPointString(int pointValue, int point = -1)
        {
            if (point == -1)
                point = VastSystemManager.Instance.DefaultPoint;

            if (point == 1)
            {
                float _temp = pointValue * 0.01f;
                pointValue = (int)_temp;

                if (0 < pointValue)
                {
                    return pointValue.ToString();
                }
                else
                {
                    return "0";
                }
            }
            else if (point == 2)
            {
                float _temp = pointValue * 0.1f;
                pointValue = (int)_temp;

                if (10 <= pointValue)
                {
                    return pointValue.ToString();
                }
                else if (0 < pointValue)
                {
                    return "0" + pointValue.ToString();
                }
                else
                {
                    return "00";
                }
            }
            else
            {
#if UNITY_EDITOR
                if (3 < point)
                    Debug.LogWarning("소수점(Point)은 1~3을 넘길 수 없습니다. / Point : " + point);
#endif

                if (100 <= pointValue)
                {
                    return pointValue.ToString();
                }
                else if (10 <= pointValue)
                {
                    return "0" + pointValue.ToString();
                }
                else if (0 < pointValue)
                {
                    return "00" + pointValue.ToString();
                }
                else
                {
                    return "000";
                }
            }
        }
        private static int GetInt(string value)
        {
            return string.IsNullOrEmpty(value) == false
                ? System.Int32.Parse(value)
                : 0;
        }
        private static Vast GetVast(string value)
        {
            Vast _vast = VastSystemManager.Spawn();
            int _digit = 1;
            
            bool _isRemainValue = true;
            while (_isRemainValue == true)
            {
                int _pivotCalc = value.Length - (_digit * 3);

                int _pivot = 0;
                if (0 <= _pivotCalc)
                {
                    _pivot = _pivotCalc;
                }
                else
                {
                    _isRemainValue = false;
                    _pivot = 3 + _pivotCalc;
                }

                string _digitValueStr = "";
                for (int i = 0; i < 3; i++)
                {
                    if (_isRemainValue == true)
                        _digitValueStr += value[_pivot + i];
                    else
                    {
                        if (i < _pivot)
                            _digitValueStr += value[i];
                        else
                            break;
                    }
                }

                int _value = Vast.GetInt(_digitValueStr);
                _vast.SetValue(_value, _digit, false);

                _digit++;
            }

            return _vast;
        }
        #endregion
        #region Public Method
        public void SetValue(int[] values, float pointValue, bool isQuickCalculating = false)
        {
            this.Clear();

            this.SetValue(false, values, pointValue, isQuickCalculating);
        }
        public void SetValue(int value, float pointValue, bool isQuickCalculating = false)
        {
            this.Clear();

            this.SetValue(false, pointValue, isQuickCalculating);
            this.SetValue(value, 1, false);
        }
        public void SetValue(float pointValue, bool isQuickCalculating = false)
        {
            this.Clear();

            this.SetValue(false, pointValue, isQuickCalculating);
        }
        public void SetValue(params int[] values)
        {
            this.Clear();

            this.SetValue(false, values);
        }
        public void SetValue(Vast value)
        {
            this.Clear();

            this.SetValue(false, value);
        }
        public void SetValue(int value, int digit, bool isClear)
        {
            if (isClear == true) this.Clear();

            Vast.Addition(this, value, digit);
        }
        public void SetValueFull()
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = 999;
        }
        public int GetValue(int digit = -1)
        {
            if (-1 < digit)
            {
                try
                { return values[digit]; }

                catch
                {
#if UNITY_EDITOR
                    Debug.LogError("Digit를 초과하여 요청함. 마지막 자릿수의 숫자로 대신 반환하겠음.");
#endif
                    return this.GetValue(-1);
                }
            }
            else
            {
                int _currentDigit = this.CurrentDigit;

                if (0 <= _currentDigit)
                    return values[_currentDigit];
                else
                    // 현 Vast는 비어있음
                    return -1;
            }
        }
        public string ToStringFull()
        {
            return this.GetStringFull();
        }
        public string ToStringFull(int point, bool isHaveComma = true)
        {
            return this.GetStringFull(point, isHaveComma);
        }
        public string ToStringLong()
        {
            return this.GetStringLong();
        }
        public string ToStringLong(int longDigit, int longValue)
        {
            return this.GetStringLong(longDigit, longValue);
        }
        public string ToStringLong(int point, bool isHaveComma = true)
        {
            return this.GetStringLong(point: point, isHaveComma: isHaveComma);
        }
        public string ToStringLong(int longDigit, int longValue, int point, bool isHaveComma = true)
        {
            return this.GetStringLong(longDigit, longValue, point, isHaveComma);
        }
        public void Clear()
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = 0;
        }
        public void Clear(int _digit)
        {
            if(_digit < this.values.Length)
            {
                this.values[_digit] = 0;
            }
            else
            {
                Debug.LogWarning("Out Of Index : " + _digit);
            }
        }
        public void Dispose()
        {
			if (isDispose == true)
                return;

            VastSystemManager.Despawn(this);
            isDispose = true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return this.GetString();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public Vast ToFloor()
        {
            Vast _result = VastSystemManager.Spawn();

            _result.SetValue(this);
            _result.Clear(0);

            return _result;
        }
        public Vast ToFloor(bool isCopy)
        {
            if (isCopy == true)
            {
                return ToFloor();
            }
            else
            {
                this.Clear(0);
                return this;
            }
        }
        public bool IsEmpty()
        {
            if (object.ReferenceEquals(this, null) == false)
            {
                for (int i = 0; i < this.values.Length; i++)
                {
                    if (0 != this.values[i])
                        return false;
                }
                
                return true;
            }
            else
                return true;
        }

        public static Vast ToVast(string value)
        {
            bool _isPointValueHave = value.Contains(".");
            bool _isCommaHave = value.Contains(",");

            if(_isCommaHave == true || _isPointValueHave == true)
            {
                return Vast.ToVast(value, _isCommaHave, _isPointValueHave);
            }
            else
            {
                return Vast.GetVast(value);
            }
        }
        public static Vast ToVast(string value, bool isCommaHave)
        {
            if (isCommaHave == true)
            {
                Vast _vast = VastSystemManager.Spawn();

                string[] _valueArr = value.Split(',');

                for (int _pivot = _valueArr.Length - 1, _digit = 1; 0 <= _pivot; _pivot--, _digit++)
                {
                    int _value = Vast.GetInt(_valueArr[_pivot]);
                    _vast.SetValue(_value, _digit, false);
                }

                return _vast;
            }
            else
            {
                return Vast.GetVast(value);
            }
        }
        public static Vast ToVast(string value, bool isCommaHave, bool isPointValueHave)
        {
            if(isPointValueHave == true)
            {
                string[] _valueArr = value.Split('.');
                
                Vast _value = VastSystemManager.Spawn();
                if (isCommaHave == true)
                {
                    _value = Vast.ToVast(_valueArr[0], isCommaHave);
                }
                else
                {
                    _value = Vast.GetVast(_valueArr[0]);
                }

                int _pointValue = 0;
                int _pointStringLength = _valueArr[1].Length;
                if (_pointStringLength == 2)
                    _valueArr[1] += "0";
                else if (_pointStringLength == 1)
                    _valueArr[1] += "00";

                _pointValue = Vast.GetInt(_valueArr[1]);

                _value.SetValue(_pointValue, 0, false);

                return _value;
            }
            else
            {
                return Vast.ToVast(value, isCommaHave);
            }
        }
        #endregion

        #region Common Interface
        bool IEqualityComparer<Vast>.Equals(Vast x, Vast y)
        {
            return x == y;
        }
        int IEqualityComparer<Vast>.GetHashCode(Vast obj)
        {
            return 0;
        }

        bool IEquatable<Vast>.Equals(Vast other)
        {
            return this == other;
        }
        #endregion

        public static class Math
        {
            public static Vast Pow(float f, float p, bool isQuickCalculating = false)
            {
                if (p <= 0)
                {
                    return 1;
                }
                else
                {
                    Vast _result = f.ToVast();
                    while (1f < p)
                    {
                        _result = _result * f;
                        p--;
                    }

                    return _result;
                }
            }
        }
    }
}

public static class VastExtensionMethod
{
    public static Vast ToVast(this float value, bool isQuickCalculating = false)
    {
        Vast _returnVast = VastSystemManager.Spawn();

        _returnVast.SetValue(value, isQuickCalculating);

        return _returnVast;
    }
    public static Vast ToVast(this string value)
    {
        return Vast.ToVast(value);
    }
    public static Vast ToVast(this string value, bool isCommaHave)
    {
        return Vast.ToVast(value, isCommaHave);
    }
    public static Vast ToVast(this string value, bool isCommaHave, bool isPointValueHave)
    {
        return Vast.ToVast(value, isCommaHave, isPointValueHave);
    }
    public static int GetEventLogValue(this Vast value)
    {
        int _eventLogValue = 0;

        int _currentDigit = value.CurrentDigit;
        if(0 < _currentDigit)
        {
            int _eventLogDigit = _currentDigit * 3;
            int _lastValue = value.GetValue();

            if(_lastValue < 10)         _eventLogDigit -= 2;
            else if(_lastValue < 100)   _eventLogDigit -= 1;

            int _lastBeforeValue =
                1 < _currentDigit
                ? _lastBeforeValue = value.GetValue(_currentDigit - 1)
                : 0;
            
            _eventLogValue =
                (_eventLogDigit * 1000000)
                + (_lastValue * (2 <= _currentDigit ? 1000 : 1))
                + _lastBeforeValue;
        }

        return _eventLogValue;
    }
}