using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;


namespace Cargold
{
    // Extension Function
    public static class Cargold_Library
    {
        public const string Signature = "_C";

        #region Animation Group
        public static void Play_Func(this Animation _anim, bool _isRewind = false, bool _isImmediatly = false, float _speed = 1f, bool isIgnoreTimeScale = false)
        {
            _anim.Play_Func(_anim.clip, _isRewind, _isImmediatly, _speed, isIgnoreTimeScale);
        }
        public static void Play_Func(this Animation _anim, AnimationClip _clip, bool _isRewind = false, bool _isImmediatly = false, float _speed = 1f, bool isIgnoreTimeScale = false)
        {
            // _isImmediatly를 사용할 경우 애니메이션 이벤트 함수는 작동 안 됨

            if (_anim != null)
            {
                if (_anim.gameObject.activeSelf == false)
                    _anim.gameObject.SetActive(true);

                string _clipName = _clip.name;
#if UNITY_EDITOR
                AnimationClip _haveClip = _anim.GetClip(_clipName);
                if (_haveClip == null)
                {
                    Debug_C.Warning_Func("애니메이션에 다음 클립이 없습니다. 안전성을 위해 기 삽입하시길 바랍니다. : " + _clipName);
                    _anim.transform.OnLogPath_Func();

                    _anim.AddClip(_clip, _clipName);

                    _haveClip = _anim.GetClip(_clipName);
                    if (_haveClip == null)
                        Debug_C.Error_Func("?");
                }

                if (_haveClip.legacy == false)
                {
                    _haveClip.legacy = true;

                    Debug_C.Warning_Func($"{_clipName} 클립을 Legacy로 강제 전환했습니다.");
                }
#endif

                AnimationState _animationState = _anim[_clipName];

#if UNITY_EDITOR
                if (_animationState == null)
                {
                    Debug_C.Error_Func($"{_clipName} 클립을 찾을 수 없습니다.");
                    return;
                } 
#endif

                float _time = 0f;
                if (_isImmediatly == false)
                {
                    if (_isRewind == false)
                    {
                        _time = 0f;
                    }
                    else
                    {
                        _speed *= -1f;

                        _time = _animationState.length;
                    }
                }
                else
                {
                    _speed = 0f;

                    if (_isRewind == false)
                    {
                        _time = _animationState.length;
                    }
                    else
                    {
                        _time = 0f;
                    }
                }

                _animationState.speed = _speed;
                _animationState.time = _time;
                _anim.Play(_clipName);

                if (isIgnoreTimeScale)
                {
                    _animationState.speed = 0;
                    var animState = _anim[_clipName];

                    UpdateUnscaled().Forget();

                    async UniTask UpdateUnscaled()
                    {
                        while (animState.normalizedTime < 1)
                        {
                            await UniTask.DelayFrame(1);
                            animState.normalizedTime += (Time.unscaledDeltaTime / animState.length);
                        }
                    }
                }
            }
            else
            {
                Debug_C.Error_Func("애니메이션 컴포넌트가 비어있습니다. : " + _anim.gameObject.name);
            }
        }
        public static void Play_Func(this Animation _anim, AniData_C _aniData, bool _isImmediatly = false)
        {
            _aniData.Play_Func(_anim, _isImmediatly);
        }
        #endregion
        #region Animator
        public static bool IsTag_Func(this Animator _anim, string _tagStr, int _stateInfoID = 0)
        {
            AnimatorStateInfo _stateInfo = _anim.GetCurrentAnimatorStateInfo(_stateInfoID);
            return _stateInfo.IsTag(_tagStr);
        }
        #endregion
        #region Array Group
        public static bool Contain_Func<T>(this T[] _arr, T _target)
        {
            foreach (T _item in _arr)
            {
                if ((object)_item == (object)_target)
                    return true;
            }

            return false;
        }
        public static bool Contain_Func<T>(this T[] _arr, T[] _targetArr)
        {
            foreach (T _item in _arr)
            {
                foreach (T _target in _targetArr)
                {
                    if ((object)_item == (object)_target)
                        return true;
                }
            }

            return false;
        }
        public static bool IsHave_Func<T>(this T[] _arr, int _minItemNum = 1)
        {
            return _arr != null && _minItemNum <= _arr.Length;
        }
        public static T[] GetAdd_Func<T>(this T[] _arr, T _addItem)
        {
            T[] _newArr = null;

            int _arrNum = 0;

            if (_arr.IsHave_Func() == true)
                _arrNum = _arr.Length;

            _newArr = new T[_arrNum + 1];

            for (int i = 0; i < _arrNum; i++)
                _newArr[i] = _arr[i];

            _newArr[_arrNum] = _addItem;

            return _newArr;
        }
        public static T[] GetRemove_Func<T>(this T[] _arr, int _index) where T : class
        {
            T _removeItem = _arr[_index];
            return GetRemove_Func(_arr, _removeItem);
        }
        public static T[] GetRemove_Func<T>(this T[] _arr, T _removeItem) where T : class
        {
            T[] _newArr = null;

            int _arrNum = 0;
            
            if (_arr.IsHave_Func() == true)
                _arrNum = _arr.Length;

            _newArr = new T[_arrNum - 1];

            for (int i = 0, j = 0; i < _arrNum; i++)
            {
                if (_arr[i] == _removeItem) continue;

                _newArr[j] = _arr[i];
                j++;
            }

            return _newArr;
        }
        public static T[] InsertFirst_Func<T>(this T[] _arr, T _insertItem)
        {
            T[] _newArr = new T[_arr.Length + 1];

            for (int i = 0; i < _arr.Length; i++)
            {
                _newArr[i + 1] = _arr[i];
            }

            _newArr[0] = _insertItem;

            return _newArr;
        }
        public static int GetRandID_Func<T>(this T[] _arr)
        {
#if UNITY_EDITOR

            if (_arr == null)
            {
                Debug.LogError("배열이 Null");
            }

            if (_arr.Length <= 0)
            {
                Debug.LogError("배열 크기가 0 이하임");
            }
#endif

            return Random_C.Random_Func(0, _arr.Length);
        }
        public static T GetLastItem_Func<T>(this T[] _arr)
        {
            return _arr[_arr.Length - 1];
        }
        public static T GetRandItem_Func<T>(this T[] _arr)
        {
            int _temp = 0;
            return _arr.GetRandItem_Func(out _temp);
        }
        public static T GetRandItem_Func<T>(this T[] _arr, int _startIndex = 0, int _lastIndex = -1)
        {
            int _temp = 0;
            return _arr.GetRandItem_Func(out _temp, _startIndex, _lastIndex);
        }
        public static T GetRandItem_Func<T>(this T[] _arr, out int _randID, int _startIndex = 0, int _lastIndex = -1, int _exceptIndex = -1)
        {
            if (_arr == null)
            {
                Debug_C.Error_Func("배열이 비어있습니다.");

                _randID = 0;

                return default;
            }
            else
            {
                if (1 < _arr.Length)
                {
                    if (_lastIndex == -1)
                        _lastIndex = _arr.Length;

                    _randID = UnityEngine.Random.Range(_startIndex, _lastIndex);

                    if(0 <= _exceptIndex)
                    {
                        if(_randID == _exceptIndex)
                        {
                            if (_randID + 1 < _lastIndex)
                                _randID++;
                            else
                                _randID = 0;
                        }
                    }

                    T _randItem = _arr[_randID];

                    return _randItem;
                }
                else
                {
                    _randID = 0;

                    return _arr[0];
                }
            }
        }
        public static T[] GetRandomPick_Func<T>(this T[] _arr, int _pickNum)
        {
            if (_pickNum <= _arr.Length)
            {
                T[] _valueTypeArr = new T[_pickNum];

                Cargold_Library.GetRandomPickNonAlloc_Func(_arr, _pickNum, _valueTypeArr);

                return _valueTypeArr;
            }
            else
            {
                Debug.LogError($"RandomPick 숫자에 비해 Array의 Item 개수가 부족합니다. (배열 크기/선택 개수 : {_arr.Length}/{_pickNum})");

                return null;
            }
        }
        public static void GetRandomPickNonAlloc_Func<T>(this T[] _arr, int _pickNum, T[] _pickedArr)
        {
            if (_pickNum <= _pickedArr.Length)
            {
                for (int i = 0; i < _pickNum; i++)
                {
                    int _randomPickIndex = UnityEngine.Random.Range(0, _arr.Length - i);
                    _pickedArr[i] = _arr[_randomPickIndex];

                    _arr.Swap_Func(_randomPickIndex, _arr.Length - i - 1);
                }
            }
            else
            {
                Debug.LogError($"RandomPick 숫자에 비해 Array의 Item 개수가 부족합니다. (배열 크기/선택 개수 : {_arr.Length}/{_pickNum})");
            }
        }
        public static bool SetSameLength_Func<T>(this T[] _arr, T[] _setArr)
        {
            if (_arr.Length == _setArr.Length)
            {
                for (int i = 0; i < _arr.Length; i++)
                    _arr[i] = _setArr[i];

                return true;
            }
            else
            {
                return false;
            }
        }

        // 이거 밸류 타입도 문제 없는지 확인 필요함
        public static void Swap_Func<T>(this T[] _arr, int _swapIndex1, int _swapIndex2)
        {
            if (_swapIndex1 != _swapIndex2)
            {
                if (_swapIndex1 < _arr.Length && _swapIndex2 < _arr.Length && 0 <= _swapIndex1 && 0 <= _swapIndex2)
                {
                    T _temp = _arr[_swapIndex1];
                    _arr[_swapIndex1] = _arr[_swapIndex2];
                    _arr[_swapIndex2] = _temp;
                }
                else
                {
                    Debug_C.Error_Func("Swap하려는 배열의 크기는 " + _arr.Length + ". 하지만 접근하려는 Index는 " + _swapIndex1 + ", 그리고 " + _swapIndex2);
                }
            }
        }
        public static void Sort_Func<T>(this T[] _arr, Func<T, T, bool> _rightIsRightDel)
        {
            System.Array.Sort(_arr, (_left, _right) =>
            {
                return _rightIsRightDel(_left, _right) == true ? 1 : -1;
            });
        }

