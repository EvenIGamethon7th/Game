using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

public partial struct ConditionType
{
    public const int None = 0;

    // 라이브러리에서 사용되는 키는 20억 이후로 세팅할 것
    //public const int UI_Normal = 2000000000;

    [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
    public int ID;

    public static implicit operator int(ConditionType value)
    {
        return value.ID;
    }
    public static implicit operator ConditionType(int value)
    {
        ConditionType _data = new ConditionType();
        _data.ID = value;

        return _data;
    }
}