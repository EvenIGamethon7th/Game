using Sirenix.OdinInspector;
using UnityEngine;
using Cargold;

public partial struct SfxType
{
    public const int None = 0;

    // 라이브러리에서 사용되는 효과음 ID는 20억 이후로 세팅할 것
    public const int UI_Normal = 2000000000;

    [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
    public int ID;

    public static implicit operator int(SfxType value)
    {
        return value.ID;
    }
    public static implicit operator SfxType(int value)
    {
        SfxType _data = new SfxType();
        _data.ID = value;

        return _data;
    }
}