        public static int GetIndex_Func<T>(this T[] _arr, T _targetItem, bool _isSearchAscending = true) where T : class
        {
            int _index = -1;

            if (_isSearchAscending == true)
            {
                for (int i = 0; i < _arr.Length; i++)
                {
                    if (_arr[i] == _targetItem)
                    {
                        _index = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = _arr.Length - 1; 0 <= i; i--)
                {
                    if (_arr[i] == _targetItem)
                    {
                        _index = i;
                        break;
                    }
                }
            }

            return _index;
        }
        public static bool TryGetItem_Func<T>(this T[] _arr, int _index, out T _item)
        {
            if(_arr != null && _index < _arr.Length && 0 <= _index)
            {
                _item = _arr[_index];

                return true;
            }
            else
            {
                _item = default;

                return false;
            }
        }
        #endregion
        #region Casting Group
        // String
        public static T ToEnum<T>(this string _value) where T : struct, System.Enum
        {
            if (_value.IsNullOrWhiteSpace_Func() == true)
                return default;

            return Enum_C.ToEnum_Func<T>(_value);
        }
        public static int ToInt(this string value, bool isCheckComma = true)
        {
            if (string.IsNullOrEmpty(value) == true) return 0;
            if(isCheckComma == true)
            {
                value = value.Replace(",", "");
            }

            return System.Int32.Parse(value);
        }
        public static float ToFloat(this string value)
        {
            if (string.IsNullOrEmpty(value) == true) return 0f;

            float returnValue = 0f;

            System.Single.TryParse(value, out returnValue);

            return returnValue;
        }
        public static Double ToDouble(this string value)
        {
            if (string.IsNullOrEmpty(value) == true) return 0d;

            double returnValue = 0d;

            System.Double.TryParse(value, out returnValue);

            return returnValue;
        }
        public static bool ToBool(this string value)
        {
            switch (value)
            {
                case "True":
                case "TRUE":
                case "T":
                case "1":
                    return true;

                default:
                    return false;
            }
        }
        public static Byte ToByte(this string value)
        {
            if (string.IsNullOrEmpty(value) == true) return 0;

            Byte returnValue = 0;

            System.Byte.TryParse(value, out returnValue);

            return returnValue;
        }

        // Enum
        public static int ToInt(this System.Enum value)
        {
            // 이거 GC 발생한다고 함

            object _returnValue = Convert.ChangeType(value, typeof(int));
            return (int)_returnValue;
        }

        // Float
        public static int GetPercent_Func(this float _value)
        {
            return (_value * 100f).ToInt();
        }
        public static string GetPercentStr_Func(this float _value, int _pointNumber = 0, bool _isContainPercent = true)
        {
            _value *= 100f;

            if (_isContainPercent == true)
            {
                string _valueArr = _value.ToString_Func(_pointNumber);
                return StringBuilder_C.Append_Func(_valueArr, StringBuilder_C.Percent);
            }
            else
            {
                return _value.ToString_Func(_pointNumber);
            }
        }
        public static int ToInt(this float _value, bool _isRound = false)
        {
            return _isRound == false ? (int)_value : Mathf.RoundToInt(_value);
        }
        public static int ToInt(this double _value, bool _isRound = false)
        {
            return _isRound == false ? (int)_value : (int)Math.Round(_value, 0);
        }
        private const string format0N0 = "{0:N0}";
        private const string format0N1 = "{0:N1}";
        private const string format0N2 = "{0:N2}";
        private const string format0N3 = "{0:N3}";
        private const string format0N4 = "{0:N4}";
        public static string ToString_Func(this float _value, int _pointNumber = 0)
        {
            if (0 < _pointNumber)
            {
                if (_pointNumber == 1)
                {
                    return string.Format(format0N1, _value);
                }
                else if (_pointNumber == 2)
                {
                    return string.Format(format0N2, _value);
                }
                else if (_pointNumber == 3)
                {
                    return string.Format(format0N3, _value);
                }
                else
                {
                    // 부동소수점의 오차범위
                    // 4자리수 넘어서까지 쓸 일 있으면 추가 바람

                    return string.Format(format0N4, _value);
                }
            }
            else
            {
                return string.Format(format0N0, _value);
            }
        }
        public static string ToString_Second_Func(this float _value, int _exPoint = 0)
        {
            string _timeStr = string.Empty;
            if (10f <= _value)
                _timeStr = _value.ToString_Func(0 + _exPoint);
            else if (1f <= _value)
                _timeStr = _value.ToString_Func(1 + _exPoint);
            else
                _timeStr = _value.ToString_Func(2 + _exPoint);

            return _timeStr;
        }
        public static string ToString_TensionTime_Func(this float _value)
        {
            return StringBuilder_C.GetTensionTime_Func(_value);
        }

        // Int
        private const string formatN0 = "N0";
        public static string ToString_Func(this int _value)
        {
            return _value.ToString(formatN0);
        }

        private const string FillZeroFormat = "D{0}";
        public static string ToString_Fill_Func(this int _value, int _fillZero)
        {
            string _fillZeroFormat = string.Format(FillZeroFormat, _fillZero);
            return _value.ToString(_fillZeroFormat);
        }
        #endregion
        #region Color Group
        public static Color GetGradientColor_Func(this Color _color, float _value)
        {
            float _rGap = 1f - ((1f - _color.r) * _value);
            float _gGap = 1f - ((1f - _color.g) * _value);
            float _bGap = 1f - ((1f - _color.b) * _value);

            return new Color(_rGap, _gGap, _bGap, 1f);
        }
        #endregion
        #region Collider
        public static void SetRadius_Func(this CircleCollider2D _circleCollider2D, SpriteRenderer _srdr, float _exSpace = 0f, bool _isLargest = true)
        {
            float _value = _isLargest == true ? Mathf.Max(_srdr.bounds.size.x, _srdr.bounds.size.y) : Mathf.Min(_srdr.bounds.size.x, _srdr.bounds.size.y);
            _circleCollider2D.radius = _value * 0.5f + _exSpace;
        }
        #endregion
        #region Component
        public static RectTransform GetRtrf_Func(this GameObject _gobj)
        {
            return _gobj.transform as RectTransform;
        }
        public static RectTransform GetRtrf_Func(this Component _component)
        {
            return _component.transform as RectTransform;
        }
        #endregion
        #region Dictionary Group
        public static bool IsHave_Func<K, V>(this Dictionary<K, V> _dic, int _minItemNum = 1)
        {
            return _dic != null && _minItemNum <= _dic.Keys.Count;
        }

        public static Dictionary<K, V> SetInstance_NoGC_EnumKey_Func<K, V>(IEqualityComparer<K> _iEqualityComparer)
        {
            return new Dictionary<K, V>(_iEqualityComparer);
        }
        public static void Add_Func<K, V>(this Dictionary<K, V> _dic, K _addKey, V _addValue)
        {
            // 오류 검출용

            if (_dic.ContainsKey(_addKey) == false)
            {
                _dic.Add(_addKey, _addValue);
            }
            else
            {
                Debug_C.Warning_Func("Dictionary에 다음 Key가 이미 존재합니다. : " + _addKey);
            }
        }
        public static void Remove_Func<K, V>(this Dictionary<K, V> _dic, K _removeKey)
        {
            // 오류 검출용

            if (_dic.ContainsKey(_removeKey) == true)
            {
                _dic.Remove(_removeKey);
            }
            else
            {
                Debug_C.Warning_Func("Dictionary에 지우려고 하는 다음 Key가 존재하지 않습니다. : " + _removeKey);
            }
        }
        public static bool TryTake_Func<K, V>(this Dictionary<K, V> _dic, K _key, out V _value)
        {
            // 오류 검출용

            if (_dic.TryGetValue(_key, out _value) == true)
            {
                _dic.Remove(_key);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetClearToValue_Func<K, V>(this Dictionary<K, V[]> _clearDic, V _clearValue)
        {
            // 함수의 Value 인자로 모두 채우기

            int _keyNum = _clearDic.Keys.Count;
            K[] _keyTypeArr = new K[_keyNum];
            _clearDic.Keys.CopyTo(_keyTypeArr, 0);

            for (int i = 0; i < _keyNum; i++)
            {
                K _keyType = _keyTypeArr[i];

                int _valueNum = _clearDic[_keyType].Length;
                for (int j = 0; j < _valueNum; j++)
                {
                    _clearDic[_keyType][j] = _clearValue;
                }
            }
        }

        public static V GetValue_Func<K, V>(this Dictionary<K, V> _dic, K _key)
        {
            return GetValue_Func(_dic, _key, null, out _);
        }
        /// <summary>
        /// Get 함수
        /// </summary>
        /// <param name="_generateDel">해당 Key가 없을 경우 이 콜백으로 값 생성한 뒤 딕셔너리에 Set하고서 반환한다.</param>
        /// <returns></returns>
        public static V GetValue_Func<K, V>(this Dictionary<K, V> _dic, K _key, Func<V> _generateDel)
        {
            return GetValue_Func<K, V>(_dic, _key, _generateDel, out _);
        }
        /// <summary>
        /// Get 함수
        /// </summary>
        /// <param name="_isInstance">생성 여부</param>
        /// <returns></returns>
        public static V GetValue_Func<K, V>(this Dictionary<K, V> _dic, K _key, Func<V> _generateDel, out bool _isInstance)
        {
#if UNITY_EDITOR
            if (_key == null)
            {
                Debug_C.Error_Func("Null Key : " + _key);

                _isInstance = false;

                return default;
            }
#endif

            if (_dic.TryGetValue(_key, out V _value) == false)
            {
                _isInstance = true;

                if(_generateDel != null)
                {
                    _value = _generateDel();
                    _dic.Add(_key, _value);
                }
                else
                {
#if UNITY_EDITOR
                    Debug_C.Error_Func($"딕셔너리에 다음 Key가 없음 : {_key} / 게다가 생성 콜백도 없음!");
#endif
                }
            }
            else
            {
                _isInstance = false;
            }

            return _value;
        }
        public static V[] GetValue_Func<K, V>(this Dictionary<K, V> _dic, params K[] _keyArr)
        {
            // Key에 해당하는 Value 반환
            // 오류 검출용
            // 인자 Key 중 중복Key가 있는지 검사하는 기능도 추가하자

            List<V> _list = new List<V>();

            for (int i = 0; i < _keyArr.Length; i++)
            {
                V _value = _dic.GetValue_Func(_keyArr[i]);
                _list.Add(_value);
            }

            return _list.ToArray();
        }
        public static V[] GetValue_Func<K, V>(this Dictionary<K, V> _dic)
        {
            // 딕셔너리의 모든 Value를 배열로 반환

            V[] _returnValueArr = new V[_dic.Values.Count];

            _dic.Values.CopyTo(_returnValueArr, 0);

            return _returnValueArr;
        }
        /// <summary>
        /// 값을 꺼낸 뒤 해당 Key는 Remove
        /// </summary>
        public static V GetTakeValue_Func<K, V>(this Dictionary<K, V> _dic, K _key)
        {
            V _value = _dic.GetValue_Func(_key);

            _dic.Remove(_key);

            return _value;
        }
        /// <summary>
        /// 값을 꺼낸 뒤 해당 Key는 Remove
        /// </summary>
        /// <param name="_generateDel">해당 Key가 없을 경우 반환할 값 생성할 콜백</param>
        /// <returns></returns>
        public static V GetTakeValue_Func<K, V>(this Dictionary<K, V> _dic, K _key, Func<V> _generateDel)
        {
            if (_dic.TryGetValue(_key, out V _value) == false)
            {
                _value = _generateDel();
            }
            else
            {
                _dic.Remove(_key);
            }

            return _value;
        }
        public static K[] GetKeys_Func<K, V>(this Dictionary<K, V> _dic)
        {
            // 딕셔너리에 입력된 모든 Key를 반환한다.

            K[] _keyTypeArr = new K[_dic.Keys.Count];
            _dic.Keys.CopyTo(_keyTypeArr, 0);
            return _keyTypeArr;
        }

        public static V ReplaceValue_Func<K, V>(this Dictionary<K, V> _dic, K _key, V _value)
        {
            // 신규 Value를 삽입하고 기존 Value는 Dictionary에서 제거 후 반환한다.

            V _originalValue = _dic.GetValue_Func(_key);
            _dic.Remove(_key);
            _dic.Add(_key, _value);
            return _originalValue;
        }
        #endregion
        #region List Group
        /// <summary>
        /// 배열에 아이템이 있는가?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_list"></param>
        /// <param name="_minItemNum">아이템 개수</param>
        /// <returns></returns>
        public static bool IsHave_Func<T>(this List<T> _list, int _minItemNum = 1)
        {
            return _list != null && _minItemNum <= _list.Count;
        }

        public static void AddNewItem_Func<ValueType>(this List<ValueType> _list, ValueType _addItem, bool _isErrorLog = false)
        {
            bool _isContain = _list.Contains(_addItem);

            if (_isContain == false)
            {
                _list.Add(_addItem);
            }
#if UNITY_EDITOR
            else
            {
                string _str = StringBuilder_C.Append_Func("이미 삽입되어 있는 Item을 중복해서 삽입하였습니다. : ", _addItem.ToString());
                if (_isErrorLog == false)
                    Debug_C.Warning_Func(_str);
                else
                    Debug_C.Error_Func(_str);
            }
#endif
        }
        public static void AddNewItem_Func<T>(this List<T> _list, T[] _addItemArr)
        {
            for (int i = 0; i < _addItemArr.Length; i++)
            {
                _list.AddNewItem_Func(_addItemArr[i]);
            }
        }
        public static void AddNewItem_Func<T>(this List<T> _list, List<T> _addItemList)
        {
            for (int i = 0; i < _addItemList.Count; i++)
            {
                _list.AddNewItem_Func(_addItemList[i]);
            }
        }
        public static void Add_Func<T>(this List<T> _list, T _addItem, bool _isNewWhenNull = true)
        {
            if (_list == null)
            {
                if (_isNewWhenNull == true)
                    _list = new List<T>();
                else
                {
                    Debug_C.Error_Func("List가 Null입니다.");
                    return;
                }
            }

            _list.Add(_addItem);
        }
        public static void Add_Func<T>(this List<T> _list, T[] _addItemArr, bool _isNewWhenNull = true)
        {
            if (_list == null)
            {
                if(_isNewWhenNull == true)
                    _list = new List<T>();
                else
                {
                    Debug_C.Error_Func("List가 Null입니다.");
                    return;
                }
            }

            for (int i = 0; i < _addItemArr.Length; i++)
            {
                _list.Add(_addItemArr[i]);
            }
        }
        public static void Add_Func<T>(this List<T> _list, List<T> _addItemList, bool _isNewWhenNull = true)
        {
            if (_list == null)
            {
                if (_isNewWhenNull == true)
                    _list = new List<T>();
                else
                {
                    Debug_C.Error_Func("?");
                    return;
                }
            }

            for (int i = 0; i < _addItemList.Count; i++)
            {
                _list.Add(_addItemList[i]);
            }
        }

        public static bool InsertNewItem_Func<T>(this List<T> _list, int _id, T _addItem)
        {
            bool _isContain = _list.Contains(_addItem);

            if (_isContain == false)
            {

            }
            else
            {
                Debug.LogWarning("이미 삽입되어 있는 Item을 중복해서 삽입하였습니다. : " + _addItem);
            }

            _list.Insert(_id, _addItem);

            return _isContain;
        }
        public static bool TryRemove_Func<T>(this List<T> _list, T _removeItem)
        {
            if (_list.Contains(_removeItem) == true)
            {
                _list.Remove(_removeItem);

                return true;
            }
            else
            {
                Debug_C.Warning_Func("존재하지 않는 Item을 Remove하고자 합니다. : " + _removeItem);

                return false;
            }
        }
        public static void Remove_Func<T>(this List<T> _list, T _removeItem)
        {
            Cargold_Library.TryRemove_Func(_list, _removeItem);
        }
        public static void RemoveLast_Func<T>(this List<T> _list)
        {
            if(_list == null)
            {
                Debug_C.Error_Func("?");
                return;
            }

            if(0 < _list.Count)
                _list.RemoveAt(_list.Count - 1);
        }
        // 이거 밸류 타입도 문제 없는지 확인 필요함
        public static void Swap_Func<T>(this List<T> _list, int _swapIndex1, int _swapIndex2)
        {
            if (_swapIndex1 != _swapIndex2)
            {
                if (_swapIndex1 < _list.Count && _swapIndex2 < _list.Count && 0 <= _swapIndex1 && 0 <= _swapIndex2)
                {
                    T _temp = _list[_swapIndex1];
                    _list[_swapIndex1] = _list[_swapIndex2];
                    _list[_swapIndex2] = _temp;
                }
                else
                {
                    Debug_C.Error_Func("Swap하려는 배열의 크기는 " + _list.Count + ". 하지만 접근하려는 Index는 " + _swapIndex1 + ", 그리고 " + _swapIndex2);
                }
            }
        }
        public static void Swap_Func<T>(this List<T> _list, T _originItem, T _newItem)
        {
            if (_list.Contains(_originItem) == false)
            {
                Debug_C.Error_Func("리스트에 " + _originItem + " 없음");
                return;
            }

            int _id = _list.IndexOf(_originItem);
            _list.Swap_Func(_id, _newItem);
        }
        public static void Swap_Func<T>(this List<T> _list, int _swapID, T _insertItem)
        {
            _list.RemoveAt(_swapID);

            _list.Insert(_swapID, _insertItem);
        }

        /// <summary>
        /// True일 시 우측 인자가 앞으로 정렬
        /// </summary>
        public static void Sort_Func<T>(this List<T> _list, Func<T, T, bool> _rightIsRightDel)
        {
            _list.Sort((_left, _right) =>
            {
                return _rightIsRightDel(_left, _right) == true ? 1 : -1;
            });
        }

        public static ValueType[] GetRandomPick_Func<ValueType>(this List<ValueType> _list, int _pickNum)
        {
            // ToArray()의 퍼포먼스를 고려해서 사용할 것!
            return _list.ToArray().GetRandomPick_Func(_pickNum);
        }
        public static ValueType GetLastItem_Func<ValueType>(this List<ValueType> _list)
        {
            // 리스트의 마지막 아이템 반환

            int _count = _list.Count;
            return 0 < _count ? _list[_count - 1] : default;
        }
        public static ValueType GetHalfItem_Func<ValueType>(this List<ValueType> _list)
        {
            // 리스트의 중간에 배치된 아이템 반환

            int _listNum = _list.Count;

            int _halfID = _listNum / 2;

            ValueType _halfItem = _list[_halfID];

            return _halfItem;
        }
        public static ValueType GetRandItem_Func<ValueType>(this List<ValueType> _list)
        {
            return GetRandItem_Func<ValueType>(_list, out _);
        }
        public static ValueType GetRandItem_Func<ValueType>(this List<ValueType> _list, out int _randID)
        {
#if UNITY_EDITOR
            if (_list.IsHave_Func() == false)
            {
                Debug_C.Error_Func("?");
                _randID = -1;
                return default;
            }
#endif

            int _cnt = _list.Count;
            _randID = UnityEngine.Random.Range(0, _cnt);
            return _list[_randID];
        }
        public static ValueType GetItem_Func<ValueType>(this List<ValueType> _list, int _id, Func<int, ValueType> _instanceDel)
        {
            if(_list.TryGetItem_Func(_id, out ValueType _item) == false)
            {
                while (_list.Count <= _id)
                {
                    _item = _instanceDel(_list.Count);
                    _list.Add(_item);
                }
            }

            return _item;
        }

        public static ValueType GetTakeItem_Func<ValueType>(this List<ValueType> _list, int _index = 0)
        {
            ValueType _item = _list[_index];

            _list.RemoveAt(_index);

            return _item;
        }
        public static ValueType GetTakeLastItem_Func<ValueType>(this List<ValueType> _list)
        {
            int _id = _list.Count - 1;
            ValueType _item = _list[_id];

            _list.RemoveAt(_id);

            return _item;
        }
        public static bool TryGetTakeLastItem_Func<T>(this List<T> _list, out T _item)
        {
            if(_list.IsHave_Func() == true)
            {
                _item = _list.GetTakeLastItem_Func();

                return true;
            }
            else
            {
                _item = default;

                return false;
            }
        }
        public static bool TryGetItem_Func<ValueType>(this List<ValueType> _list, int _index, out ValueType _item)
        {
            if (_list != null && _index < _list.Count && 0 <= _index)
            {
                _item = _list[_index];

                return true;
            }
            else
            {
                _item = default;

                return false;
            }
        }
        #endregion
        #region Transform Group
        public static void SetDefault_Func(this Transform _thisTrf)
        {
            _thisTrf.localPosition = Vector3.zero;
            _thisTrf.rotation = Quaternion.identity;
            _thisTrf.localScale = Vector3.one;
        }

        public static void LookAt_Func(this Transform _thisTrf, Transform _targetTrf)
        {
            _thisTrf.LookAt_Func(_targetTrf.position);
        }
        public static void LookAt_Func(this Transform _thisTrf, Vector2 _targetPos)
        {
            _thisTrf.rotation = Math_C.GetLookAt_Func(_thisTrf.position, _targetPos);
        }
        public static float GetAngle_Func(this Transform _thisTrf, Vector2 _targetPos, bool _isRelativeToRotate = false)
        {
            return _thisTrf.position.GetAngle_Func(_targetPos, _isRelativeToRotate);
        }
        public static float GetAngle_Func(this Transform _thisTrf)
        {
            float _angle = _thisTrf.eulerAngles.z * -1f;
            return Math_C.Case_Func(_angle, 0f, 360f);
        }

        /// <summary>
        /// TargetPos를 바라보는데 필요한 회전 방향
        /// </summary>
        public static Vector3 GetRotateDir_Func(this Transform _thisTrf, Vector2 _targetPos)
        {
            return Math_C.GetRotateDir_Func(_thisTrf, _targetPos);
        }

        public static void SetPos_Func(this Transform _thisTrf, float _posX, float _posY, UnityEngine.Space _space)
        {
            SetPos_Func(_thisTrf, new Vector2(_posX, _posY), _space);
        }
        public static void SetPos_Func(this Transform _thisTrf, Vector3 _pos, UnityEngine.Space _space)
        {
            if (_space == Space.World)
            {
                _thisTrf.position = _pos;
            }
            else
            {
                _thisTrf.localPosition = _pos;
            }
        }
        public static void SetPosX_Func(this Transform _thisTrf, float _value, UnityEngine.Space _space)
        {
            if (_space == Space.World)
            {
                _thisTrf.position = new Vector3(_value, _thisTrf.position.y, _thisTrf.position.z);
            }
            else
            {
                _thisTrf.localPosition = new Vector3(_value, _thisTrf.localPosition.y, _thisTrf.localPosition.z);
            }
        }
        public static void SetPosY_Func(this Transform _thisTrf, float _value, UnityEngine.Space _space = Space.World)
        {
            if (_space == Space.World)
            {
                _thisTrf.position = new Vector3(_thisTrf.position.x, _value, _thisTrf.position.z);
            }
            else
            {
                _thisTrf.localPosition = new Vector3(_thisTrf.localPosition.x, _value, _thisTrf.localPosition.z);
            }
        }
        public static void SetPosZ_Func(this Transform _thisTrf, float _value, UnityEngine.Space _space)
        {
            if (_space == Space.World)
            {
                _thisTrf.position = new Vector3(_thisTrf.position.x, _thisTrf.position.y, _value);
            }
            else
            {
                _thisTrf.localPosition = new Vector3(_thisTrf.localPosition.x, _thisTrf.localPosition.y, _value);
            }
        }

        public static void SetRotX_Func(this Transform _thisTrf, float _value, UnityEngine.Space _space = Space.World)
        {
            if (_space == Space.World)
                _thisTrf.eulerAngles = Vector3.right * _value;
            else
                _thisTrf.localEulerAngles = Vector3.right * _value;
        }
        public static void SetRotZ_Func(this Transform _thisTrf, float _value, UnityEngine.Space _space = Space.World)
        {
            if (_space == Space.World)
                _thisTrf.eulerAngles = Vector3.forward * _value;
            else
                _thisTrf.localEulerAngles = Vector3.forward * _value;
        }

        public static void SetScale_Func(this Transform _thisTrf, float _value)
        {
            _thisTrf.localScale = Vector3.one * _value;
        }
        public static void SetScaleUp_Func(this Transform _thisTrf, float _value)
        {
            _thisTrf.localScale = new Vector3(_thisTrf.localScale.x, _value, _thisTrf.localScale.z);
        }
        public static void SetScaleRight_Func(this Transform _thisTrf, float _value)
        {
            _thisTrf.localScale = new Vector3(_value, _thisTrf.localScale.y, _thisTrf.localScale.z);
        }

        public static float GetDistance_Func(this Transform _thisTrf, Transform _targetTrf)
        {
            return _thisTrf.GetDistance_Func(_targetTrf.position);
        }
        public static float GetDistance_Func(this Transform _thisTrf, Vector2 _targetPos)
        {
            return Vector2.Distance(_thisTrf.position, _targetPos);
        }

        /// <summary>
        /// 트랜스폼의 히어라키 전체 경로를 문자열로 반환합니다.
        /// </summary>
        public static string GetPath_Func(this Transform _trf)
        {
            string _pathStr = _trf.name;

            _trf = _trf.parent;

            while (_trf is null == false)
            {
                _pathStr = StringBuilder_C.Append_Func(_trf.name, " / ", _pathStr);

                _trf = _trf.parent;
            }

            return _pathStr;
        }
        /// <summary>
        /// 트랜스폼의 히어라키 전체 경로를 콘솔로그에 출력합니다.
        /// </summary>
        public static void OnLogPath_Func(this Transform _trf, LogType _logType = LogType.Log)
        {
            switch (_logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    Debug_C.Error_Func(GetPath_Func(_trf));
                    break;

                case LogType.Warning:
                    Debug_C.Warning_Func(GetPath_Func(_trf));
                    break;

                case LogType.Assert:
                case LogType.Log:
                default:
                    Debug_C.Log_Func(GetPath_Func(_trf));
                    break;
            }

        }

        public static void SetParent_Func(this Transform _trf, Transform _parentTrf)
        {
            _trf.SetParent_Func(_parentTrf, Vector3.zero, Vector3.one);
        }
        public static void SetParent_Func(this Transform _trf, Transform _parentTrf, Vector3 _localPos, Vector3 _localScale)
        {
            _trf.SetParent(_parentTrf);
            _trf.localPosition = _localPos;
            _trf.localScale = _localScale;
        }

        public static bool TryFind_Func(this Transform _trf, string _childNameStr, out Transform _childTrf)
        {
            _childTrf = _trf.Find(_childNameStr);
            return _childTrf != null;
        }
        #endregion
        #region UGUI Group
        public static void SetFade_Func(this SpriteRenderer _spriteRend, float _alphaValue)
        {
            Color _returnColor = _spriteRend.color;

            _spriteRend.color = GetNaturalAlphaColor_Func(_returnColor, _alphaValue);
        }
        public static void SetFade_Func(this Image _image, float _alphaValue)
        {
            Color _returnColor = _image.color;

            _image.color = GetNaturalAlphaColor_Func(_returnColor, _alphaValue);
        }
        public static void SetFade_Func(this UnityEngine.UI.Text _text, float _alphaValue)
        {
            Color _returnColor = _text.color;

            _text.color = GetNaturalAlphaColor_Func(_returnColor, _alphaValue);
        }
        public static void SetFade_Func(this Graphic _graphic, float _alphaValue)
        {
            Color _returnColor = _graphic.color;

            _graphic.color = GetNaturalAlphaColor_Func(_returnColor, _alphaValue);
        }

        public static Color GetNaturalAlphaColor_Func(this Color _color, float _alphaValue)
        {
            Color _returnColor = new Color
                (
                _color.r,
                _color.g,
                _color.b,
                _alphaValue
                );

            return _returnColor;
        }
        #region Image
        public static void SetColorOnBaseAlpha_Func(this Image _image, Color _setColor)
        {
            _setColor = new Color
                (
                _setColor.r,
                _setColor.g,
                _setColor.b,
                _image.color.a
                );

            _image.color = _setColor;
        }
        public static void SetNativeSize_Func(this Image _image, Sprite _sprite)
        {
            _image.sprite = _sprite;
            _image.SetNativeSize();
        }

        public static void FillAmount_Func(this Image _image, int _setValue, int _maxValue)
        {
            _image.FillAmount_Func((float)_setValue, (float)_maxValue);
        }
        public static void FillAmount_Func(this Image _image, float _setValue, int _maxValue)
        {
            _image.FillAmount_Func(_setValue, (float)_maxValue);
        }
        public static void FillAmount_Func(this Image _image, int _setValue, float _maxValue)
        {
            _image.FillAmount_Func((float)_setValue, _maxValue);
        }
        public static void FillAmount_Func(this Image _image, float _setValue, float _maxValue)
        {
            if (0f < _setValue && 0f < _maxValue)
                _image.fillAmount = _setValue / _maxValue;
            else
                _image.fillAmount = 0f;
        }
        public static void SetGrayScale_Func(this Image _image)
        {
            _image.material = Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.GetGrayMat;
        }
        #endregion

        #region Text
        public static void SetText_Func(this UnityEngine.UI.Text _txt, int _value)
        {
            _txt.text = _value.ToString();
        }
        public static void SetText_Func(this UnityEngine.UI.Text _txt, float _value, int _digitNum = -1)
        {
            if (_digitNum == -1)
            {
                _txt.text = _value.ToString();
            }
            else
            {
                _txt.text = _value.ToString_Func(_digitNum);
            }
        }
        #endregion

        #region Tmp
        public static void SetText_Func(this TMPro.TextMeshProUGUI _tmp, int _value, bool _isContainComma = true)
        {
            _tmp.text = _isContainComma == true ? _value.ToString_Func() : _value.ToString();
        }
        public static void SetText_Func(this TMPro.TextMeshPro _tmp, int _value)
        {
            _tmp.text = _value.ToString();
        } 
        public static void SetGradientColor_Func(this TMPro.TMP_Text _tmp, Color _color, Cargold.DirectionType _directionType)
        {
            TMPro.VertexGradient _vertexGradient = _tmp.colorGradient;

            switch (_directionType)
            {
                case DirectionType.Left:
                    {
                        _vertexGradient.bottomLeft = _color;
                        _vertexGradient.topLeft = _color;
                    }
                    break;

                case DirectionType.Down:
                    {
                        _vertexGradient.bottomLeft = _color;
                        _vertexGradient.bottomRight = _color;
                    }
                    break;

                case DirectionType.Up:
                    {
                        _vertexGradient.topLeft = _color;
                        _vertexGradient.topRight = _color;
                    }
                    break;

                case DirectionType.Right:
                    {
                        _vertexGradient.bottomRight = _color;
                        _vertexGradient.topRight = _color;
                    }
                    break;

                default:
                    Debug_C.Error_Func("_directionType : " + _directionType);
                    break;
            }

            _tmp.colorGradient = _vertexGradient;
        }
        public static void SetFontSize_Func(this TMPro.TMP_Text _tmp, float _min, float _max)
        {
            _tmp.fontSizeMin = _min;
            _tmp.fontSizeMax = _max;
        }
        #endregion

        #region Scroll Rtrf Height Dynamic
        public static float GetScrollRtrfHeightDynamic_Func(this GridLayoutGroup _gridLayoutGroup, int _elemNum)
        {
            return GetScrollRtrfHeightDynamic_Func(_gridLayoutGroup, _elemNum, out _);
        }
        public static float GetScrollRtrfHeightDynamic_Func(this GridLayoutGroup _gridLayoutGroup, int _elemNum, out int _rowNum)
        {
#if UNITY_EDITOR
            // _gridLayoutGroup x 피벗이 0.5이어야 함
            RectTransform _gridRtrf = _gridLayoutGroup.transform as RectTransform;
            if (_gridRtrf.pivot.x != 0.5f)
            {
                _gridRtrf.pivot = new Vector2(0.5f, _gridRtrf.pivot.y);

                Debug_C.Warning_Func(_gridLayoutGroup + "의 피벗을 0.5f로 강제했습니다.");
            }
#endif

            float _scrollWidthSize = _gridLayoutGroup.transform.localPosition.x * 2f;
            float _elemWidth = _gridLayoutGroup.cellSize.x + _gridLayoutGroup.spacing.x;
            int _columnNum = (int)(_scrollWidthSize / _elemWidth);
            float _elemHeight = _gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y;

            return GetScrollRtrfHeightDynamic_Func(_elemNum, _columnNum, _elemHeight, out _rowNum);
        }
        public static float GetScrollRtrfHeightDynamic_Func(int _elemNum, int _columnNum, float _elemHeight, out int _rowNum)
        {
            if (1 <= _columnNum)
            {
                _rowNum = _elemNum / _columnNum;
                if (_elemNum % _columnNum != 0)
                    ++_rowNum;
            }
            else
            {
                _rowNum = _elemNum;
            }

            return _elemHeight * _rowNum;
        }

        public static void SetScrollRtrfHeightDynamic_Func(this GridLayoutGroup _thisGrid, ICollection _elemCollection, float _moreUnderSpaceY = 100f)
        {
            int _cnt = _elemCollection.Count;
            SetScrollRtrfHeightDynamic_Func(_thisGrid, _cnt, _moreUnderSpaceY);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this GridLayoutGroup _thisGrid, int _elemNum, float _moreUnderSpaceY = 100f)
        {
            RectTransform _thisRtrf = _thisGrid.transform as RectTransform;
            SetScrollRtrfHeightDynamic_Func(_thisRtrf, _elemNum, _thisGrid, out _, _moreUnderSpaceY);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this RectTransform _scrollRtrf,
            int _elemNum, GridLayoutGroup _gridLayoutGroup, out float _scrollContentSizeY, float _moreUnderSpaceY = 0f)
        {
            _scrollContentSizeY = _gridLayoutGroup.GetScrollRtrfHeightDynamic_Func(_elemNum, out _);

            SetDynamicSize_Func(_scrollRtrf, _scrollContentSizeY + _moreUnderSpaceY, DirectionType.Down);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this RectTransform _scrollRtrf,
            int _elemNum, GridLayoutGroup _gridLayoutGroup, out int _rowNum, out float _scrollContentSizeY, float _moreUnderSpaceY = 100f)
        {
            _scrollContentSizeY = _gridLayoutGroup.GetScrollRtrfHeightDynamic_Func(_elemNum, out _rowNum);

            SetDynamicSize_Func(_scrollRtrf, _scrollContentSizeY + _moreUnderSpaceY, DirectionType.Down);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this RectTransform _scrollRtrf,
            int _elemNum, float _scrollWidthSize, float _elemWidth, float _elemHeight, out int _rowNum, out float _scrollContentSizeY, float _moreUnderSpaceY = 100f)
        {
            int _columnNum = (int)(_scrollWidthSize / _elemWidth);
            SetScrollRtrfHeightDynamic_Func(_scrollRtrf, _elemNum, _elemHeight, _moreUnderSpaceY, _columnNum, out _rowNum, out _scrollContentSizeY);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this VerticalLayoutGroup _verticalLayoutGroup, int _elemNum, float _elemHeight)
        {
            RectTransform _scrollRtrf = _verticalLayoutGroup.GetRtrf_Func();
            float _moreSpaceY = _verticalLayoutGroup.padding.top + _verticalLayoutGroup.padding.bottom;
            _moreSpaceY += _verticalLayoutGroup.spacing * (_elemNum - 1);
            SetScrollRtrfHeightDynamic_Func(_scrollRtrf, _elemNum, _elemHeight, _moreSpaceY, 1, out _, out _);
        }
        public static void SetScrollRtrfHeightDynamic_Func(this RectTransform _scrollRtrf,
            int _elemNum, float _elemHeight, float _moreUnderSpaceY, int _columnNum, out int _rowNum, out float _scrollContentSizeY)
        {
            _scrollContentSizeY = GetScrollRtrfHeightDynamic_Func(_elemNum, _columnNum, _elemHeight, out _rowNum);
            SetDynamicSize_Func(_scrollRtrf, _scrollContentSizeY + _moreUnderSpaceY, DirectionType.Down);
        }
        #endregion

        public static void SetScrollValue_Func(this ScrollRect _scrollRect, ScrollRectDir _dir = ScrollRectDir.Vertical)
        {
            SetScrollValue_Func(_scrollRect, 1f, _dir);
        }
        public static void SetScrollValue_Func(this ScrollRect _scrollRect, float _value, ScrollRectDir _dir = ScrollRectDir.Vertical)
        {
            if (_dir == ScrollRectDir.Vertical)
                _scrollRect.verticalNormalizedPosition = _value; // 1f이 위, 0f이 아래
            else if (_dir == ScrollRectDir.Horizontal)
                _scrollRect.horizontalNormalizedPosition = _value;
            else
                Debug_C.Error_Func("_dir : " + _dir);
        }
        public enum ScrollRectDir
        {
            None = 0,

            Vertical,
            Horizontal,
        }

        #region RectTransform
        public static void SetSizeDeltaX_Func(this RectTransform _scrollRtrf, float _x)
        {
            _scrollRtrf.sizeDelta = new Vector2(_x, _scrollRtrf.sizeDelta.y);
        }
        public static void SetSizeDeltaY_Func(this RectTransform _scrollRtrf, float _y)
        {
            _scrollRtrf.sizeDelta = new Vector2(_scrollRtrf.sizeDelta.x, _y);
        }
        public static void SetDynamicSize_Func(this RectTransform _scrollRtrf, float _sizeValue, DirectionType _stretchDirType)
        {
            // Rtrf의 앵커가 아래와 같이 세팅돼야 함
            // _scrollRtrf.anchorMin = Vector2.up;
            // _scrollRtrf.anchorMax = Vector2.one;

            Vector2 _anchorMin = default;
            Vector2 _anchorMax = default;
            Vector2 _pivot = default;
            Vector2 _sizeDir = default;
            switch (_stretchDirType)
            {
                case DirectionType.Left:
                    {
                        _anchorMin = Vector2.right;
                        _anchorMax = Vector2.one;

                        _pivot = new Vector2(1f, _scrollRtrf.pivot.y);

                        _sizeDir = Vector2.right;
                    }
                    break;

                case DirectionType.Down:
                    {
                        _anchorMin = Vector2.up;
                        _anchorMax = Vector2.one;

                        _pivot = new Vector2(_scrollRtrf.pivot.x, 1f);

                        _sizeDir = Vector2.up;
                    }
                    break;

                case DirectionType.Up:
                    {
                        _anchorMin = Vector2.zero;
                        _anchorMax = Vector2.right;

                        _pivot = new Vector2(_scrollRtrf.pivot.x, 0f);

                        _sizeDir = Vector2.up;
                    }
                    break;

                case DirectionType.Right:
                    {
                        _anchorMin = Vector2.zero;
                        _anchorMax = Vector2.up;

                        _pivot = new Vector2(0f, _scrollRtrf.pivot.y);

                        _sizeDir = Vector2.right;
                    }
                    break;

                default:
                    Debug_C.Error_Func("_stretchDirType : " + _stretchDirType);
                    return;
            }

            if (_scrollRtrf.anchorMin != _anchorMin)
            {
                _scrollRtrf.anchorMin = _anchorMin;

                Debug_C.Warning_Func(StringBuilder_C.Append_Func(_scrollRtrf.GetPath_Func(), "의 anchorMin을 ", _anchorMin.ToString(), "으로 강제했습니다."));
            }

            if (_scrollRtrf.anchorMax != _anchorMax)
            {
                _scrollRtrf.anchorMax = _anchorMax;

                Debug_C.Warning_Func(StringBuilder_C.Append_Func(_scrollRtrf.GetPath_Func(), "의 anchorMax을 ", _anchorMax.ToString(), "으로 강제했습니다."));
            }

            if(_scrollRtrf.pivot != _pivot)
            {
                _scrollRtrf.pivot = _pivot;

                Debug_C.Warning_Func(StringBuilder_C.Append_Func(_scrollRtrf.GetPath_Func(), " 의 Pivot을 ", _pivot.ToString(), "으로 강제했습니다."));
            }

            _scrollRtrf.anchoredPosition = Vector2.zero;
            _scrollRtrf.sizeDelta = _sizeDir * _sizeValue;
        }
        public static void SetHeight_Func(this RectTransform _scrollRtrf, float _height, DirectionType _stretchDirType = DirectionType.Down)
        {
            // Rtrf의 앵커가 아래와 같이 세팅돼야 함
            // _scrollRtrf.anchorMin = Vector2.up;
            // _scrollRtrf.anchorMax = Vector2.one;

            Vector2 _anchorMin = default;
            Vector2 _anchorMax = default;
            if (_stretchDirType == DirectionType.Down)
            {
                _anchorMin = Vector2.up;
                _anchorMax = Vector2.one;
            }
            else if(_stretchDirType == DirectionType.Up)
            {

            }
            else
            {
                Debug_C.Error_Func("_stretchDirType : " + _stretchDirType);
                return;
            }

            if (_scrollRtrf.anchorMin != _anchorMin)
            {
                _scrollRtrf.anchorMin = _anchorMin;

                Debug_C.Warning_Func(_scrollRtrf + "의 anchorMin을 Vector.up으로 강제했습니다.");
            }

            if (_scrollRtrf.anchorMax != _anchorMax)
            {
                _scrollRtrf.anchorMax = _anchorMax;

                Debug_C.Warning_Func(_scrollRtrf + "의 anchorMax을 Vector.one으로 강제했습니다.");
            }

            _scrollRtrf.anchoredPosition = Vector2.zero;
            _scrollRtrf.sizeDelta = Vector2.up * _height;
        }
        public static void SetStretch_Func(this RectTransform _rtrf, bool _isScaleReset = true)
        {
            _rtrf.localPosition = Vector3.zero;
            _rtrf.anchorMin = Vector2.zero;
            _rtrf.anchorMax = Vector2.one;
            _rtrf.anchoredPosition = Vector2.zero;
            _rtrf.sizeDelta = Vector2.zero;

            if (_isScaleReset == true)
                _rtrf.localScale = Vector3.one;
        }
        public static void SetAnchorX_Func(this RectTransform _rtrf, float _x)
        {
            _rtrf.anchorMin = new Vector2(_x, _rtrf.anchorMin.y);
            _rtrf.anchorMax = new Vector2(_x, _rtrf.anchorMax.y);
        }
        public static void SetAnchorY_Func(this RectTransform _rtrf, float _y)
        {
            _rtrf.anchorMin = new Vector2(_rtrf.anchorMin.x, _y);
            _rtrf.anchorMax = new Vector2(_rtrf.anchorMax.x, _y);
        }
        public static void SetAnchor_Func(this RectTransform _rtrf, Pivot _pivot)
        {
            float _minX = 0f, _minY = 0f, _maxX = 0f, _maxY = 0f;

            switch (_pivot)
            {
                case Pivot.TopLeft:
                    {
                        _minX = 0f; _minY = 1f;
                        _maxX = 0f; _maxY = 1f;
                    }
                    break;

                case Pivot.Top:
                    {
                        _minX = .5f; _minY = 1f;
                        _maxX = .5f; _maxY = 1f;
                    }
                    break;

                case Pivot.TopRight:
                    {
                        _minX = 1f; _minY = 1f;
                        _maxX = 1f; _maxY = 1f;
                    }
                    break;

                case Pivot.Left:
                    {
                        _minX = 0f; _minY = .5f;
                        _maxX = 0f; _maxY = .5f;
                    }
                    break;

                case Pivot.Center:
                    {
                        _minX = .5f; _minY = .5f;
                        _maxX = .5f; _maxY = .5f;
                    }
                    break;

                case Pivot.Right:
                    {
                        _minX = 1f; _minY = .5f;
                        _maxX = 1f; _maxY = .5f;
                    }
                    break;

                case Pivot.BottomLeft:
                    {
                        _minX = 0f; _minY = 0f;
                        _maxX = 0f; _maxY = 0f;
                    }
                    break;
                case Pivot.Bottom:
                    {
                        _minX = .5f; _minY = 0f;
                        _maxX = .5f; _maxY = 0f;
                    }
                    break;
                case Pivot.BottomRight:
                    {
                        _minX = 1f; _minY = 0f;
                        _maxX = 1f; _maxY = 0f;
                    }
                    break;

                case Pivot.None:
                default:
                    Debug_C.Error_Func("_pivot : " + _pivot);
                    break;
            }

            SetAnchor_Func(_rtrf, _minX, _minY, _maxX, _maxY);
        }
        public static void SetAnchor_Func(this RectTransform _rtrf, float _minX, float _minY, float _maxX, float _maxY)
        {
            _rtrf.anchorMin = new Vector2(_minX, _minY);
            _rtrf.anchorMax = new Vector2(_maxX, _maxY);
        }
        public static void SetAnchor_Func(this RectTransform _rtrf, Vector2 _min, Vector2 _max)
        {
            _rtrf.anchorMin = _min;
            _rtrf.anchorMax = _max;
        }
        public static void SetAnchorPosX_Func(this RectTransform _rtrf, float _x)
        {
            _rtrf.anchoredPosition = new Vector2(_x, _rtrf.anchoredPosition.y);
        }
        public static void SetAnchorPosY_Func(this RectTransform _rtrf, float _y)
        {
            _rtrf.anchoredPosition = new Vector2(_rtrf.anchoredPosition.x, _y);
        }

        public enum Pivot
        {
            None = 0,

            TopLeft, Top, TopRight,
            Left, Center, Right,
            BottomLeft, Bottom, BottomRight,
        } 
        #endregion
        #endregion
        #region Data Structure
        public static bool HasItem_Func<T>(this Queue<T> _queue)
        {
            return 0 < _queue.Count ? true : false;
        }
        public static bool TryDequeue_Func<T>(this Queue<T> _queue, out T _tryGet)
        {
            bool _isHave = false;

            if (0 < _queue.Count)
            {
                _isHave = true;

                _tryGet = _queue.Dequeue();
            }
            else
            {
                _isHave = false;

                _tryGet = default(T);
            }

            return _isHave;
        }
        #endregion
        #region DateTime
        public static DateTime GetMidNight_Func(this DateTime _dateTime)
        {
            _dateTime = _dateTime.AddDays(1);
            return new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, 0, 0, 0);
        }
        public static DateTime GetNextDayOfWeek_Func(this DateTime _dateTime, DayOfWeek _targetDayOfWeek)
        {
            DayOfWeek _nowDayOfWeek = _dateTime.DayOfWeek;

            int _nextDay = -1;
            _nextDay = _targetDayOfWeek - _nowDayOfWeek;
            if (_targetDayOfWeek <= _nowDayOfWeek)
                _nextDay += 7;

            _dateTime = _dateTime.AddDays(_nextDay);
            return new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, 0, 0, 0);
        }
        public static DateTime GetNextBeginningOfMonth_Func(this DateTime _dateTime)
        {
            _dateTime = _dateTime.AddMonths(1);
            return new DateTime(_dateTime.Year, _dateTime.Month, 1, 0, 0, 0);
        }
        public static DateTime GetAfterDay_Func(this DateTime _dateTime, int _days, bool _isMidnight = false)
        {
            DateTime _afterTime = _dateTime.AddDays(_days);
            return _isMidnight == false ? _afterTime : _afterTime.GetMidNight_Func();
        }
        public static double GetTotalSec_Func(this DateTime _dateTime)
        {
            TimeSpan _timeSpan = new TimeSpan(_dateTime.Ticks);
            return _timeSpan.TotalSeconds;
        }
        public static double GetTotalMin_Func(this DateTime _dateTime)
        {
            TimeSpan _timeSpan = new TimeSpan(_dateTime.Ticks);
            return _timeSpan.TotalMinutes;
        }
        public static TimeSpan GetTimeSpan_Func(this DateTime _dateTime)
        {
            return new TimeSpan(_dateTime.Ticks);
        }
        public static bool IsPassedDay_Func(this DateTime _dateTime, DateTime _targetTime)
        {
            if(_targetTime <= _dateTime)
                return false;

            if (_dateTime.Day < _targetTime.Day ||
                _dateTime.Month < _targetTime.Month ||
                _dateTime.Year < _targetTime.Year)
                return true;

            return false;
        }

        // TimeSpan
        public static string ToString_HMS_Func(this TimeSpan _value, bool _isIncludeHour = true, bool _isIncludeMilliSecond = false)
        {
            DateTime _dateTime = _value.ToDateTime_Func();
            return _dateTime.ToString_HMS_Func(_isIncludeMilliSecond: _isIncludeMilliSecond, _isIncludeHour: _isIncludeHour);
        }
        public static DateTime ToDateTime_Func(this TimeSpan _value)
        {
            return new DateTime(_value.Ticks);
        }

        // Common
        public static string ToString_HMS_Func(this DateTime _value, bool _isIncludeMilliSecond = false, bool _isIncludeHour = true)
        {
            return ToString_Time_Func(_value, false, _isIncludeMilliSecond, _isIncludeHour, default);
        }
        public static string ToString_MS_Func(this DateTime _value, bool _isIncludeMilliSecond = false, bool _isIncludeSecondToMinute = false)
        {
            return ToString_Time_Func(_value, true, _isIncludeMilliSecond, default, _isIncludeSecondToMinute);
        }

        private const string formatHMSms = "{0:00}:{1:00}:{2:00}.{3:00}";
        private const string formatMSms = "{0:00}:{1:00}.{2:00}";
        private const string formatHMS = "{0:00}:{1:00}:{2:00}";
        private const string formatMS = "{0:00}:{1:00}";
        private const string formatSms = "{0:00}.{1:00}";
        private const string formatS = "{0:00}";
        private static string ToString_Time_Func(DateTime _value, bool _isIncludeMinuteToHour, bool _isIncludeMilliSecond, bool _isIncludeHour, bool _isIncludeSecondToMinute)
        {
            int _hour = _value.Hour;
            int _minute = _value.Minute;
            int _second = _value.Second;
            int _miSec = _value.Millisecond;

            // 시를 분에 포함 안 하나?
            if (_isIncludeMinuteToHour == false)
            {
                // 밀리초를 포함 안 하나?
                if (_isIncludeMilliSecond == false)
                {
                    // 시를 포함하나?
                    if (_isIncludeHour == true)
                        return string.Format(formatHMS, _hour, _minute, _second);
                    else
                        return string.Format(formatMS, _minute, _second);
                }
                else
                {
                    // 시를 포함하나?
                    if (_isIncludeHour == true)
                        return string.Format(formatHMSms, _hour, _minute, _second, _miSec);
                    else
                        return string.Format(formatMSms, _minute, _second, _miSec);
                }
            }

            // 시를 분에 포함하나?
            else
            {
                _minute += _hour * 60;

                // 분을 초에 포함 안 하나?
                if (_isIncludeSecondToMinute == false)
                {
                    // 밀리초를 포함 안 하나?
                    if (_isIncludeMilliSecond == false)
                    {
                        return string.Format(formatMS, _minute, _second);
                    }
                    else
                    {
                        return string.Format(formatMSms, _minute, _second, _miSec);
                    }
                }

                // 분을 초에 포함하나?
                else
                {
                    _second += _minute * 60;

                    // 밀리초를 포함 안 하나?
                    if (_isIncludeMilliSecond == false)
                    {
                        return string.Format(formatS, _second);
                    }
                    else
                    {
                        return string.Format(formatSms, _second, _miSec);
                    }
                }
            }
        }
        #endregion
        #region StringBuilder
        public static void RemoveAll_Func(this StringBuilder _stringBuilder)
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
        }
        #endregion
        #region String
        public static bool IsNullOrWhiteSpace_Func(this string _value)
        {
            return string.IsNullOrWhiteSpace(_value);
        }
        /// <summary>
        /// 완전 동일한가?
        /// </summary>
        public static bool IsCompare_Func(this string _value, string _targetStr)
        {
            return string.Compare(_value, _targetStr) == 0;
        }
        #endregion
        #region Sprite
        public static Texture GetTexture_Func(this Sprite _sprite)
        {
            try
            {
                if (_sprite.rect.width != _sprite.texture.width)
                {
                    int _width = Mathf.FloorToInt(_sprite.textureRect.width);
                    int _height = Mathf.FloorToInt(_sprite.textureRect.height);
                    Texture2D _texture = new Texture2D(_width, _height);

                    int _x = Mathf.FloorToInt(_sprite.textureRect.x);
                    int _y = Mathf.FloorToInt(_sprite.textureRect.y);
                    Color[] newColors = _sprite.texture.GetPixels(_x, _y, _width, _height);
                    _texture.SetPixels(newColors);

                    _texture.Apply();

                    return _texture;
                }
                else
                    return _sprite.texture;
            }
            catch (Exception _e)
            {
                Debug_C.Error_Func(_e.ToString());

                return _sprite.texture;
            }
        }
        public static Vector3 GetBoundsSize_Func(this Sprite _sprite, float _xPivot, float _yPivot)
        {
            Vector3 _size = _sprite.bounds.size;
            return new Vector3(_size.x * _xPivot, _size.y * _yPivot);
        }
        public static Vector3 GetBoundsSize_Func(this SpriteRenderer _srdr, float _xPivot, float _yPivot, Space _space = Space.Self)
        {
            Vector3 _localPos = _srdr.sprite.GetBoundsSize_Func(_xPivot, _yPivot);
            if(_space == Space.World)
                return _localPos + _srdr.transform.position;

            return _localPos;
        }
        #endregion
        #region Stack
        public static bool TryPush_Func<T>(this Stack<T> _stack, T _item)
        {
            if (_stack.Contains(_item) == false)
            {
                _stack.Push(_item);

                return true;
            }
            else
            {
                return false;
            }
        } 
        #endregion
        #region Vector
        /// <summary>
        /// 지정된 2D 영역 안의 값을 알아내는 함수
        /// </summary>
        /// <param name="_targetPos">좌표</param>
        /// <param name="_spaceMinPos">최소 지정 영역 Pos</param>
        /// <param name="_spaceMaxPos">최소 지정 영역 Pos</param>
        /// <param name="_isInside">좌표가 지정 영역 안에 있나요?</param>
        /// <returns>지정 영역 안으로 보정된 Vector2</returns>
        public static Vector2 GetInsidePos_Func(this Vector2 _targetPos, Vector2 _spaceMinPos, Vector2 _spaceMaxPos, out bool _isInside)
        {
            Vector2 _modifyPos = _targetPos;
            _isInside = true;

            if (_targetPos.x < _spaceMinPos.x)
            {
                _isInside = false;

                _modifyPos.x = _spaceMinPos.x;
            }
            else if (_spaceMaxPos.x < _targetPos.x)
            {
                _isInside = false;

                _modifyPos.x = _spaceMaxPos.x;
            }

            if (_targetPos.y < _spaceMinPos.y)
            {
                _isInside = false;

                _modifyPos.y = _spaceMinPos.y;
            }
            else if(_spaceMaxPos.y < _targetPos.y)
            {
                _isInside = false;

                _modifyPos.y = _spaceMaxPos.y;
            }

            return _modifyPos;
        }
        /// <summary>
        /// 지정된 2D 영역 안의 값을 알아내는 함수
        /// </summary>
        /// <param name="_targetPos">좌표</param>
        /// <param name="_spaceMinTrf">최소 지정 영역 Trf</param>
        /// <param name="_spaceMaxTrf">최소 지정 영역 Trf</param>
        /// <param name="_isInside">좌표가 지정 영역 안에 있나요?</param>
        /// <returns>지정 영역 안으로 보정된 Vector2</returns>
        public static Vector2 GetInsidePos_Func(this Vector2 _targetPos, Transform _spaceMinTrf, Transform _spaceMaxTrf, out bool _isInside)
        {
            return GetInsidePos_Func(_targetPos, _spaceMinTrf.position, _spaceMaxTrf.position, out _isInside);
        }

