using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cargold
{
    public static class Enum_C
    {
        public static T[] GetEnumItemAll_Func<T>() where T : struct, System.Enum
        {
            return EnumContainer<T>.GetEnumItemAll_Func();
        }
        public static List<T> GetEnumItemAll_Func<T>(params T[] _exceptEnumArr) where T : struct, System.Enum
        {
            return EnumContainer<T>.GetEnumItemAll_Func(_exceptEnumArr);
        }

        public static string ToString_Func<T>(this T _thisItem) where T : struct, System.Enum
        {
            return EnumContainer<T>.ToString_NoGC_Func(_thisItem);
        }
        public static T ToEnum_Func<T>(this string _thisItem) where T : struct, System.Enum
        {
            return EnumContainer<T>.ToEnum_NoGC_Func(_thisItem);
        }
        public static bool TryToEnum_Func<T>(this string _key, out T _value) where T : struct, System.Enum
        {
            return EnumContainer<T>.TryToEnum_Func(_key, out _value);
        }
        public static bool IsEnum_Func<T>(this string _key) where T : struct, System.Enum
        {
            return EnumContainer<T>.IsEnum_Func(_key);
        }

        [System.Serializable]
        public static class EnumContainer<T> where T : struct, System.Enum
        {
            [SerializeField] private static List<T> EnumList = new List<T>();
            [SerializeField] private static T[] AllEnumItemArr;
            [SerializeField] private static Dictionary<T, string> EnumByStrDic;
            [SerializeField] private static Dictionary<T, int> EnumByIntDic;
            [SerializeField] private static Dictionary<string, T> StrByEnumDic;

            public static void Init_Func(bool _isForced = true)
            {
                if (EnumList == null)
                    EnumList = new List<T>();

                if (_isForced == true && EnumContainer<T>.EnumByStrDic == null)
                    EnumContainer<T>.EnumByStrDic = new Dictionary<T, string>(EnumCompare.EnumCompare<T>.Instance);

                if(_isForced == true && EnumContainer<T>.EnumByIntDic == null)
                    EnumContainer<T>.EnumByIntDic = new Dictionary<T, int>(EnumCompare.EnumCompare<T>.Instance);

                if (_isForced == true && EnumContainer<T>.StrByEnumDic == null)
                    EnumContainer<T>.StrByEnumDic = new Dictionary<string, T>();

                T[] _arr = EnumContainer<T>.GetEnumItemAll_Func();

                foreach (T _enum in _arr)
                {
                    string _enumStr = _enum.ToString();
                    if (EnumByStrDic.ContainsKey(_enum) == false)
                        EnumByStrDic.Add(_enum, _enumStr);

                    int _enumID = _enum.ToInt();
                    if (EnumByIntDic.ContainsKey(_enum) == false)
                        EnumByIntDic.Add(_enum, _enumID);

                    if (StrByEnumDic.ContainsKey(_enumStr) == false)
                        StrByEnumDic.Add(_enumStr, _enum);
                }
            }

            public static T[] GetEnumItemAll_Func()
            {
                if(AllEnumItemArr.IsHave_Func() == false)
                {
                    Type _type = typeof(T);
                    Array _arr = Enum.GetValues(_type);
                    IEnumerable<T> _enumerable = _arr.Cast<T>();
                    AllEnumItemArr = _enumerable.ToArray();
                }

                return AllEnumItemArr;
            }
            public static List<T> GetEnumItemAll_Func(params T[] _exceptEnumArr)
            {
                if (EnumList == null || EnumByIntDic.IsHave_Func() == false)
                    Init_Func();

                EnumList.Clear();

                T[] _arr = EnumContainer<T>.GetEnumItemAll_Func();
                foreach (T _item in _arr)
                {
                    int _itemID = EnumByIntDic.GetValue_Func(_item);
                    bool _isExcept = false;

                    foreach (T _exceptEnum in _exceptEnumArr)
                    {
                        int _exceptEnumID = EnumByIntDic.GetValue_Func(_exceptEnum);

                        if (_itemID == _exceptEnumID)
                        {
                            _isExcept = true;
                            break;
                        }
                    }

                    if (_isExcept == true)
                        continue;

                    EnumList.Add(_item);
                }

                return EnumList;
            }
            public static string ToString_NoGC_Func(T _key)
            {
                string _value = null;

                if (EnumByStrDic.IsHave_Func() == false)
                    Init_Func();

                if (EnumByStrDic.TryGetValue(_key, out _value) == false)
                {
                    Debug_C.Warning_Func("다음 Enum Key가 캐싱되지 않았습니다 : " + _key);

                    _value = _key.ToString();
                }

                return _value;
            }
            public static T ToEnum_NoGC_Func(string _key)
            {
                if (StrByEnumDic.IsHave_Func() == false)
                    Init_Func();

                if (TryToEnum_Func(_key, out T _value) == false)
                {
                    Debug_C.Warning_Func("다음 Enum이 캐싱되어 있지 않아 동적 할당했습니다. 미리 캐싱하는 걸 추천합니다. : " + _key);

                    if (_key.IsNullOrWhiteSpace_Func() == true)
                        return default;

                    _value = (T)System.Enum.Parse(typeof(T), _key, true);

                    StrByEnumDic.Add(_key, _value);
                }

                return _value;
            }
            public static bool TryToEnum_Func(string _key, out T _value)
            {
                if (_key.IsNullOrWhiteSpace_Func() == true)
                {
                    _value = default;
                    return false;
                }

                _value = default;

                if (StrByEnumDic.IsHave_Func() == false)
                    Init_Func();

                return StrByEnumDic.TryGetValue(_key, out _value);
            }
            public static bool IsEnum_Func(string _key)
            {
                if (_key.IsNullOrWhiteSpace_Func() == true)
                {
                    return false;
                }

                if (StrByEnumDic.IsHave_Func() == false)
                    Init_Func();

                return StrByEnumDic.ContainsKey(_key);
            }
        }
    }
}