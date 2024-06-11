using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Effect;

public class Effect_OneShot_StringKey_Script : Cargold.Effect.Effect_OneShot_Script
{
    [SerializeField] private string poolingKey = null;

    public override string GetPoolingKey => this.poolingKey;

#if UNITY_EDITOR
    protected override void CallEdit_Catching_Func()
    {
        this.poolingKey = this.gameObject.name;

        base.CallEdit_Catching_Func();
    }
    protected override void CallEdit_Catching_Func(Effect_Element_Script[] _effElemClassArr)
    {
        base.CallEdit_Catching_Func(_effElemClassArr);

        base.CallEdit_Catching_PS_Func();
    }
#endif
}