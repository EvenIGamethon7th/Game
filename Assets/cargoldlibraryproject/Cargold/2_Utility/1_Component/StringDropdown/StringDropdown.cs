using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    public static class DropdownDefine
    {
        public const float LabelWidth = 125f;
        public const float ValueWidth = 0f;
    }

    #region StringDropdown
    [System.Serializable, InlineProperty]
    public abstract class StringDropdown
    {
        [SerializeField, LabelText("$CallEdit_GetKeyName"), LabelWidth(150f), ValueDropdown("CallEdit_GetKeyArr_Func"), OnValueChanged("CallEdit_OnKeyChanged_Func")]
        protected string key;

        public virtual string GetKey => this.key;

        public StringDropdown(string _key)
        {
            this.key = _key;
        }

#if UNITY_EDITOR
        protected abstract IEnumerable CallEdit_GetKeyArr_Func();
        protected virtual string CallEdit_GetKeyName => string.Empty;
        protected virtual void CallEdit_OnKeyChanged_Func() { }
#endif

        public static implicit operator string(StringDropdown _value)
        {
            return _value.GetKey;
        }
    } 
    #endregion
    #region ObjDropdownKey
    [System.Serializable, InlineProperty]
    public abstract class ObjDropdownKey<T> where T : UnityEngine.Object
    {
        [LabelWidth(DropdownDefine.LabelWidth)]
        [SerializeField, LabelText("$CallEdit_GetKeyName"), ValueDropdown("CallEdit_GetKeyArr_Func"), OnValueChanged("CallEdit_OnKeyChanged_Func")] protected T key;
        [ShowInInspector, HideLabel] private T GetPreview => this.key;

        public virtual T GetKey => this.key;

        public ObjDropdownKey(T _key)
        {
            SetKey_Func(_key);
        }

        public void SetKey_Func(T _key)
        {
            this.key = _key;
        }

#if UNITY_EDITOR
        protected abstract IEnumerable CallEdit_GetKeyArr_Func();
        protected virtual string CallEdit_GetKeyName => string.Empty;
        protected virtual void CallEdit_OnKeyChanged_Func() { }
#endif

        public static implicit operator T(ObjDropdownKey<T> _value)
        {
            return _value.GetKey;
        }
        public static implicit operator string(ObjDropdownKey<T> _value)
        {
            return _value.GetKey.name;
        }
    } 
    #endregion
    #region ObjDropdownItem
    [System.Serializable]
    public class ObjDropdownItem<T> : IValueDropdownItem where T : UnityEngine.Object
    {
        [SerializeField] private T obj;

        public T GetObj => this.obj;

        public ObjDropdownItem(T _obj)
        {
            this.Init_Func(_obj);
        }
        public void Init_Func(T _obj)
        {
            this.obj = _obj;
        }

        public string GetText()
        {
#if UNITY_EDITOR
            if (obj == null)
            {
                System.Type _type = typeof(T);

                Debug_C.Error_Func(_type + " 비어있음!");
                return null;
            }
#endif

            return obj.name;
        }
        public T GetValue_Func()
        {
            return this.obj;
        }
        public object GetValue()
        {
            return this.obj;
        }
    } 
    #endregion
    #region ObjDropdownContainer
    [System.Serializable]
    public abstract class ObjDropdownContainer<ObjType, ObjDropdownItem, ObjDropdownKey>
    where ObjType : UnityEngine.Object
    where ObjDropdownItem : Cargold.ObjDropdownItem<ObjType>
    where ObjDropdownKey : Cargold.ObjDropdownKey<ObjType>
    {
        [SerializeField, LabelText("드롭다운 목록")] private List<ObjDropdownItem> objDropdownItemList = new List<ObjDropdownItem>();

        public List<ObjDropdownItem> GetObjDropdownItemList => this.objDropdownItemList;

        protected abstract ObjDropdownItem GetInstance_Func(ObjType _obj);

        public void OnResetList_Func()
        {
            this.objDropdownItemList.Clear();
        }

#if UNITY_EDITOR
        public bool CallEdit_AddItem_Func(ObjType _obj, bool _isLog = true)
        {
            string _objName = _obj.name;

            foreach (ObjDropdownItem _objDropdownItem in this.objDropdownItemList)
            {
                if (_objDropdownItem.GetText().IsCompare_Func(_objName) == true)
                {
                    return false;
                }
            }

            ObjDropdownItem _newObjDropdownItem = this.GetInstance_Func(_obj);
            this.objDropdownItemList.Add(_newObjDropdownItem);

            if (_isLog == true)
            {
                Debug_C.Log_Func(typeof(ObjType).Name + "에 새로운 Key가 등록되었습니다. : " + _objName);
            }

            return true;
        }
        public bool CallEdit_RemoveItem_Func(ObjType _obj)
        {
            string _objName = _obj.name;

            foreach (ObjDropdownItem _objDropdownItem in this.objDropdownItemList)
            {
                if (_objDropdownItem.GetText().IsCompare_Func(_objName) == true)
                {
                    this.objDropdownItemList.Remove_Func(_objDropdownItem);

                    return true;
                }
            }

            Debug_C.Error_Func("다음 프랍 위치는 DB에 기록되지 않았습니다. : " + _obj.name);

            return false;
        }
#endif 
    }
    #endregion
}