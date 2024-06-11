using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


public class SfxTypeSobj : SobjDropdown
{
    public const string Str = "SfxType";

    private static SfxTypeSobj instance;
    public static SfxTypeSobj Instance
    {
        get
        {
            if(instance == null)
                instance = Resources.Load<SfxTypeSobj>(CargoldLibrary_C.GetSobjPathS + SfxTypeSobj.Str);

            return instance;
        }
    }
}

[System.Serializable, InlineProperty, HideLabel]
public partial struct SfxType
{
    // Const

#if UNITY_EDITOR
    private IEnumerable GetIEnumerable => SfxTypeSobj.Instance.GetIEnumerable;
#endif
    public new string ToString()
    {
#if UNITY_EDITOR
        return SfxTypeSobj.Instance.CallEdit_ToString_Func(this.ID); 
#endif

        return this.ID.ToString();
    }
}