        public static bool CheckClose_Func(this Vector2 _thisPos, Transform _targetTrf, float _distance)
        {
            return _thisPos.CheckClose_Func(_targetTrf.position, _distance);
        }
        public static bool CheckClose_Func(this Vector2 _thisPos, Vector2 _targetPos, float _distance)
        {
            return Vector2.Distance(_thisPos, _targetPos) <= _distance;
        }

        public static float GetAngle_Func(this Vector3 _thisPos, Vector3 _targetPos, bool _isRelativeToRotate = false)
        {
            return Math_C.GetAngle_Func(_thisPos, _targetPos, _isRelativeToRotate);
        }
        public static float GetAngle_Func(this Vector2 _thisPos, Vector2 _targetPos, bool _isRelativeToRotate = false)
        {
            return Math_C.GetAngle_Func(_thisPos, _targetPos, _isRelativeToRotate);
        }
        public static Vector2 GetRight_Func(this Vector2 _thisPos, float _value)
        {
            return new Vector2(_thisPos.x + _value, _thisPos.y);
        }
        public static Vector2 GetLeft_Func(this Vector2 _thisPos, float _value)
        {
            return new Vector2(_thisPos.x - _value, _thisPos.y);
        }
        public static Vector2 GetUp_Func(this Vector2 _thisPos, float _value)
        {
            return new Vector2(_thisPos.x, _thisPos.y + _value);
        }
        public static Vector2 GetHeight_Func(this Vector2 _thisPos, float _value)
        {
            return new Vector2(_thisPos.x, _value);
        }
        public static Vector2 GetDown_Func(this Vector2 _thisPos, float _value)
        {
            return new Vector2(_thisPos.x, _thisPos.y - _value);
        }

