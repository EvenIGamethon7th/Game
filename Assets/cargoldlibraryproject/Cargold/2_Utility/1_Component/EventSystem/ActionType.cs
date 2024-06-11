using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

public partial struct ActionType
{
    public const int None = 0;

    // 라이브러리에서 사용되는 키는 20억 이후로 세팅할 것
    public const int 다이얼로그 = 2000000000;
    //public const int 포커싱 = 2000000010;

    [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
    public int ID;

    public static implicit operator int(ActionType value)
    {
        return value.ID;
    }
    public static implicit operator ActionType(int value)
    {
        ActionType _data = new ActionType();
        _data.ID = value;

        return _data;
    }
}