using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
public class 스트링드롭다운Sobj : SobjDropdown
{
    public const string Str = "스트링드롭다운";

    private static 스트링드롭다운Sobj instance;
    public static 스트링드롭다운Sobj Instance
    {
        get
        {
            if(instance == null)
                instance = Resources.Load<스트링드롭다운Sobj>(CargoldLibrary_C.GetSobjPathS + 스트링드롭다운Sobj.Str);

            return instance;
        }
    }
}

[System.Serializable, InlineProperty, HideLabel]
public partial struct 스트링드롭다운
{
    public const int None = 0;
// Const

    [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
    public int ID;

#if UNITY_EDITOR
        private IEnumerable GetIEnumerable => 스트링드롭다운Sobj.Instance.GetIEnumerable;
#endif

    public static implicit operator int(스트링드롭다운 value)
    {
        return value.ID;
    }
    public static implicit operator 스트링드롭다운(int value)
    {
        스트링드롭다운 _data = new 스트링드롭다운();
        _data.ID = value;

        return _data;
    }
    public override string ToString()
    {
#if UNITY_EDITOR
        return 스트링드롭다운Sobj.Instance.CallEdit_ToString_Func(this.ID);
#endif

        return this.ID.ToString();
    }
}
} // End