        public static Vector2 GetRandX_Func(this Vector2 _thisPos, float _minX, float _maxX)
        {
            return new Vector2(UnityEngine.Random.Range(_minX, _maxX), _thisPos.y);
        }
        public static Vector2 GetRandY_Func(this Vector2 _thisPos, float _minY, float _maxY)
        {
            return new Vector2(_thisPos.x, UnityEngine.Random.Range(_minY, _maxY));
        }
        public static Vector2 GetRand_Func(this Vector2 _thisPos, float _minX, float _maxX, float _minY, float _maxY)
        {
            return new Vector2(UnityEngine.Random.Range(_minX, _maxX), UnityEngine.Random.Range(_minY, _maxY));
        }
        public static Vector2 GetRand_Func(this Vector2 _thisPos)
        {
            if (_thisPos != Vector2.zero)
            {
                float _randX = UnityEngine.Random.Range(0f, _thisPos.x);
                float _randY = UnityEngine.Random.Range(0f, _thisPos.y);
                return new Vector2(_randX, _randY);
            }
            else
            {
                return Vector2.zero;
            }
        }

        public static Vector2 GetCircumferencePos_Func(this Vector3 _circleCenterPos, float _radius, float _angle)
        {
            return Math_C.GetCircumferencePos_Func(_circleCenterPos, _radius, _angle);
        }
        public static Vector2 GetCircumferencePos_Func(this Vector2 _circleCenterPos, float _radius, float _angle)
        {
            return Math_C.GetCircumferencePos_Func(_circleCenterPos, _radius, _angle);
        }
        public static Vector2 GetCircumferencePos_Func(this Vector2 _circleCenterPos, float _radius)
        {
            float _angle = UnityEngine.Random.Range(0f, 360f);
            return GetCircumferencePos_Func(_circleCenterPos, _radius, _angle);
        }

