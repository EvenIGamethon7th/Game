using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

public abstract class BaseData_C<T> : SerializedScriptableObject
{
    [SerializeField, LabelText("데이터")] protected T value;

    public void SetValue_Func(T _value)
    {
        this.value = _value;

#if UNITY_EDITOR
        Editor_C.SetSaveAsset_Func(this);
#endif
    }
}

public abstract class SingleData_C<T> : BaseData_C<T>
{
    public T GetValue => base.value;
}