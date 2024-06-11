using Cargold.Infinite;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 0.1.4 ('20.08.06)
// Developed By Cargold
namespace Cargold.Infinite
{
    [System.Serializable]
    public partial struct Infinite : IEquatable<Infinite>, IEqualityComparer<Infinite>
    {
        #region Variable
        private static StringBuilder StringBuilder = new StringBuilder(128);
        private static decimal Thousand = new decimal(1000);
        private static Infinite _max;
        public static Infinite Max
        {
            get
            {
                if(_max.IsEmpty() == true)
                {
                    _max.currentDigit = int.MaxValue;
                    _max.values._val1 = 999;
                    _max.values._val2 = 999;
                    _max.values._val3 = 999;
                    _max.values._val4Ex = 999;
                    _max.values._val5Ex = 999;
                }

                return _max;
            }
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("데이터")]
#endif
        [SerializeField, Newtonsoft.Json.JsonRequired] private int currentDigit;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("데이터")]
#endif
        [SerializeField, Newtonsoft.Json.JsonRequired] private IntArrangement values;

        [Newtonsoft.Json.JsonIgnore] public int CurrentDigit { get { return this.currentDigit; } }
        [Newtonsoft.Json.JsonIgnore] public IntArrangement IntArr { get => values; }
        
        [System.Serializable]
        public struct IntArrangement
        {
            public int _val1;
            public int _val2;
            public int _val3;
            public int _val4Ex;
            public int _val5Ex;

            public int this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return this._val1;
                        case 1: return this._val2;
                        case 2: return this._val3;
                        case 3: return this._val4Ex;
                        case 4: return this._val5Ex;

                        default:
                            Debug.LogError("Out Of Range // Index : " + i);
                            return -1;
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: this._val1 = value; break;
                        case 1: this._val2 = value; break;
                        case 2: this._val3 = value; break;
                        case 3: this._val4Ex = value; break;
                        case 4: this._val5Ex = value; break;

                        default:
                            Debug.LogError("Out Of Range // Index : " + i);
                            break;
                    }
                }
            }
            public int Length
            {
                get { return 5; }
            }

            public void Clear()
            {
                this._val1 = 0;
                this._val2 = 0;
                this._val3 = 0;
                this._val4Ex = 0;
                this._val5Ex = 0;
            }
            public void ShiftDigit(int shiftIndex)
            {
                if(0 < shiftIndex && shiftIndex < 5)
                {
                    int i = 0;
                    int _pivot = 0;

                    for (; i < 5; ++i)
                    {
                        if(shiftIndex <= i)
                        {
                            this[_pivot] = this[i];

                            ++_pivot;
                        }

                        this[i] = 0;
                    }
                }
                else if(-5 < shiftIndex && shiftIndex < 0)
                {
                    shiftIndex = 4 + shiftIndex;

                    int i = 4;
                    int _pivot = 4;

                    for(; 0 <= i; --i)
                    {
                        if(i <= shiftIndex)
                        {
                            this[_pivot] = this[i];

                            --_pivot;
                        }

                        this[i] = 0;
                    }
                }
            }
        }
        #endregion

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.HorizontalGroup("1"), Sirenix.OdinInspector.BoxGroup("1/축약")]
        private string GetValueStr => this.ToString();

        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.HorizontalGroup("1"), Sirenix.OdinInspector.BoxGroup("1/전체")]
        private string GetValueFullStr => this.ToStringFull(3);