        public static float GetDistance_Func(this Vector3 _thisPos, Vector3 _targetPos)
        {
            return Vector3.Distance(_thisPos, _targetPos);
        }
        public static float GetDistance_Func(this Vector2 _thisPos, Vector2 _targetPos)
        {
            return Vector2.Distance(_thisPos, _targetPos);
        }
        public static bool CheckDistance_Func(this Vector3 _thisPos, Vector3 _targetPos, float _innerDist)
        {
            return Math_C.CheckDistance_Func(ref _thisPos, ref _targetPos, ref _innerDist);
        }
        public static bool CheckDistance_Func(this Vector2 _thisPos, Vector2 _targetPos, float _innerDist)
        {
            return Math_C.CheckDistance_Func(ref _thisPos, ref _targetPos, ref _innerDist);
        }
        #endregion
    }

    #region Singleton
    namespace Singleton
    {
        public abstract class Singleton_C<T> : MonoBehaviour where T : MonoBehaviour
        {
            private static T instance;
            public static T Instance
            {
                get
                {
                    if (instance is null)
                        Generate_Func();

                    return instance;
                }
            }

            // 싱글턴을 사용하기 위해 아래의 함수를 최초 1회 호출해야 함.
            // 미호출 시 Property를 통해 예외처리하므로 문제는 없으나 Warning Log는 출력됨.

