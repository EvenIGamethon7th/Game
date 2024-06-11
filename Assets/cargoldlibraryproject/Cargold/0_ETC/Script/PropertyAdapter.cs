using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.Example
{
    public abstract class PropertyAdapter : SerializedMonoBehaviour
    {
        public abstract string GetLibraryClassType { get; }

#if UNITY_EDITOR
        [Button("컴포넌트 연결")]
        public virtual void CallEdit_AddComponent_Func()
        {
            string _typeStr = this.GetLibraryClassType;
            Type _type = Type.GetType(_typeStr);
            Debug_C.Log_Func($"다음의 프로퍼티 어댑터에서 컴포넌트 연결을 시도합니다. {_typeStr}({_type})");

            if (this.gameObject.TryGetComponent(_type, out Component _component) == false)
                _component = this.gameObject.AddComponent(_type);

            IPropertyAdapter _iExampleData = _component as IPropertyAdapter;
            _iExampleData.CallEdit_AddComponent_Func(this);

            if (_component == null)
                Debug_C.Error_Func("프로퍼티 어댑터) 다음 타입의 컴포넌트에 문제가 있습니다. : " + _typeStr);
        }

        public virtual bool CallEdit_TryAddComponent_Func<T>(out T _component) where T : Component
        {
            string _typeStr = this.GetLibraryClassType;
            Type _type = Type.GetType(_typeStr);
            Debug_C.Log_Func($"다음의 프로퍼티 어댑터에서 컴포넌트 연결을 시도합니다. {_typeStr}({_type})");

            if (this.gameObject.TryGetComponent(out _component) == false)
                _component = this.gameObject.AddComponent(_type) as T;

            IPropertyAdapter _iExampleData = _component as IPropertyAdapter;
            _iExampleData.CallEdit_AddComponent_Func(this);

            if(_component is null == false)
            {
                return true;
            }
            else
            {
                Debug_C.Error_Func("프로퍼티 어댑터) 다음 타입의 컴포넌트에 문제가 있습니다. : " + _typeStr);
                return false;
            }
        }
#endif
    }

    public interface IPropertyAdapter
    {
#if UNITY_EDITOR
        void CallEdit_AddComponent_Func<T>(T _exampleData) where T : PropertyAdapter; 
#endif
    }
}