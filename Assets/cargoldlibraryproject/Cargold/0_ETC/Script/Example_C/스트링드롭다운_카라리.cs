using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
public class 스트링드롭다운_카라리Sobj : SobjDropdown
{
    public const string Str = "스트링드롭다운_카라리";

    private static 스트링드롭다운_카라리Sobj instance;
    public static 스트링드롭다운_카라리Sobj Instance
    {
        get
        {
            if(instance == null)
                instance = Resources.Load<스트링드롭다운_카라리Sobj>(CargoldLibrary_C.GetSobjPathS + 스트링드롭다운_카라리Sobj.Str);

            return instance;
        }
    }
}

[System.Serializable, InlineProperty, HideLabel]
public partial struct 스트링드롭다운_카라리
{
    // Const

#if UNITY_EDITOR
    private IEnumerable GetIEnumerable => 스트링드롭다운_카라리Sobj.Instance.GetIEnumerable;
#endif
    public new string ToString()
    {
#if UNITY_EDITOR
        return 스트링드롭다운_카라리Sobj.Instance.CallEdit_ToString_Func(this.ID); 
#endif

        return this.ID.ToString();
    }
}
} // End