            public static void Generate_Func()
            {
                T _singletonComponent = FindObjectOfType<T>();

                Singleton_C<T>.Generate_Func(_singletonComponent);
            }
            public static void Generate_Func(T _existenceComonent)
            {
                GameObject _singletonObj = null;

                if (_existenceComonent != null)
                {
                    _singletonObj = _existenceComonent.gameObject;

                    instance = _existenceComonent;
                }
                else
                {
                    Debug.LogWarning("싱글턴 객체를 동적 생성하였습니다. 따라서 Data Initialize를 고려해주시기 바랍니다. - " + typeof(T));

                    _singletonObj = new GameObject();
                    instance = _singletonObj.AddComponent<T>();
                    _singletonObj.name = StringBuilder_C.Append_Func("(Singleton)", typeof(T).ToString());
                }

                UnityEngine.Object.DontDestroyOnLoad(_singletonObj);
            }
        }

        public abstract class Singleton_Cor_Mono_DontDestroy<T> : Singleton_C<T> where T : MonoBehaviour
        {
            public abstract IEnumerator Init_Cor();
        }
        public abstract class Singleton_Func_Mono_DontDestroy<T> : Singleton_C<T> where T : MonoBehaviour
        {
            public abstract void Init_Func();
        }
    }
    #endregion
    #region Resource
    public static class Resources_C
    {
        public static T Load<T>(string _path) where T : UnityEngine.Object
        {
            T _loadObj = Resources.Load<T>(_path);

#if UNITY_EDITOR
            if (_loadObj == null)
                Debug_C.Error_Func("다음 경로의 리소스를 불러오지 못했습니다. : " + _path); 
#endif

            return _loadObj;
        }
    }

    namespace ResourceFindPath
    {
        // ResourceFindPath 상속 받는 클래스에서 경로를 스크립트에 적어놓고 쓰는 거 추천

        public abstract class ResourceFindPath<PathType>
        {
            private Dictionary<PathType, string> pathDic;

            public virtual void Init_Func()
            {
                pathDic = new Dictionary<PathType, string>();
            }

            public void SetPath_Func(PathType _pathType, string _path)
            {
                pathDic.Add_Func(_pathType, _path);
            }

            public T GetResource_Func<T>(PathType _pathType, bool _isDebug = true) where T : UnityEngine.Object
            {
                string _path = this.pathDic.GetValue_Func(_pathType);

                return this.GetResource_Func<T>(_path, _isDebug);
            }

            // 잘 불러와졌는지 체크
            public T GetResource_Func<T>(string _path, bool _isDebug = true) where T : UnityEngine.Object
            {
                T _returnObj = Resources.Load<T>(_path);
                if (_returnObj == null)
                {
                    if (_isDebug == true)
                    {
                        Debug.LogError("Bug : 데이터 로드 실패");
                        Debug.Log("Path : " + _path);
                    }
                }

                return _returnObj;
            }
            public T[] GetResourceAll_Func<T>(PathType _pathType, bool _isDebug = true) where T : UnityEngine.Object
            {
                string _path = this.pathDic.GetValue_Func(_pathType);

                return this.GetResourceAll_Func<T>(_path, _isDebug);
            }

            // 잘 불러와졌는지 체크
            public T[] GetResourceAll_Func<T>(string _path, bool _isDebug = true) where T : UnityEngine.Object
            {
                T[] _returnObjArr = Resources.LoadAll<T>(_path);
                if (_returnObjArr == null)
                {
                    if (_isDebug == true)
                    {
                        Debug.LogError("Bug : 데이터 로드 실패");
                        Debug.Log("Path : " + _path);
                    }
                }

                return _returnObjArr;
            }

            public ComponentType GetComponentByInstantiateObj_Func<ComponentType>(PathType _pathType)
            {
                GameObject _loadObj = this.GetResource_Func<GameObject>(_pathType);
                GameObject _genObj = GameObject.Instantiate(_loadObj);

                ComponentType _componentType = _genObj.GetComponent<ComponentType>();

                return _componentType;
            }
        }
    }

    #endregion
    #region Data Structure
    namespace DataStructure
    {
        [System.Serializable]
        public sealed class CirculateQueue<T>
        {
            [SerializeField] private List<T> circulateList;
            private int circulateID;
            public T GetItem { get { return this.circulateList[this.circulateID]; } }
            public int GetCount { get { return this.circulateList.Count; } }

            public CirculateQueue()
            {
                this.circulateList = new List<T>();

                this.circulateID = 0;
            }

            public void SetID_Func(int _id)
            {
                this.circulateID = _id;
            }

            public int GetIndexToItem_Func(T _t)
            {
                return this.circulateList.IndexOf(_t);
            }

            public T GetItemToIndex_Func(int _idx)
            {
                return this.circulateList[_idx];
            }

            public void Enqueue_Func(T _t)
            {
                this.circulateList.AddNewItem_Func(_t);
            }

            public void Enqueue_Func(params T[] _tArr)
            {
                for (int i = 0; i < _tArr.Length; i++)
                {
                    this.circulateList.AddNewItem_Func(_tArr[i]);
                }
            }

            public T Dequeue_Func(bool _isReverse = false)
            {
                if (_isReverse == false)
                {
                    this.circulateID++;

                    if (this.circulateList.Count <= this.circulateID)
                        this.circulateID = 0;
                }
                else
                {
                    this.circulateID--;

                    if (this.circulateID < 0)
                        this.circulateID = this.circulateList.Count - 1;
                }

                return this.circulateList[circulateID];
            }

            public void Clear_Func()
            {
                this.circulateList.Clear();
            }
        }

        // Generic Queue가 있어서 사용할 필요 없을 듯...?
        public sealed class Queue_C<T>
        {
            private List<T> queueList;
            public int QueueItemNum { get { return this.queueList.Count; } }
            public bool HasItem { get { return 0 < this.queueList.Count ? true : false; } }

            public Queue_C()
            {
                this.queueList = new List<T>();
            }

            public void Enqueue_Func(T _t)
            {
                this.queueList.AddNewItem_Func(_t);
            }
            public T Dequeue_Func()
            {
                T _returnValue = queueList[0];

                this.queueList.Remove(_returnValue);

                return _returnValue;
            }
            public bool Dequeue_Func(out T _tryGet)
            {
                bool _isHave = false;

                if (0 < queueList.Count)
                {
                    _isHave = true;

                    _tryGet = this.Dequeue_Func();
                }
                else
                {
                    _isHave = false;

                    _tryGet = default(T);
                }

                return _isHave;
            }
        }
    }
    #endregion
    #region Gacha
    namespace Gacha
    {
        [System.Serializable]
        public class GachaSystem<T>
        {
            public Node rootNode;

            private Data[] dataArr;
            private int totalFP;
            private int splitCount;
            private GachaResultData<T>[] gachaResultDataArr;

            public int TotalFP { get => totalFP; }

            public GachaSystem(GachaResultData<T>[] _gachaResultDataArr, int _splitCount = 2)
            {
                this.gachaResultDataArr = _gachaResultDataArr;

                int _dataNum = _gachaResultDataArr.Length;
                this.dataArr = new Data[_dataNum];
                this.totalFP = 0;
                this.splitCount = _splitCount;

                for (int i = 0; i < _dataNum; i++)
                {
                    dataArr[i] = new Data();
                    dataArr[i].arrID = i;
                    int _FPvalue = _gachaResultDataArr[i].floatingProbability;
                    dataArr[i].floatingProbability = _FPvalue;

                    totalFP += _FPvalue;
                }

                rootNode = GetNode_Func(null, -1, 0, _dataNum, 0, this.totalFP);
            }
            private Node GetNode_Func(Node _parentNode, int _siblingLv, int _start, int _end, int _loadedFP, int _totalCalcFP)
            {
                _siblingLv++;

                Node _node = null;

                if (_start < _end &&
                    (_end != this.dataArr.Length || _start + 1 != this.dataArr.Length))
                {
                    _node = new Node();
                    _node.childNodeArr = new Node[this.splitCount];

                    int _addFP = 0;
                    int _splitFP = _totalCalcFP / this.splitCount;
                    int _addSplitFP = _splitFP;
                    int _childNodeID = 0;
                    for (int i = _start; i < _end; i++)
                    {
                        int _FPvalue = dataArr[i].floatingProbability;
                        _addFP += _FPvalue;

                        if (_addSplitFP < _addFP)
                        {
                            Node _childNode = GetNode_Func(_node, _siblingLv, _start, i, _loadedFP, _totalCalcFP - _addFP);
                            _node.childNodeArr[_childNodeID] = _childNode;

                            _childNodeID++;

                            if (_childNodeID + 1 < this.splitCount)
                            {
                                _addSplitFP = (_childNodeID + 1) * _splitFP;
                            }
                            else
                            {
                                _childNode = GetNode_Func(_node, _siblingLv, i + 1, _end, _loadedFP + _addFP, _totalCalcFP - _addFP);
                                _node.childNodeArr[_childNodeID] = _childNode;

                                break;
                            }
                        }
                    }

                    if (0 < _siblingLv)
                    {
                        _node.nodeType = NodeType.Normal;
                    }
                    else
                    {
                        _node.nodeType = NodeType.Root;
                    }
                }
                else
                {
                    LeafNode _leafNode = new LeafNode();

                    _leafNode.data = this.dataArr[_start];
                    _leafNode.nodeType = NodeType.Leaf;
                    _node = _leafNode;
                }

                _node.siblingLv = _siblingLv;
                _node.FP = _loadedFP;
                _node.parentNode = _parentNode;

                return _node;
            }

            public T GetGachaResultType_Func()
            {
                int _checkFP = UnityEngine.Random.Range(0, this.totalFP) + 1;
                int _resultID = GetGachaID_Func(_checkFP, this.rootNode.childNodeArr);
                return this.gachaResultDataArr[_resultID].resultType;
            }
            private static int GetGachaID_Func(int _checkFP, Node[] _nodeArr)
            {
                for (int i = _nodeArr.Length - 1; 0 <= i; i--)
                {
                    NodeType _nodeType = _nodeArr[i].nodeType;
                    if (_nodeType == NodeType.Normal)
                    {
                        if (_nodeArr[i].FP < _checkFP)
                        {
                            return GetGachaID_Func(_checkFP, _nodeArr[i].childNodeArr);
                        }
                    }
                    else if (_nodeType == NodeType.Leaf)
                    {
                        LeafNode _leafNode = _nodeArr[i] as LeafNode;

                        if (_leafNode.FP < _checkFP)
                        {
                            return _leafNode.data.arrID;
                        }
                    }
                    else
                    {
                        Debug.LogError("_nodeType : " + _nodeType);
                        break;
                    }
                }

                Debug.LogError("_checkFP : " + _checkFP);

                return 0;
            }
        }

        [System.Serializable]
        public class Node
        {
            public int siblingLv;
            public int FP;
            public NodeType nodeType;

            public Node parentNode;
            public Node[] childNodeArr;
        }

        [System.Serializable]
        public class LeafNode : Node
        {
            public Data data;
        }

        [System.Serializable]
        public struct Data
        {
            public int arrID;
            public int floatingProbability;
        }

        [System.Serializable]
        public struct GachaResultData<T>
        {
            public T resultType;
            public int floatingProbability;
        }

        public enum NodeType
        {
            None = 0,

            Root,
            Normal,
            Leaf,
        }
    }
    #endregion
    #region TagDictionary
    namespace TagDictionary
    {
        [System.Serializable]
        public class TagDictionary<Key, Value> where Value : ITagDic<Key>
        {
            private Dictionary<Key, List<Value>> dic;

            public TagDictionary(params Key[] _keyArr)
            {
                this.dic = new Dictionary<Key, List<Value>>();

                this.GenerateKey_Func(_keyArr);
            }
            public TagDictionary(IEqualityComparer<Key> _iEqualityComparer, params Key[] _keyArr)
            {
                this.dic = new Dictionary<Key, List<Value>>(_iEqualityComparer);

                this.GenerateKey_Func(_keyArr);
            }

            public void GenerateKey_Func(params Key[] _keyArr)
            {
                dic = new Dictionary<Key, List<Value>>();

                for (int i = 0; i < _keyArr.Length; i++)
                {
                    List<Value> _list = new List<Value>();

                    this.dic.Add(_keyArr[i], _list);
                }
            }

            public void Add_Func(Value _value)
            {
                Key[] _keyArr = _value.GetTagKey_Func();

                foreach (Key _key in _keyArr)
                {
                    List<Value> _list = this.dic.GetValue_Func(_key);
                    _list.AddNewItem_Func(_value);
                }
            }

            public bool TryGetListValue_Func(Key _key, out List<Value> _valueList)
            {
                return this.dic.TryGetValue(_key, out _valueList);
            }

            public void Remove_Func(Value _value)
            {
                Key[] _keyArr = _value.GetTagKey_Func();

                foreach (Key _key in _keyArr)
                {
                    List<Value> _list = this.dic.GetValue_Func(_key);
                    _list.Remove_Func(_value);
                }
            }

            public bool IsEmpty_Func()
            {
                foreach (var item in this.dic)
                {
                    if (item.Value.Count != 0)
                        return false;
                }

                return true;
            }
        }

        public interface ITagDic<KeyType>
        {
            KeyType[] GetTagKey_Func();
        }
    }
    #endregion
    #region DateTimeTick
    [System.Serializable]
    public struct DateTimeTick
    {
        [HideInInspector] public long tick;
        [Newtonsoft.Json.JsonIgnore]
        public DateTime GetTime
        {
            get
            {

#if UNITY_EDITOR
                if (this.tick < DateTime.MinValue.Ticks || DateTime.MaxValue.Ticks < this.tick)
                {
                    Debug_C.Error_Func(this.tick);
                    return default;
                } 
#endif

                return new DateTime(this.tick);
            }
        }