#endif

        #region Constructor & Destructor
        public Infinite(int value)
        {
            currentDigit = 0;
            values = new IntArrangement();

            this.Addition(value, 1);
        }
        public Infinite(float value, bool isQuickCalculating = false)
        {
            currentDigit = 0;
            values = new IntArrangement();

            int _quotientValue = 0;
            int _reminderValue = 0;
            Infinite.ExtractPointValue(value, out _quotientValue, out _reminderValue, isQuickCalculating);
            
            this.Addition(_quotientValue, 1);
            this.Addition(_reminderValue, 0);
        }
        #endregion
        #region Implicit & Explicit Casting
        public static implicit operator Infinite(int value)
        {
            return 0 < value
                ? new Infinite(value)
                : new Infinite();
        }
        public static implicit operator Infinite(List<int> values)
        {
            Infinite _inf = new Infinite();

            for (int i = 0; i < values.Count && i < 5; ++i)
            {
                if (0 < values[i])
                    _inf.Addition(values[i], values.Count - i);
            }

            return _inf;
        }
        public static implicit operator Infinite(float value)
        {
            return 0f < value
                ? new Infinite(value, true)
                : new Infinite();
        }
        public static implicit operator string(Infinite value)
        {
            return value.GetString();
        }

        public static explicit operator Infinite(string value)
        {
            return Infinite.ToInfinite(value);
        }
        public static explicit operator float(Infinite value)
        {
            float _returnValue = 0f;

            if(value.currentDigit == 0)
            {
                _returnValue += value.values._val1 * 0.001f;
            }
            else if (value.currentDigit == 1)
            {
                _returnValue += value.values._val1;
                _returnValue += value.values._val2 * 0.001f;
            }
            else if (value.currentDigit == 2)
            {
                _returnValue += value.values._val1 * 1000f;
                _returnValue += value.values._val2;
                _returnValue += value.values._val3 * 0.001f;
            }
            else
            {
                _returnValue = 999999.999f;
            }

            return _returnValue;
        }
        #endregion

        #region Calculate Group
        public static Infinite operator ++(Infinite inf)
        {
            return inf + 1;
        }
        public static Infinite operator +(Infinite inf, int value)
        {
            if(0 < value)
                inf.Addition(value, 1);

            return inf;
        }
        public static Infinite operator +(Infinite inf, float value)
        {
            if(0f < value)
            {
                int _quotientValue = 0;
                int _reminderValue = 0;
                Infinite.ExtractPointValue(value, out _quotientValue, out _reminderValue);

                inf.Addition(_quotientValue, 1);
                inf.Addition(_reminderValue, 0);
            }

            return inf;
        }
        public static Infinite operator +(int value, Infinite inf)
        {
            if(0 < value)
                inf.Addition(value, 1);

            return inf;
        }
        public static Infinite operator +(float value, Infinite inf)
        {
            if(0f < value)
            {
                int _quotientValue = 0;
                int _reminderValue = 0;
                Infinite.ExtractPointValue(value, out _quotientValue, out _reminderValue);

                inf.Addition(_quotientValue, 1);
                inf.Addition(_reminderValue, 0);
            }

            return inf;
        }
        public static Infinite operator +(Infinite leftInf, Infinite rightInf)
        {
            Infinite _biggerInf = 0;
            Infinite _smallerInf = 0;

            if (rightInf.currentDigit <= leftInf.currentDigit)
            {
                _biggerInf = leftInf;
                _smallerInf = rightInf;
            }
            else
            {
                _biggerInf = rightInf;
                _smallerInf = leftInf;
            }

            for (int i = 0; i < 5; i++)
            {
                int _digit = _smallerInf.currentDigit - i;
                if (0 <= _digit)
                {
                    int _digitGap = _biggerInf.currentDigit - _digit;
                    if (_digitGap < 5)
                    {
                        if(0 < _smallerInf.values[i])
                            _biggerInf.Addition(_smallerInf.values[i], _digit);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return _biggerInf;
        }
        public void Addition(int value, int digit)
        {
            if (0 <= digit)
            {
                if (1000 <= value)
                {
                    int _quotientValue = value / 1000;
                    value -= _quotientValue * 1000;

                    this.Addition(_quotientValue, digit + 1);
                }
            }
            else if (digit == -1)
            {
                if (1000 <= value)
                {
                    digit = 0;

                    value = (int)(value * 0.001f);
                }
                else if (500 <= value)
                {
                    // 반올림

                    digit = 0;

                    value = 1;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
            
            // 5이상이면 무시할 수 있는 값
            // 0~4이면 허용 범위의 값
            // 음수값이면 기존보다 높은 값
            int _digitGap = this.currentDigit - digit;

            // 허용 범위의 값인가?
            if (_digitGap < 5)
            {
                int _addDigitID = 0;

                // 현 디지트 이내의 값인 경우
                if (0 <= _digitGap)
                {
                    _addDigitID = _digitGap;
                }

                // 현 디지트보다 큰 값인 경우
                else
                {
                    this.currentDigit -= _digitGap;

                    // 기존값들이 허용범위 안에 있는가?
                    if (-5 < _digitGap)
                    {
                        // 기존값들을 디지트 격차만큼 밀어내기
                        
                        this.values.ShiftDigit(_digitGap);
                    }
                    else
                    {
                        this.values.Clear();
                    }

                    _addDigitID = 0;
                }

                int _addValue = this.values[_addDigitID] + value;
                if (_addValue < 1000)
                {
                    this.values[_addDigitID] = _addValue;
                }

                // Digit의 덧셈값이 1천 이상인가?
                else
                {
                    int _quotientValue = _addValue / 1000;
                    int _remainValue = _addValue - (_quotientValue * 1000); // 나머지

                    this.values[_addDigitID] = _remainValue;

                    this.Addition(_quotientValue, digit + 1);
                }
            }
        }

        public static Infinite operator --(Infinite inf)
        {
            return inf - 1;
        }
        public static Infinite operator -(Infinite inf, int value)
        {
            inf.Subtraction(value, 1);

            return inf;
        }
        public static Infinite operator -(Infinite inf, float value)
        {
            Infinite _floatInf = value;

            return inf - _floatInf;

            // 최적화
            //// Infinite가 Float보다 큰가?
            //if (true)
            //{
            //    int _quotientValue = 0;
            //    int _reminderValue = 0;
            //    Infinite.ExtractPointValue(value, out _quotientValue, out _reminderValue);

            //    inf.Subtraction(_reminderValue, 0);
            //    inf.Subtraction(_quotientValue, 1);

            //    return inf;
            //}
            //else
            //{
            //    inf.Clear();

            //    return inf;
            //}
        }
        public static Infinite operator -(int value, Infinite inf)
        {
            Infinite _intInf = value;

            return _intInf - inf;
        }
        public static Infinite operator -(float value, Infinite inf)
        {
            Infinite _floatInf = value;

            return _floatInf - inf;

            // 최적화
            //// Float가 Infinite보다 작은가?
            //if (true)
            //{
            //    inf.Clear();

            //    return inf;
            //}
            //else
            //{
            //    Infinite _floatInf = value;

            //    return _floatInf - inf;
            //}
        }
        public static Infinite operator -(Infinite biggerInf, Infinite smallerInf)
        {
            if(smallerInf < biggerInf)
            {
                for (int i = 0; i < 5; i++)
                {
                    int _digit = smallerInf.currentDigit - i;
                    if (0 <= _digit)
                    {
                        int _digitGap = biggerInf.currentDigit - _digit;
                        if (_digitGap < 5)
                        {
                            int _value = smallerInf.values[i];
                            if(0 < _value)
                                biggerInf.Subtraction(_value, _digit);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (biggerInf.values._val1 == 0)
                    {
                        --biggerInf.currentDigit;
                        biggerInf.values.ShiftDigit(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return biggerInf;
            }
            else
            {
                return new Infinite();
            }
        }
        public void Subtraction(int value, int digit)
        {
            if (0 <= digit)
            {
                if (1000 <= value)
                {
                    int _quotientValue = value / 1000;
                    value -= _quotientValue * 1000;

                    this.Subtraction(_quotientValue, digit + 1);
                }

                // 현재값이 뺄셈값 이상인가?
                if (digit <= this.currentDigit)
                {
                    // 5이상이면 무시해도 되는 작은값
                    // 0~4이면 허용 범위의 값
                    // 음수값이면 현재보다 높은 값
                    int _digitGap = this.currentDigit - digit;

                    // 허용 범위의 값인가?
                    if (_digitGap < 5)
                    {
                        int _subValue = this.values[_digitGap] - value;
                        if (0 <= _subValue)
                        {
                            this.values[_digitGap] = _subValue;
                        }
                        
                        // 뺄셈값이 동 Digit 내 현재값보다 큰 경우
                        else
                        {
                            // 내림 계산을 해야 함

                            // 현재값의 Digit가 뺄셈값의 Digit보다 더 높은가?
                            if (0 < _digitGap)
                            {
                                bool _isNeedShift = false;

                                // 값이 존재하는 Digit를 찾아서 값 가져오기
                                for (int i = _digitGap - 1; 0 <= i; --i)
                                {
                                    // Digit에 값이 0보다 많다면
                                    if (0 < this.values[i])
                                    {
                                        // 내림

                                        --this.values[i];

                                        if(this.values[i] == 0 && i == 0)
                                            _isNeedShift = true;

                                        break;
                                    }

                                    // Digit에 값이 0 이라면
                                    else if (this.values[i] == 0)
                                    {
                                        // 내림으로 인해 999값 삽입

                                        this.values[i] = 999;
                                    }

                                    else
                                    {
                                        // IntArr에 마이너스값이 존재함

                                        Debug.LogError("[Infinite] 진입할 수 없는 코드입니다.\ni : " + i);
                                    }
                                }

                                // 상위 Digit의 내림값과 뺄셈값을 계산한 뒤 해당 Digit에 삽입
                                // 덧셈을 하는 건, 뺄셈값이 이 로직에선 음수값이기 때문
                                this.values[_digitGap] = 1000 + _subValue;

                                if(_isNeedShift == true)
                                {
                                    --this.currentDigit;

                                    this.values.ShiftDigit(1);
                                }
                            }

                            // 현재값의 Digit가 뺄셈값 Digit와 같거나 더 적은가?
                            else
                            {
                                this.Clear();
                            }
                        }
                    }

                    else if(_digitGap < 0)
                    {
                        // 이미 현재값보다 높은 뺄셈값을 걸러내는 IF문이 있었으므로 여기로 진입하는 건 논리 오류

                        Debug.LogError("[Infinite] 진입할 수 없는 코드입니다.\n_digitGap : " + _digitGap);

                        this.Clear();
                    }
                }
                else
                {
                    this.Clear();
                }
            }
        }

        public static Infinite operator *(Infinite inf, int value)
        {
            if(0 < value)
            {
                Infinite _intInf = value;

                return inf.Multiplication(ref _intInf);
            }
            else
            {
                return new Infinite();
            }
        }
        public static Infinite operator *(Infinite inf, float value)
        {
            if (0f < value)
            {
                Infinite _floatInf = value;

                return inf.Multiplication(ref _floatInf);
            }
            else
            {
                return new Infinite();
            }
        }
        public static Infinite operator *(int value, Infinite inf)
        {
            if(0 < value)
            {
                Infinite _intInf = value;

                return _intInf.Multiplication(ref inf);
            }
            else
            {
                return new Infinite();
            }
        }
        public static Infinite operator *(float value, Infinite inf)
        {
            if (0f < value)
            {
                Infinite _floatInf = value;

                return inf.Multiplication(ref _floatInf);
            }
            else
            {
                return new Infinite();
            }
        }
        public static Infinite operator *(Infinite leftInf, Infinite rightInf)
        {
            if(leftInf.IsEmpty() == false && rightInf.IsEmpty() == false)
                return leftInf.Multiplication(ref rightInf);
            else
                return new Infinite();
        }
        public Infinite Multiplication(ref Infinite calcInf)
        {
            Infinite _resultInf = new Infinite();
            
            for (int _thisDigit = 0; _thisDigit < 5; ++_thisDigit)
            {
                int _thisValue = this.values[_thisDigit];

                for (int _calcDigit = 0; _calcDigit < 5; ++_calcDigit)
                {
                    int _multipledValue = _thisValue * calcInf.values[_calcDigit];
                    if(0 < _multipledValue)
                    {
                        int _multipledDigit = (this.currentDigit - _calcDigit) + (calcInf.currentDigit - _thisDigit) - 1;
                        if(-1 <= _multipledDigit)
                            _resultInf.Addition(_multipledValue, _multipledDigit);
                    }
                }
            }

            return _resultInf;
        }

        public static Infinite operator /(Infinite leftInf, int divideValue)
        {
            if(2 <= divideValue)
            {
                return leftInf.Division(divideValue, 3);
            }
            else if (1 == divideValue)
            {
                return leftInf;
            }
            else
            {
                return new Infinite();
            }
        }
        public static Infinite operator /(Infinite leftInf, Infinite rightInf)
        {
            if (leftInf.IsEmpty() == false && rightInf.IsEmpty() == false)
            {
                int _divideValue = 0;

                #region GetInt

                int _startDigit = rightInf.currentDigit <= 2 ? rightInf.currentDigit : 2;
                for (int _digit = _startDigit; 0 <= _digit; --_digit)
                {
                    int _addValue = rightInf.values[_digit];
                    if (0 < _addValue)
                    {
                        for (int _multipleCnt = _digit; _multipleCnt < _startDigit; ++_multipleCnt)
                        {
                            _addValue *= 1000;
                        }

                        _divideValue += _addValue;
                    }
                }
                #endregion

                return leftInf.Division(_divideValue, rightInf.currentDigit);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("0으로 나눌 수 없습니다.");
#endif
                return new Infinite();
            }
        }
        public Infinite Division(int divideValue, int digit)
        {
            Infinite _resultInf = new Infinite();

            long _remainValue = 0;
            for (int _arrID = 0; _arrID < 5; ++_arrID)
            {
                int _dividedDigit = this.currentDigit - digit - _arrID;
                if (2 <= digit) _dividedDigit++;
                else if (digit <= 0) _dividedDigit--;

                if (_dividedDigit < -1) break;

                long _originValue = this.values[_arrID];

                for (int j = 0; j < 2; j++)
                    _originValue *= 1000;

                _originValue += _remainValue;
                _remainValue = 0;

                long _quotientValue = 0 < divideValue ? _originValue / divideValue : _originValue;

                // 의역 : 허용 가능한 소수점인가? (직역 : 0 이상의 디지트인가?)
                if (-1 <= _dividedDigit)
                {
                    if (0 < _quotientValue)
                        _resultInf.Addition((int)_quotientValue, _dividedDigit);

                    long _reminderValue = _originValue - (divideValue * _quotientValue);

                    if (0f < _reminderValue)
                        _remainValue = _reminderValue * 1000;
                }
            }

            return _resultInf;
        }
        #endregion
        #region Compare Group
        public static bool operator ==(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.Equal);
        }
        public static bool operator !=(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.Unequal);
        }
        public static bool operator <(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.Over);
        }
        public static bool operator >(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.Under);
        }
        public static bool operator <=(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.EqualOrOver);
        }
        public static bool operator >=(Infinite infiniteValue, int intValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, true, CompareType.EqualOrUnder);
        }

        public static bool operator ==(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.Equal);
        }
        public static bool operator !=(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.Unequal);
        }
        public static bool operator <(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.Over);
        }
        public static bool operator >(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.Under);
        }
        public static bool operator <=(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.EqualOrOver);
        }
        public static bool operator >=(int intValue, Infinite infiniteValue)
        {
            return Infinite.GetCompare(ref infiniteValue, ref intValue, false, CompareType.EqualOrUnder);
        }

        public static bool operator ==(Infinite left, Infinite right)
        {
            if(left.currentDigit != right.currentDigit)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    if (left.values[i] != right.values[i]) return false;
            }
            
            return true;
        }
        public static bool operator !=(Infinite left, Infinite right)
        {
            if (left.currentDigit == right.currentDigit)
            {
                for (int i = 0; i < 5; i++)
                    if (left.values[i] != right.values[i]) return true;

                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool operator <(Infinite left, Infinite right)
        {
            if (left.currentDigit < right.currentDigit)
            {
                return true;
            }
            else if(right.currentDigit < left.currentDigit)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (left.values[i] == right.values[i]) continue;
                    if (left.values[i] < right.values[i]) return true;
                    else return false;
                }

                return false;
            }
        }
        public static bool operator >(Infinite left, Infinite right)
        {
            if (right.currentDigit < left.currentDigit)
            {
                return true;
            }
            else if(left.currentDigit < right.currentDigit)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (left.values[i] == right.values[i]) continue;
                    if (left.values[i] > right.values[i]) return true;
                    else return false;
                }

                return false;
            }
        }
        public static bool operator <=(Infinite left, Infinite right)
        {
            if (left.currentDigit < right.currentDigit)
            {
                return true;
            }
            else if (right.currentDigit < left.currentDigit)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (left.values[i] == right.values[i]) continue;
                    if (left.values[i] <= right.values[i]) return true;
                    else return false;
                }

                return true;
            }
        }
        public static bool operator >=(Infinite left, Infinite right)
        {
            if (right.currentDigit < left.currentDigit)
            {
                return true;
            }
            else if (left.currentDigit < right.currentDigit)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (left.values[i] == right.values[i]) continue;
                    if (left.values[i] >= right.values[i]) return true;
                    else return false;
                }

                return true;
            }
        }

        private enum CompareType
        {
            Equal,
            Unequal,
            Over,
            Under,
            EqualOrOver,
            EqualOrUnder,
        }
        #endregion

        #region Private Method
        private string GetStringFull(int point = -1, bool isHaveComma = true)
        {
            Infinite.StringBuilder.Clear();

            int _arrID = 0;

            if (point == -1)
                point = InfiniteSystemManager.Instance.DefaultPoint;

            if (0 < this.currentDigit)
            {
                int _digitValue = this.values._val1;
                Infinite.StringBuilder.Append(_digitValue.ToString());

                ++_arrID;
            }
            else
                Infinite.StringBuilder.Append("0");

            for (; _arrID <= this.currentDigit; ++_arrID)
            {
                if (_arrID < 5)
                {
                    // 정수값
                    if (0 < this.currentDigit - _arrID)
                    {
                        int _digitValue = this.values[_arrID];

                        if (isHaveComma == true)
                            Infinite.StringBuilder.AppendFormat(",{0:000}", _digitValue);
                        else
                            Infinite.StringBuilder.AppendFormat("{0:000}", _digitValue);
                    }

                    // 실수값
                    else
                    {
                        if (0 < point)
                        {
                            int _pointValue = this.values[_arrID];

                            if (0 < _pointValue)
                            {
                                string _pointString = "";
                                _pointString = GetPointString(ref _pointValue, point);
                                Infinite.StringBuilder.Append(".").Append(_pointString);
                            }
                        }
                    }
                }
                else
                {
                    // 정수값
                    if (0 < this.currentDigit - _arrID)
                    {
                        if (isHaveComma == true)
                            Infinite.StringBuilder.Append(",000");
                        else
                            Infinite.StringBuilder.Append("000");
                    }

                    // 실수값
                    else
                    {
                        if (0 < point)
                        {
                            int _pointValue = 0;
                            string _pointString = Infinite.GetPointString(ref _pointValue, point);
                            Infinite.StringBuilder.Append(".").Append(_pointString);
                        }
                    }
                }
            }
            
            return Infinite.StringBuilder.ToString();
        }
        private string GetStringLong(int longDigit = -1, int longValue = -1, int point = -1, bool isHaveComma = true)
        {
            if (longDigit == -1)
                longDigit = InfiniteSystemManager.Instance.LongDataValue.longDigit;
            
            if (longDigit < this.currentDigit)
            {
                return this.GetString(); 
            }
            else if(this.currentDigit == longDigit)
            {
                if (longValue == -1)
                    longValue = InfiniteSystemManager.Instance.LongDataValue.longValue;

                return this.values._val1 <= longValue
                    ? this.GetStringFull(point, isHaveComma)
                    : this.GetString();
            }
            else
            {
                return this.GetStringFull(point, isHaveComma);
            }
        }
        private string GetString(int point = -1)
        {
            if (point == -1)
                point = InfiniteSystemManager.Instance.DefaultPoint;

            if (0 < this.currentDigit)
            {
                Infinite.StringBuilder.Clear();

                int _digitValue = this.values._val1;
                Infinite.StringBuilder.Append(_digitValue);

                if (0 < point)
                {
                    int _pointValue = this.values._val2;
                    if (0 < _pointValue)
                    {
                        string _pointString = string.Empty;
                        _pointString = GetPointString(ref _pointValue, point);
                        Infinite.StringBuilder.Append(".").Append(_pointString);
                    }
                }

                if (2 <= this.currentDigit)
                {
                    string _digitUnit = InfiniteSystemManager.Instance.GetDigitUnit(this.currentDigit - 2);
                    Infinite.StringBuilder.Append(_digitUnit);
                }

                return Infinite.StringBuilder.ToString();
            }
            else if (0 == this.currentDigit)
            {
                return (this.values._val1 * 0.001f).ToString();
            }
            else
            {
                return "0";
            }
        }

        private static void ExtractPointValue(float pointValue, out int quotientValue, out int reminderValue, bool isQuickCalculating = true)
        {
            if (isQuickCalculating == true)
            {
                quotientValue = (int)pointValue;

                float _value = pointValue - quotientValue;
                reminderValue = (int)(_value * 1000f);
            }
            else
            {
                decimal _pointValueDecimal = new Decimal(pointValue);

                decimal _quotientValueDecimal = Decimal.Truncate(_pointValueDecimal);
                quotientValue = Decimal.ToInt32(_quotientValueDecimal);

                decimal _reminderValueDecimal = Decimal.Multiply(_pointValueDecimal - _quotientValueDecimal, Infinite.Thousand);
                reminderValue = Decimal.ToInt32(_reminderValueDecimal);
            }
        }
        private static string GetPointString(ref int pointValue, int point = -1)
        {
            if (point == -1)
                point = InfiniteSystemManager.Instance.DefaultPoint;

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
        private static Infinite GetInfinite(string value)
        {
            // Optimization : 가장 낮은 값부터 3자리씩 캐스팅하고 있음. 가장 높은 값부터 5 Digit만 하게끔 변경.

            Infinite _inf = new Infinite();
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

                int _value = string.IsNullOrEmpty(_digitValueStr) == false 
                    ? System.Int32.Parse(_digitValueStr)
                    : 0;

                if (0 < _value)
                    _inf.Addition(_value, _digit);

                _digit++;
            }

            return _inf;
        }
        
        private static bool GetCompare(ref Infinite infiniteValue, ref int intValue, bool isIntRight, CompareType compareType)
        {
            if (infiniteValue.currentDigit < 5)
            {
                if (intValue < 1000)
                {
                    if(0 < intValue)
                    {
                        if (infiniteValue.currentDigit == 1)
                        {
                            switch (compareType)
                            {
                                case CompareType.Equal:         return infiniteValue.values._val1 == intValue;

                                case CompareType.Unequal:       return infiniteValue.values._val1 != intValue;

                                case CompareType.Over:          return isIntRight == true
                                        ? infiniteValue.values._val1 < intValue
                                        : intValue < infiniteValue.values._val1;

                                case CompareType.Under:         return isIntRight == true
                                        ? infiniteValue.values._val1 > intValue
                                        : intValue > infiniteValue.values._val1;

                                case CompareType.EqualOrOver:   return isIntRight == true
                                        ? infiniteValue.values._val1 <= intValue
                                        : intValue <= infiniteValue.values._val1;

                                case CompareType.EqualOrUnder:  return isIntRight == true
                                        ? infiniteValue.values._val1 >= intValue
                                        : intValue >= infiniteValue.values._val1;

                                default:
    #if UNITY_EDITOR
                                    Debug.LogError("잘못된 비교 타입 : " + compareType);
    #endif
                                    return false;
                            }
                        }
                        else
                        {
                            switch (compareType)
                            {
                                case CompareType.Equal:         return false;

                                case CompareType.Unequal:       return true;

                                case CompareType.Over:
                                case CompareType.EqualOrOver:   return isIntRight == true
                                        ? infiniteValue.currentDigit < 1
                                        : 1 < infiniteValue.currentDigit;

                                case CompareType.Under:         
                                case CompareType.EqualOrUnder:  return isIntRight == true
                                        ? infiniteValue.currentDigit > 1
                                        : 1 > infiniteValue.currentDigit;

                                default:
    #if UNITY_EDITOR
                                    Debug.LogError("잘못된 비교 타입 : " + compareType);
    #endif
                                    return false;
                            }
                        }
                    }
                    
                    // Int 0과 비교
                    else
                    {
                        switch (compareType)
                            {
                                case CompareType.Equal:         return infiniteValue.IsEmpty();

                                case CompareType.EqualOrOver:   return isIntRight == true
                                    ? infiniteValue.IsEmpty()
                                    : true;
                                    
                                case CompareType.EqualOrUnder:  return isIntRight == true
                                    ? true
                                    : infiniteValue.IsEmpty();

                                case CompareType.Over:          return isIntRight == true
                                    ? false
                                    : !infiniteValue.IsEmpty();

                                case CompareType.Under:         return isIntRight == true
                                    ? !infiniteValue.IsEmpty()
                                    : false;

                                case CompareType.Unequal:       return !infiniteValue.IsEmpty();

                            default:
    #if UNITY_EDITOR
                                    Debug.LogError("잘못된 비교 타입 : " + compareType);
    #endif
                                    return false;
                            }
                    }
                }
                else if (intValue < 1000000)
                {
                    if (infiniteValue.currentDigit == 2)
                    {
                        int _infInt = 0;

                        _infInt += infiniteValue.values._val1 * 1000;
                        _infInt += infiniteValue.values._val2;

                        switch (compareType)
                        {
                            case CompareType.Equal:         return _infInt == intValue;

                            case CompareType.Unequal:       return _infInt != intValue;

                            case CompareType.Over:          return isIntRight == true
                                    ? _infInt < intValue
                                    : intValue < _infInt;

                            case CompareType.Under:         return isIntRight == true
                                    ? _infInt > intValue
                                    : intValue > _infInt;

                            case CompareType.EqualOrOver:   return isIntRight == true
                                    ? _infInt <= intValue
                                    : intValue <= _infInt;

                            case CompareType.EqualOrUnder:  return isIntRight == true
                                    ? _infInt >= intValue
                                    : intValue >= _infInt;

                            default:
#if UNITY_EDITOR
                                Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                                return false;
                        }
                    }
                    else
                    {
                        switch (compareType)
                        {
                            case CompareType.Equal:         return false;

                            case CompareType.Unequal:       return true;

                            case CompareType.Over:
                            case CompareType.EqualOrOver:   return isIntRight == true
                                    ? infiniteValue.currentDigit < 2
                                    : 2 < infiniteValue.currentDigit;

                            case CompareType.Under:         
                            case CompareType.EqualOrUnder:  return isIntRight == true
                                    ? infiniteValue.currentDigit > 2
                                    : 2 > infiniteValue.currentDigit;

                            default:
#if UNITY_EDITOR
                                Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                                return false;
                        }
                    }
                }
                else if (intValue < 1000000000)
                {
                    if (infiniteValue.currentDigit == 3)
                    {
                        int _infInt = 0;

                        _infInt += infiniteValue.values._val1 * 1000000;
                        _infInt += infiniteValue.values._val2 * 1000;
                        _infInt += infiniteValue.values._val3;

                        switch (compareType)
                        {
                            case CompareType.Equal:         return _infInt == intValue;

                            case CompareType.Unequal:       return _infInt != intValue;

                            case CompareType.Over:          return isIntRight == true
                                    ? _infInt < intValue
                                    : intValue < _infInt;

                            case CompareType.Under:         return isIntRight == true
                                    ? _infInt > intValue
                                    : intValue > _infInt;

                            case CompareType.EqualOrOver:   return isIntRight == true
                                    ? _infInt <= intValue
                                    : intValue <= _infInt;

                            case CompareType.EqualOrUnder:  return isIntRight == true
                                    ? _infInt >= intValue
                                    : intValue >= _infInt;

                            default:
#if UNITY_EDITOR
                                Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                                return false;
                        }
                    }
                    else
                    {
                        switch (compareType)
                        {
                            case CompareType.Equal:         return false;

                            case CompareType.Unequal:       return true;

                            case CompareType.Over:          
                            case CompareType.EqualOrOver:   return isIntRight == true
                                    ? infiniteValue.currentDigit < 3
                                    : 3 < infiniteValue.currentDigit;

                            case CompareType.Under:         
                            case CompareType.EqualOrUnder:  return isIntRight == true
                                    ? infiniteValue.currentDigit > 3
                                    : 3 > infiniteValue.currentDigit;

                            default:
#if UNITY_EDITOR
                                Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                                return false;
                        }
                    }
                }
                else
                {
                    Infinite _intInf = new Infinite(intValue);

                    switch (compareType)
                    {
                        case CompareType.Equal:         return infiniteValue == _intInf;

                        case CompareType.Unequal:       return infiniteValue != _intInf;

                        case CompareType.EqualOrOver:   return isIntRight == true
                                ? infiniteValue <= _intInf
                                : _intInf <= infiniteValue;

                        case CompareType.Over:          return isIntRight == true
                                ? infiniteValue < _intInf
                                : _intInf < infiniteValue;

                        case CompareType.EqualOrUnder:  return isIntRight == true
                                ? infiniteValue >= _intInf
                                : _intInf >= infiniteValue;

                        case CompareType.Under:         return isIntRight == true
                                ? infiniteValue > _intInf
                                : _intInf > infiniteValue;

                        default:
#if UNITY_EDITOR
                            Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                            return false;
                    }
                }
            }
            else
            {
                switch (compareType)
                {
                    case CompareType.Equal:         return false;

                    case CompareType.Unequal:       return true;

                    case CompareType.Over:          
                    case CompareType.EqualOrOver:   return !isIntRight;

                    case CompareType.Under:         
                    case CompareType.EqualOrUnder:  return isIntRight;

                    default:
#if UNITY_EDITOR
                        Debug.LogError("잘못된 비교 타입 : " + compareType);
#endif
                        return false;
                }
            }
        }
        #endregion
        #region Public Method
        public void Clear()
        {
            this.currentDigit = 0;
            this.values.Clear();
        }
        public void SetDigit(int digit)
        {
            this.currentDigit = digit;
        }
        public string ToString(int point = -1)
        {
            return GetString(point);
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
        public string ToStringLong(int longDigit = -1, int longValue = -1, int point = -1, bool isHaveComma = true)
        {
            return this.GetStringLong(longDigit, longValue, point, isHaveComma);
        }
        public string ToStringForLog()
        {
            StringBuilder.Clear();

            string _value1 = this.values._val1.ToString_Fill_Func(3);
            string _value2 = this.values._val2.ToString_Fill_Func(3);
            return StringBuilder.Append(_value1).Append(_value2).Append("_").Append(this.currentDigit).ToString();
        }
        public bool IsEmpty()
        {
            if(this.currentDigit == 0
            && this.values._val1 == 0
            && this.values._val2 == 0)
                return true;
            else
                return false;
        }
        public Infinite ToFloor()
        {
            if(this.currentDigit < 5)
            {
                this.values[this.currentDigit] = 0;
            }

            return this;
        }
        public Infinite ToRound()
        {
            if (this.currentDigit < 5)
            {
                int _value = this.values[this.currentDigit];

                this.values[this.currentDigit] = 0;

                if (500 <= _value)
                    this.Addition(1, 1);
            }

            return this;
        }
        public override string ToString()
        {
            return this.GetString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static Infinite ToInfinite(string value)
        {
            bool _isPointValueHave = value.Contains(".");
            bool _isCommaHave = value.Contains(",");

            if (_isCommaHave == true || _isPointValueHave == true)
            {
                return Infinite.ToInfinite(value, _isCommaHave, _isPointValueHave);
            }
            else
            {
                return Infinite.GetInfinite(value);
            }
        }
        public static Infinite ToInfinite(string value, bool isCommaHave)
        {
            // Optimization : 가장 낮은 String Type Digit부터 캐스팅하고 있음. 가장 높은 값부터 5 Digit만 하게끔 변경.

            if (isCommaHave == true)
            {
                Infinite _inf = new Infinite();

                string[] _valueArr = value.Split(',');

                for (int _pivot = _valueArr.Length - 1, _digit = 1; 0 <= _pivot; _pivot--, _digit++)
                {
                    int _value = string.IsNullOrEmpty(_valueArr[_pivot]) == false 
                        ? System.Int32.Parse(_valueArr[_pivot])
                        : 0;
                    
                    if(0 < _value)
                        _inf.Addition(_value, _digit);
                }

                return _inf;
            }
            else
            {
                return Infinite.GetInfinite(value);
            }
        }
        public static Infinite ToInfinite(string value, bool isCommaHave, bool isPointValueHave)
        {
            if (isPointValueHave == true)
            {
                string[] _valueArr = value.Split('.');

                Infinite _value = new Infinite();
                if (isCommaHave == true)
                {
                    _value = Infinite.ToInfinite(_valueArr[0], isCommaHave);
                }
                else
                {
                    _value = Infinite.GetInfinite(_valueArr[0]);
                }

                int _pointValue = 0;
                int _pointStringLength = _valueArr[1].Length;
                if (_pointStringLength == 2)
                    _valueArr[1] += "0";
                else if (_pointStringLength == 1)
                    _valueArr[1] += "00";

                _pointValue = string.IsNullOrEmpty(_valueArr[1]) == false
                    ? System.Int32.Parse(_valueArr[1])
                    : 0;

                if(0 < _pointValue)
                    _value.Addition(_pointValue, 0);

                return _value;
            }
            else
            {
                return Infinite.ToInfinite(value, isCommaHave);
            }
        }
        #endregion

        #region Common Interface
        bool IEquatable<Infinite>.Equals(Infinite otherInf)
        {
            if (this.currentDigit == otherInf.currentDigit)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (this.values[i] != otherInf.values[i])
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        bool IEqualityComparer<Infinite>.Equals(Infinite leftInf, Infinite rightInf)
        {
            if (leftInf.currentDigit == rightInf.currentDigit)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (leftInf.values[i] != rightInf.values[i])
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        int IEqualityComparer<Infinite>.GetHashCode(Infinite inf)
        {
            return inf.GetHashCode();
        }
        #endregion

        #region Static Method
        public static Infinite Pow(string key, int cnt)
        {
            return Pow_C.Pow(key, cnt);
        }
        public static Infinite Pow(float value, int cnt)
        {
            Infinite _infValue = value;
            return Pow(_infValue, cnt);
        }
        public static Infinite Pow(Infinite value, int cnt)
        {
            return Pow(value, cnt, value);
        }
        public static Infinite Pow(Infinite value, int cnt, Infinite powValue)
        {
            if (1 < cnt)
            {
                for (int i = 1; i < cnt; i++)
                {
                    powValue *= value;
                }

                return powValue;
            }
            else if (cnt == 1)
            {
                return powValue;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        public static class Pow_C
        {
#if ODIN_INSPECTOR
            [Sirenix.OdinInspector.ReadOnly, Sirenix.OdinInspector.ShowInInspector]
#endif
            private static Dictionary<string, PowControl> powControlDic;

            public static void Initialize(string _key, Infinite _defaultPowValue)
            {
                if (powControlDic == null)
                    powControlDic = new Dictionary<string, PowControl>();

                PowControl _powControl = new PowControl(_defaultPowValue);
                if(powControlDic.ContainsKey(_key) == false)
                {
                    powControlDic.Add(_key, _powControl);
                }
                else
                {
                    powControlDic.Remove(_key);
                    powControlDic.Add(_key, _powControl);
                }
            }

            public static bool IsHavePowControl_Func(string _key)
            {
                return powControlDic != null && powControlDic.ContainsKey(_key);
            }

            public static Infinite Pow(string key, int cnt)
            {
                if (powControlDic.ContainsKey(key) == true)
                {
                    PowControl _powControl = powControlDic[key];
                    return _powControl.Pow(cnt);
                }
                else
                {
                    return 0;
                }
            }

            private class PowControl
            {
#if ODIN_INSPECTOR
                [Sirenix.OdinInspector.ReadOnly, Sirenix.OdinInspector.ShowInInspector]
#endif
                private Dictionary<int, Infinite> powValueDic;
                private Infinite defaultPowValue;

                public PowControl(Infinite defaultPowValue)
                {
                    this.powValueDic = new Dictionary<int, Infinite>();
                    this.defaultPowValue = defaultPowValue;
                }

                public Infinite Pow(int cnt)
                {
                    if (this.powValueDic.TryGetValue(cnt, out Infinite powValue) == false)
                    {
                        powValue = Infinite.Pow(this.defaultPowValue, cnt); // 이전 제곱을 찾아 거기서 부터 제곱 반복문 돌리기 ㄱㄱ
                        this.powValueDic.Add(cnt, powValue);
                    }

                    return powValue;
                }
            }
        }
    }

    public static partial class InfiniteExtensionMethod
    {
        public static Infinite ToInfinite(this string value)
        {
            return Infinite.ToInfinite(value);
        }
        public static Infinite ToInfinite(this string value, bool isCommaHave)
        {
            return Infinite.ToInfinite(value, isCommaHave);
        }
        public static Infinite ToInfinite(this string value, bool isCommaHave, bool isPointValueHave)
        {
            return Infinite.ToInfinite(value, isCommaHave, isPointValueHave);
        }

        public static int GetEventLogValue(this Infinite _value)
        {
            int _eventLogValue = _value.CurrentDigit *  1000000000;
            _eventLogValue += _value.IntArr[0] *        1000000;
            _eventLogValue += _value.IntArr[1] *        1000;
            _eventLogValue += _value.IntArr[2];

            return _eventLogValue;
        }
    }
}