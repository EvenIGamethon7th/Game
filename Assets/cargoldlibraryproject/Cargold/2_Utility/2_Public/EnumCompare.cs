using System;
using System.Collections.Generic;

namespace Cargold.EnumCompare
{
    using Unity.Collections.LowLevel.Unsafe;

    public class EnumCompare<T> : IEqualityComparer<T> where T : struct, IConvertible
    {
        private static EnumCompare<T> instance;
        public static EnumCompare<T> Instance
        {
            get
            {
                if (EnumCompare<T>.instance == null)
                    EnumCompare<T>.instance = new EnumCompare<T>();

                return EnumCompare<T>.instance;
            }
        }

        bool IEqualityComparer<T>.Equals(T _x, T _y)
        {
            int _xInt = 0;
            int _yInt = 0;

#if UNITY_2018_1_OR_NEWER
            _xInt = UnsafeUtility.EnumToInt(_x);
            _yInt = UnsafeUtility.EnumToInt(_y);
#else
            _xInt = EnumCompare<T>.Enum32ToInt(_x);
            _yInt = EnumCompare<T>.Enum32ToInt(_y);
#endif

            return _xInt == _yInt;
        }

        int IEqualityComparer<T>.GetHashCode(T _obj)
        {
#if UNITY_2018_1_OR_NEWER
            return UnsafeUtility.EnumToInt(_obj);
#else
            return EnumCompare<T>.Enum32ToInt(_obj);
#endif
        }

#if UNITY_2018_1_OR_NEWER

#else
        public static int Enum32ToInt(T _enumValue)
        {
            Shell _shell = new Shell();
            _shell.EnumValue = _enumValue;

            unsafe
            {
                int* pi = &_shell.IntValue;
                pi += 1;
                return *pi;
            }
        }

        public static T IntToEnum32(int _value)
        {
            Shell _shell = new Shell();

            unsafe
            {
                int* pi = &_shell.IntValue;
                pi += 1;
                *pi = _value;
            }

            return _shell.EnumValue;
        }

        private struct Shell
        {
            public int IntValue;
            public T EnumValue;
        }
#endif

        public static Dictionary<T, Value> GetEnumDic_Func<Value>(Dictionary<T, Value> _baseDic)
        {
            Dictionary<T, Value> _enumDic = new Dictionary<T, Value>(EnumCompare<T>.Instance);

            foreach (KeyValuePair<T, Value> item in _baseDic)
                _enumDic.Add(item.Key, item.Value);

            return _enumDic;
        }
    }
}