        /// <summary>
        /// 현재 시간을 기준으로 남은 시간
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public DateTime GetRemainTime
        {
            get
            {
                DateTime _nowTime = TimeSystem_Manager.Instance.Now;
                DateTime _thisTime = this.GetTime;
                if (_nowTime <= _thisTime)
                    return new DateTime((_thisTime - _nowTime).Ticks);
                else
                    return default;
            }
        }
        /// <summary>
        /// 현재 시간을 기준으로 지나간 시간
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan GetPassTime
        {
            get
            {
                DateTime _thisTime = this.GetTime;
                DateTime _nowTime = TimeSystem_Manager.Instance.Now;
                if (_thisTime <= _nowTime)
                    return _nowTime - _thisTime;
                else
                    return default;
            }
        }
        [Newtonsoft.Json.JsonIgnore] public bool IsPassTime => this.tick < TimeSystem_Manager.Instance.Now.Ticks;
        [Newtonsoft.Json.JsonIgnore] public bool IsRemainTime => TimeSystem_Manager.Instance.Now.Ticks < this.tick;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        [Newtonsoft.Json.JsonIgnore]
        public string GetTimeStr => this.GetTime.ToString();
        [Newtonsoft.Json.JsonIgnore]
        public bool IsEmpty => this.tick == 0L;

        public DateTimeTick(DateTime _dateTime)
        {
            this.tick = _dateTime.Ticks;
        }
        public DateTimeTick(TimeSpan _timeSpan)
        {
            this.tick = _timeSpan.Ticks;
        }
        public DateTimeTick(long _tick)
        {
            this.tick = _tick;
        }

        public void SetNow_Func()
        {
            this.tick = TimeSystem_Manager.Instance.Now.Ticks;
        }

        public DateTimeTick AddSecond_Func(long _sec)
        {
            return new DateTimeTick(this.tick + (_sec * 10000000));
        }

        public override string ToString()
        {
            return this.GetTimeStr;
        }

        public static DateTimeTick operator +(DateTimeTick _left, DateTimeTick _right)
        {
            return _left.tick + _right.tick;
        }
        public static DateTimeTick operator -(DateTimeTick _left, DateTimeTick _right)
        {
            return _left.tick - _right.tick;
        }

        public static DateTimeTick operator -(DateTime _left, DateTimeTick _right)
        {
            return _left.Ticks - _right.tick;
        }
        public static DateTimeTick operator -(DateTimeTick _left, DateTime _right)
        {
            return _left.tick - _right.Ticks;
        }

        public static bool operator <(DateTimeTick _left, DateTimeTick _right) => _left.tick < _right.tick;
        public static bool operator >(DateTimeTick _left, DateTimeTick _right) => _left.tick > _right.tick;
        public static bool operator <=(DateTimeTick _left, DateTimeTick _right) => _left.tick <= _right.tick;
        public static bool operator >=(DateTimeTick _left, DateTimeTick _right) => _left.tick >= _right.tick;

        public static implicit operator DateTimeTick(DateTime _dateTime) => new DateTimeTick(_dateTime);
        public static implicit operator DateTimeTick(TimeSpan _timeSpan) => new DateTimeTick(_timeSpan);
        public static implicit operator DateTimeTick(long _tick) => new DateTimeTick(_tick);
        public static implicit operator DateTime(DateTimeTick _tick) => _tick.GetTime;
        public static implicit operator TimeSpan(DateTimeTick _tick) => new TimeSpan(_tick.tick);

        public static explicit operator DateTimeTick(string _dateTimeStr) => System.Convert.ToDateTime(_dateTimeStr);
        // GetRemainTime을 호출할 시 현재 시간보다 늦춰지면 마이너스라 에러를 뱉을 듯. 이럴 경우 0을 뱉도록 예외처리 ㄱㄱ
    }
    #endregion
    #region Vector_C
    public static class Vector_C
    {
        public static Vector2 GetRand_Func(float _value, RandMinType _randMinType = RandMinType.Zero)
        {
            float _minValue = 0f;
            switch (_randMinType)
            {
                case RandMinType.Zero:
                    _minValue = 0f;
                    break;

                case RandMinType.Minus:
                    _minValue = -_value;
                    break;

                case RandMinType.None:
                default:
                    Debug_C.Error_Func("_randMinType : " + _randMinType);
                    _minValue = 0f;
                    break;
            }

            float _x = Random_C.Random_Func(_minValue, _value);
            float _y = Random_C.Random_Func(_minValue, _value);
            return new Vector2(_x, _y);
        }

        public enum RandMinType
        {
            None = 0,

            Zero,
            Minus,
        }
    }
    #endregion

    #region Tween_C

    public static class Tween_C
    {
        private static Dictionary<Transform, TweenData> baseTwnDic;
        private static Dictionary<Transform, TweenData> twnDic
        {
            get
            {
                if (baseTwnDic == null)
                    baseTwnDic = new Dictionary<Transform, TweenData>();

                return baseTwnDic;
            }
        }

        public static void OnPunch_Func(GameObject _punchObj, float _punchPower = -1f, float _duration = -1f, float _originScale = 1f, Action _del = null)
        {
            OnPunch_Func(_punchObj.transform, _punchPower, _duration, _originScale, _del);
        }
        public static void OnPunch_Func(Component _punchComponent, float _punchPower = -1f, float _duration = -1f, float _originScale = 1f, Action _del = null)
        {
            OnPunch_Func(_punchComponent.transform, _punchPower, _duration, _originScale, _del);
        }
        public static void OnPunch_Func(Transform _punchTrf, float _punchPower = -1f, float _duration = -1f, float _originScale = 1f, Action _del = null)
        {
            if (_punchPower == -1f)
                _punchPower = FrameWork.DataBase_Manager.Instance.GetUi_C.twn_Power;

            if (_duration == -1f)
                _duration = FrameWork.DataBase_Manager.Instance.GetUi_C.twn_Duraion;

            _punchTrf.DORewind();

            if (twnDic.TryGetValue(_punchTrf, out TweenData _twnData) == false)
            {
                _twnData = new TweenData();

                _twnData.originScale = Vector3.one * _originScale;

                Tween _twn = _punchTrf.DOPunchScale(Vector3.one * _punchPower, _duration).OnComplete(delegate ()
                {
                    _punchTrf.localScale = Vector3.one * _originScale;

                    if (_del != null)
                        _del();
                }).SetAutoKill(false);

                _twn.Pause();

                _twnData.twn = _twn;

                twnDic.Add_Func(_punchTrf, _twnData);
            }

            _punchTrf.localScale = _twnData.originScale;

            _twnData.twn.Restart();
        }

        public class TweenData
        {
            public Tween twn;
            public Vector3 originScale;
        }
    } 

    #endregion
    #region UGUI_C
    public class UGUI_C
    {
        public static void SetContentGroupResize_Func
            (RectTransform _contentGroupRtrf, float _groupSpace = 0f, float _topSpace = 0f, float _bottomSpace = 0f, params ContentGroupData[] _dataArr)
        {
            float _stackHeight = _topSpace;

            for (int i = 0; i < _dataArr.Length; i++)
            {
                ContentGroupData _data = _dataArr[i];
                RectTransform _contentRtrf = _data.contentRtrf;
                float _height = _data.height;
                _contentRtrf.SetDynamicSize_Func(_height, DirectionType.Down);

                //if (0 < i)
                _contentRtrf.anchoredPosition = Vector3.down * _stackHeight;

                _stackHeight += _height + _groupSpace;
            }

            _contentGroupRtrf.SetDynamicSize_Func(_stackHeight + _bottomSpace, DirectionType.Down);
        }

        [System.Serializable]
        public struct ContentGroupData
        {
            public float height;
            public RectTransform contentRtrf;

            public ContentGroupData(float height, RectTransform contentRtrf)
            {
                this.height = height;
                this.contentRtrf = contentRtrf;
            }
        }
    }
    #endregion
    #region StringFormat_C
    public static class StringFormat_C
    {
        public static string GetFormatting_Func(string _base, IFormatter _iFormat)
        {
            int _formattingNum = _iFormat.FormattingNum;

            switch (_formattingNum)
            {
                case 0:
                    return _base;

                case 1:
                    return string.Format(_base
                    , _iFormat.GetFormattingStr_Func(0)
                    );

                case 2:
                    return string.Format(_base
                    , _iFormat.GetFormattingStr_Func(0)
                    , _iFormat.GetFormattingStr_Func(1)
                    );

                case 3:
                    return string.Format(_base
                    , _iFormat.GetFormattingStr_Func(0)
                    , _iFormat.GetFormattingStr_Func(1)
                    , _iFormat.GetFormattingStr_Func(2)
                    );

                case 4:
                    return string.Format(_base
                    , _iFormat.GetFormattingStr_Func(0)
                    , _iFormat.GetFormattingStr_Func(1)
                    , _iFormat.GetFormattingStr_Func(2)
                    , _iFormat.GetFormattingStr_Func(3)
                    );

                case 5:
                    return string.Format(_base
                    , _iFormat.GetFormattingStr_Func(0)
                    , _iFormat.GetFormattingStr_Func(1)
                    , _iFormat.GetFormattingStr_Func(2)
                    , _iFormat.GetFormattingStr_Func(3)
                    , _iFormat.GetFormattingStr_Func(4)
                    );

                default:
                    Debug.LogError("_formattingNum : " + _formattingNum);
                    return _base;
            }
        }

        public interface IFormatter
        {
            int FormattingNum { get; }
            string GetFormattingStr_Func(int _id);
        }
    }
    #endregion
    #region Random_C
    public static class Random_C
    {
        public static float GetValue => UnityEngine.Random.value;
        public static int Random_Func(int _min, int _max)
        {
            return UnityEngine.Random.Range(_min, _max);
        }
        public static float Random_Func(float _min, float _max)
        {
            return UnityEngine.Random.Range(_min, _max);
        }
        public static float Random_Func(Vector2 _range)
        {
            return UnityEngine.Random.Range(_range.x, _range.y);
        }
        public static bool Random_Func()
        {
            return UnityEngine.Random.value < .5f;
        }
        
        public static Vector2 RandomePos_Func(Transform _minPosTrf, Transform _maxPosTrf)
        {
            return RandomePos_Func(_minPosTrf.position, _maxPosTrf.position);
        }
        public static Vector2 RandomePos_Func(Vector2 _minPos, Vector2 _maxPos)
        {
            return RandomePos_Func(_minPos.x, _maxPos.x, _minPos.y, _maxPos.y);
        }
        public static Vector2 RandomePos_Func(float _minPosX, float _maxPosX, float _minPosY, float _maxPosY)
        {
            float _x = UnityEngine.Random.Range(_minPosX, _maxPosX);
            float _y = UnityEngine.Random.Range(_minPosY, _maxPosY);

            return new Vector2(_x, _y);
        }

        public static bool CheckPercent_Func(int _maxPercent, int _checkPercent = 1)
        {
            return UnityEngine.Random.Range(0, _maxPercent) <= (_checkPercent - 1);
        }
        public static T GetRandomItem_Func<T>(T _item1, T _item2)
        {
            return UnityEngine.Random.value < .5f == true ? _item1 : _item2;
        }
        public static T GetRandomItem_Func<T>(T _item1, T _item2, T _item3)
        {
            float _value = UnityEngine.Random.value;
            if (_value < 0.333f)
                return _item1;
            else if (_value < 0.666f)
                return _item2;
            else
                return _item3;
        }
        public static T GetRandomItem_Func<T>(T _item1, T _item2, T _item3, T _item4)
        {
            float _value = UnityEngine.Random.value;
            if (_value < 0.25f)
                return _item1;
            else if (_value < 0.5f)
                return _item2;
            else if (_value < 0.75f)
                return _item3;
            else
                return _item4;
        }

        public static bool IsRandom_Func(this float _value)
        {
            if (0f >= _value) return false;
            if (1f <= _value) return true;

            return UnityEngine.Random.value <= _value;
        }
    }
    #endregion
    #region Logic_C
    public static class Logic_C
    {
        public static void OnZigZagTile_Func(int _xMax, int _yMax, Action<int, int> _loopCallback, int _interval = 1, int _xBegin = 0)
        {
            bool _isZigZag = Random_C.IsRandom_Func(.5f);
            int _yInterval = _interval * 2;
            for (int _x = 0; _x < _xMax; _x += _interval)
            {
                for (int _y = (_isZigZag == false ? 0 : _interval); _y < _yMax; _y += _yInterval)
                {
                    _loopCallback(_x, _y);
                }

                _isZigZag = !_isZigZag;
            }
        }
    }
    #endregion
    #region ListDic
    public class ListDic<K, V>
    {
        [ShowInInspector, ReadOnly] private List<Data> list;
        [ShowInInspector, ReadOnly] private Dictionary<K, V> dic;

        public ListDic()
        {
            this.list = new List<Data>();
            this.dic = new Dictionary<K, V>();
        }

        public void Add_Func(K _key, V _value)
        {
            this.dic.Add(_key, _value);
            this.list.Add(new Data(_key, _value));
        }

        public void Remove_Func(K _key)
        {
            this.TryTake_Func(_key, out _);
        }

        public bool TryTakeLastItem_Func()
        {
            //Data _data = this.list.GetTakeLastItem_Func();
            //this.dic.TakeLast(0);

            return true;
        }
        public bool TryTake_Func(K _key, out V _Value, bool _isLog = true)
        {
            if (this.dic.TryTake_Func(_key, out _Value) == true)
            {
                //this.list.Remove(_Value);

                return true;
            }
            else
            {
                if (_isLog == true)
                    Debug_C.Error_Func("다음 Key는 없습니다. : " + _key);

                return false;
            }
        }

        public struct Data
        {
            public K key;
            public V value;

            public Data(K _key, V _value)
            {
                this.key = _key;
                this.value = _value;
            }
        }
    } 
    #endregion

    #region DataStructure_C
    public class DataStructure_C
    {
        public static Dictionary<K, List<V>> Add_Func<K, V>(
            Dictionary<K, List<V>> _dic,
            K _key, V _value)
            where K : struct, IConvertible
        {
            if (_dic == null)
                _dic = new Dictionary<K, List<V>>(EnumCompare.EnumCompare<K>.Instance);

            if (_dic.TryGetValue(_key, out List<V> _list) == false)
            {
                _list = new List<V>();
                _dic.Add(_key, _list);
            }

            _list.AddNewItem_Func(_value);

            return _dic;
        }

