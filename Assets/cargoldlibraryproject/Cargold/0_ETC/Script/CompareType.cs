using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    public enum CompareType
    {
        // 어휘는 다음 링크를 참고함 https://speckofdust.tistory.com/111 

        None = 0,

        /// <summary>
        /// 이상
        /// </summary>
        More = 10,

        /// <summary>
        /// 이하
        /// </summary>
        Less = 20,

        /// <summary>
        /// 초과
        /// </summary>
        Above = 30,

        /// <summary>
        /// 미만
        /// </summary>
        Below = 40,

        /// <summary>
        /// 같음
        /// </summary>
        Equal = 50,

        /// <summary>
        /// 다름
        /// </summary>
        Unequal = 60,
    }

    [System.Serializable, InlineProperty]
    public struct CompareDataInt
    {
        [SerializeField, HorizontalGroup("1"), HideLabel, BoxGroup("1/값")] private int value;
        [SerializeField, HorizontalGroup("1"), HideLabel, BoxGroup("1/비교")] private CompareType compareType;

        public int GetValue { get => value; }
        public CompareType GetCompareType { get => compareType; }

        public CompareDataInt(int _value, CompareType _compareType)
        {
            this.value = _value;
            this.compareType = _compareType;
        }

        public bool IsCompare_Func(int _target)
        {
            return Extention_C.IsCompare_Func(this.value, this.compareType, _target);
        }

        public string ToString_Func()
        {
            return $"{this.value} {this.compareType.GetLczKor_Func()}";
        }
#if UNITY_EDITOR
        public bool CallEdit_IsUnitTestDone_Func()
        {
            return this.compareType != CompareType.None;
        }
#endif
    }
    [System.Serializable, InlineProperty]
    public struct CompareDataFloat
    {
        [SerializeField, HorizontalGroup("1"), HideLabel, BoxGroup("1/값")] private float value;
        [SerializeField, HorizontalGroup("1"), HideLabel, BoxGroup("1/비교")] private CompareType compareType;

        public bool IsCompare_Func(int _target)
        {
            return Extention_C.IsCompare_Func(this.value, this.compareType, _target);
        }
#if UNITY_EDITOR
        public bool CallEdit_IsUnitTestDone_Func()
        {
            return this.compareType != CompareType.None;
        }
#endif
    }

    public static partial class Extention_C
    {
        public static string GetLczKor_Func(this CompareType _compareType)
        {
            switch (_compareType)
            {
                case CompareType.More:      return "이상";
                case CompareType.Less:      return "이하";
                case CompareType.Above:     return "초과";
                case CompareType.Below:     return "미만";
                case CompareType.Equal:     return "같음";
                case CompareType.Unequal:   return "다름";

                default:
                    Debug_C.Error_Func(_compareType.ToString_Func());
                    return default;
            }
        }

        public static bool IsCompare_Func(this int _value, CompareType _type, int _target)
        {
            switch (_type)
            {
                case CompareType.Equal:
                    return _value == _target;

                case CompareType.Unequal:
                    return _value != _target;

                case CompareType.More:
                    return _value <= _target;

                case CompareType.Less:
                    return _value >= _target;

                case CompareType.Above:
                    return _value < _target;

                case CompareType.Below:
                    return _value > _target;

                default:
                    Debug_C.Error_Func(_type.ToString_Func());
                    return false;
            }
        }
        public static bool IsCompare_Func(this float _value, CompareType _type, float _target)
        {
            switch (_type)
            {
                case CompareType.Equal:
                    return _value == _target;

                case CompareType.Unequal:
                    return _value != _target;

                case CompareType.More:
                    return _value <= _target;

                case CompareType.Less:
                    return _value >= _target;

                case CompareType.Above:
                    return _value < _target;

                case CompareType.Below:
                    return _value > _target;

                default:
                    Debug_C.Error_Func(_type.ToString_Func());
                    return false;
            }
        }
        public static bool IsCompare_Func(this Cargold.Infinite.Infinite _value, CompareType _type, Cargold.Infinite.Infinite _target)
        {
            switch (_type)
            {
                case CompareType.Equal:
                    return _value == _target;

                case CompareType.Unequal:
                    return _value != _target;

                case CompareType.More:
                    return _value <= _target;

                case CompareType.Less:
                    return _value >= _target;

                case CompareType.Above:
                    return _value < _target;

                case CompareType.Below:
                    return _value > _target;

                default:
                    Debug_C.Error_Func(_type.ToString_Func());
                    return false;
            }
        }
    }
}