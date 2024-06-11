using Sirenix.OdinInspector;
using UnityEngine;

public partial struct BgmType
{
    public const int None = -1;

    [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
    public int ID;

    public static implicit operator int(BgmType value)
    {
        return value.ID;
    }
    public static implicit operator BgmType(int value)
    {
        BgmType _data = new BgmType();
        _data.ID = value;

        return _data;
    }
}