        public static Dictionary<KeyType, ValueType[]> GetListToArr_Func<KeyType, ValueType>(
            Dictionary<KeyType, List<ValueType>> _dic,
            Dictionary<KeyType, ValueType[]> _resultDic = null)
            where KeyType : struct, IConvertible
        {
            if (_resultDic == null)
                _resultDic = new Dictionary<KeyType, ValueType[]>(EnumCompare.EnumCompare<KeyType>.Instance);

            foreach (var item in _dic)
            {
                KeyType _key = item.Key;
                List<ValueType> _list = item.Value;
                _resultDic.Add(_key, _list.ToArray());
            }

            return _resultDic;
        }
    }
    #endregion
    #region GameDataEditor
#if GameDataEditor
namespace GameDataEditor
{
    public partial class GDEDataManager
    {
        public static GDEDataManager Instance
        {
            get
            {
                if (_instance == null || dataDic == null)
                    Init_Func();

                return _instance;
            }
        }
        private static GDEDataManager _instance;

        private static Dictionary<string, IGDEData> dataDic;

        private DateTime editorSyncTime;

        public static void Init_Func(string _gdePath = null)
        {
            _instance = new GDEDataManager();

            if (_gdePath.IsNullOrWhiteSpace_Func() == true)
                _gdePath = "gde_data";

            if (dataDic == null)
                dataDic = new Dictionary<string, IGDEData>();

            GDEDataManager.Init(_gdePath);
        }

        public List<T> GetData_Func<T>() where T : IGDEData
        {
            if (Instance == null)
                Init_Func();

            List<T> _gdeDataList = GDEDataManager.GetAllItems<T>();
            return _gdeDataList;
        }

        public T GetData_Func<T>(string _key) where T : IGDEData
        {
            T _resultData = null;

            if(this.editorSyncTime < DateTime.Now && _key.IsNullOrWhiteSpace_Func() == false)
            {
                this.editorSyncTime = this.editorSyncTime.AddSeconds(0.5d);

                if (dataDic.ContainsKey(_key) == false)
                {
                    List<T> _gdeDataList = GDEDataManager.GetAllItems<T>();

                    foreach (T _gdeData in _gdeDataList)
                    {
                        if (dataDic.ContainsKey(_gdeData.Key) == false)
                            dataDic.Add(_gdeData.Key, _gdeData);
                    }
                }

                if(dataDic.ContainsKey(_key) == true)
                    _resultData = dataDic.GetValue_Func(_key) as T;
            }

            return _resultData;
        }
        public bool TryGetData_Func<T>(string _key, out T _gdeData) where T : IGDEData
        {
            _gdeData = this.GetData_Func<T>(_key);
            return _gdeData != null;
        }
    }
}
#endif
    #endregion

    public static partial class LayerMask_C
    {
        private static Dictionary<string, int> nameToLayerDic;

        public static int NameToLayer_Func(string _layerStr)
        {
            if (nameToLayerDic == null)
                nameToLayerDic = new Dictionary<string, int>();

            return nameToLayerDic.GetValue_Func(_layerStr, () => LayerMask.NameToLayer(_layerStr));
        }
    }
    public static partial class SortingLayer_C
    {
        private static Dictionary<string, int> nameToIdDic;

        public static int NameToID_Func(string _layerStr)
        {
            if (nameToIdDic == null)
                nameToIdDic = new Dictionary<string, int>();

            return nameToIdDic.GetValue_Func(_layerStr, () => SortingLayer.NameToID(_layerStr));
        }
    }

    // Developing System
    #region TextPrint_Manager
    namespace TextPrint
    {
        //public class TextPrint_Manager : MonoBehaviour
        //{
        //    public static TextPrint_Manager Instance;

        //    [SerializeField] private Color printColor;
        //    [SerializeField] private float punchTime;
        //    [SerializeField] private float printSize;
        //    [SerializeField] private float printTime;
        //    [SerializeField] private float clearTime;
        //    public Color PrintColor { get { return printColor; } }
        //    public float PunchTime { get { return punchTime; } }
        //    public float PrintSize { get { return printSize; } }
        //    public float PrintTime { get { return printTime; } }
        //    public float ClearTime { get { return clearTime; } }
        //    public static Color _a;

        //    [SerializeField] private Transform dpGroupTrf;
        //    private List<TextPrint_Script> dpList;
        //    [SerializeField] private GameObject dpObj;

        //    public void Init_Func()
        //    {
        //        Instance = this;

        //        dpList = new List<TextPrint_Script>();
        //        for (int i = 0; i < 10; i++)
        //        {
        //            GenerateDP_Func();
        //        }
        //    }
        //    private TextPrint_Script GenerateDP_Func()
        //    {
        //        GameObject _dpObj = Instantiate(dpObj);
        //        _dpObj.transform.SetParent(dpGroupTrf);

        //        TextPrint_Script _dpClass = _dpObj.GetComponent<TextPrint_Script>();
        //        _dpClass.Init_Func();
        //        dpList.Add(_dpClass);
        //        _dpObj.SetActive(false);

        //        return _dpClass;
        //    }

        //    public void Print_Func(Vector2 _pos, string _value, Sprite _sprite = null)
        //    {
        //        Print_Func(_pos, _value, PrintColor, _sprite);
        //    }
        //    public void Print_Func(Vector2 _pos, float _value)
        //    {
        //        Print_Func(_pos, _value, PrintColor);
        //    }
        //    public void Print_Func(Vector2 _pos, float _value, Color _color)
        //    {
        //        Print_Func(_pos, ((int)_value).ToString(), _color);
        //    }
        //    public void Print_Func(Vector2 _pos, string _value, Color _color, Sprite _sprite = null, params float[] _varArr)
        //    {
        //        TextPrint_Script _textPrintClass = null;
        //        if (0 < dpList.Count)
        //        {
        //            _textPrintClass = this.dpList[0];
        //            this.dpList.RemoveAt(0);
        //        }
        //        else
        //        {
        //            _textPrintClass = GenerateDP_Func();
        //        }

        //        _textPrintClass.Print_Func(_pos, _value, _color, null, _varArr);
        //    }

        //    public void PrintOver_Func(TextPrint_Script _textPrintClass)
        //    {
        //        this.dpList.Add(_textPrintClass);
        //    }
        //}
        //public class TextPrint_Script : MonoBehaviour
        //{
        //    public Text damageText;
        //    private float punchTime;
        //    private float printSize;
        //    private float printTime;
        //    private float clearTime;
        //    [SerializeField]
        //    private Image printImage;

        //    public void Init_Func()
        //    {
        //        this.gameObject.SetActive(false);
        //    }
        //    public void Print_Func(Vector2 _pos, string _value, Color _color, Sprite _sprite = null, params float[] _varArr)
        //    {
        //        if (_sprite != null)
        //        {
        //            printImage.SetFade_Func(1f);
        //            printImage.SetNativeSize_Func(_sprite);
        //        }

        //        if (_varArr.Length != 4)
        //        {
        //            punchTime = TextPrint_Manager.Instance.PunchTime;
        //            printSize = TextPrint_Manager.Instance.PrintSize;
        //            printTime = TextPrint_Manager.Instance.PrintTime;
        //            clearTime = TextPrint_Manager.Instance.ClearTime;
        //        }
        //        else
        //        {
        //            punchTime = 0f < _varArr[0] ? _varArr[0] : TextPrint_Manager.Instance.PunchTime;
        //            printSize = 0f < _varArr[1] ? _varArr[1] : TextPrint_Manager.Instance.PrintSize;
        //            printTime = 0f < _varArr[2] ? _varArr[2] : TextPrint_Manager.Instance.PrintTime;
        //            clearTime = 0f < _varArr[3] ? _varArr[3] : TextPrint_Manager.Instance.ClearTime;
        //        }

        //        this.gameObject.SetActive(true);

        //        damageText.text = _value;
        //        damageText.color = _color;

        //        this.transform.position = _pos;
        //        this.transform.localScale = Vector3.zero;
        //        this.transform.DOScale(Vector3.one * printSize, punchTime);

        //        damageText.DOColor(_color, printTime).OnComplete(DoClear_Func);
        //    }

        //    public void DoClear_Func()
        //    {
        //        damageText.DOColor(Color.clear, clearTime);

        //        this.transform.DOScale(Vector3.zero, clearTime).OnComplete(PrintOver_Func);
        //    }

        //    public void PrintOver_Func()
        //    {
        //        if (printImage.sprite != null)
        //        {
        //            printImage.sprite = null;
        //            printImage.SetFade_Func(0f);
        //        }

        //        this.gameObject.SetActive(false);

        //        TextPrint_Manager.Instance.PrintOver_Func(this);
        //    }
        //}
    }
    #endregion
    #region Abstract Data
    // 용도 : 부모 인터페이스에서 어느 타입인지 확인하고서 적합한 타입으로 다운 캐스팅한 후 데이터 Get하기
    // 개선 : 밸류들을 데이터용 클래스에 기록한 뒤 인자로 주고 받으면 어떨까? 매니저가 데이터 클래스를 풀링한 뒤 관리
    namespace AbstractData
    {
        public interface IAbstractData
        {
            AbstractDataType GetAbstractDataType_Func();
        }

        public interface IAD_Int : IAbstractData
        {
            int GetAD_Int_Func();
        }

        public interface IAD_Int_2 : IAbstractData
        {
            AD_Int_2 GetAD_Int_2_Func();
        }

        public interface IAD_Float_2 : IAbstractData
        {
            AD_Float_2 GetAD_Float_2_Func();
        }

        public interface IAD_Int_Float : IAbstractData
        {
            AD_Int_Float GetAD_Int_Float_Func();
        }

        public struct AD_Int_2 { public int value1, value2; }
        public struct AD_Int_Float { public int intValue; public float floatValue; }
        public struct AD_Float_2 { public float value1, value2; }

        public enum AbstractDataType
        {
            None = 0,

            Int,
            Int_2,

            Int_Float,

            Float2,

            Vector2,

            Vector3,
        }
    }
    #endregion

    // Coming Soon...
    #region Dragger
    // Potion 게임에서 쓰던 SelectMatter를 범용적으로 모듈화하여 WhichOne처럼 쓸모있게 만들자
    // 1. 끌고 다니는게, 선택한 객체 그 자체일 수도 있고, 새로운 드래깅 객체일 수도 있고 ㅇㅇ
    // 2. 드래그의 동기화 속도를 조절 가능하게끔
    #endregion
    #region Sound System
    #endregion
    #region Trash Group
    namespace Trash
    {
        public class RaritySort
        {
            // 임의로 명명한 등급 순으로 영웅을 정렬하고 싶을 때 어떻게 하는가?

            public string[] fixRarityArr = { "SSS", "SS", "S", "AAA", "A", "B", "C", "D" };
            public List<hero_dic_info> hero_Dic_Info_Item;

            public class hero_dic_info
            {
                public string rarity;

                public hero_dic_info(string _rarity)
                {
                    this.rarity = _rarity;
                }
            }

            private void Start()
            {
                hero_Dic_Info_Item = new List<hero_dic_info>();

                this.ReadCsv_Func(this.hero_Dic_Info_Item);

                this.Sort_Func(this.hero_Dic_Info_Item);

                this.PrintDesc_Func(this.hero_Dic_Info_Item);
            }

            private void ReadCsv_Func(List<hero_dic_info> _setList)
            {
                for (int i = 0; i < 10; i++)
                {
                    int _randRarityID = UnityEngine.Random.Range(0, fixRarityArr.Length);
                    string _randRarity = fixRarityArr[_randRarityID];
                    hero_dic_info _info = new hero_dic_info(_randRarity);
                    _setList.Add(_info);
                }
            }

            private void Sort_Func(List<hero_dic_info> _sortList)
            {
                for (int x = 0; x < _sortList.Count - 1; x++)
                {
                    for (int y = x + 1; y < _sortList.Count; y++)
                    {
                        hero_dic_info _x = _sortList[x];
                        hero_dic_info _y = _sortList[y];

                        int _xRarityID = this.GetRarityID_Func(_x);
                        int _yRarityID = this.GetRarityID_Func(_y);

                        if (_yRarityID < _xRarityID)
                        {
                            this.SwapHero_Func(ref _x, ref _y);

                            _sortList[x] = _x;
                            _sortList[y] = _y;

                            continue;
                        }
                        else
                        {

                        }
                    }
                }
            }

            private int GetRarityID_Func(hero_dic_info _heroInfoClass)
            {
                for (int _rarity = 0; _rarity < this.fixRarityArr.Length; _rarity++)
                {
                    if (_heroInfoClass.rarity != this.fixRarityArr[_rarity])
                    {

                    }
                    else
                    {
                        return _rarity;
                    }
                }

                Debug_C.Error_Func("해당 등급이 없다능");

                return -1;
            }

            private void SwapHero_Func(ref hero_dic_info _x, ref hero_dic_info _y)
            {
                hero_dic_info _temp = _x;

                _x = _y;

                _y = _temp;
            }

            private void PrintDesc_Func(List<hero_dic_info> _printList)
            {
                for (int i = 0; i < _printList.Count; i++)
                {
                    Debug.Log(i + " / " + _printList[i].rarity);
                }
            }
        }
    }
    #endregion
    #region Wrapping
    namespace Wrapping
    {
        // GC 없이 Enum을 Int로 캐스팅하는 내용인데, 다른 용도로도 쓸 수 있을 듯?

        class WrapperObject<TEnum, TValue>
        {
            TValue[] data;

            static Dictionary<TEnum, int> _enumKey = new Dictionary<TEnum, int>();

            static WrapperObject()
            {
                int[] intValues = Enum.GetValues(typeof(TEnum)) as int[];
                TEnum[] enumValues = Enum.GetValues(typeof(TEnum)) as TEnum[];

                for (int i = 0; i < intValues.Length; i++)
                {
                    _enumKey.Add(enumValues[i], intValues[i]);
                }
            }

            public WrapperObject(int count)
            {
                data = new TValue[count];
            }

            public TValue this[TEnum key]
            {
                get { return data[_enumKey[key]]; }
                set { data[_enumKey[key]] = value; }
            }
        }
    }
    #endregion
    #region Sort
    namespace Sort
    {
        // 정렬 알고리즘 ㄱㄱ
    }
    #endregion 

    public enum DirectionType
    {
        Left = -2,
        Down = -1,
        None = 0,
        Up = 1,
        Right = 2,

        //UpRight, // ↗
        //UpLeft, // ↖
        //DownRight, // ↘
        //DownLeft, // ↙
    }
}