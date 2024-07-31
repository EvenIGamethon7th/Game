using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class UnitEnchantKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("UnitEnchant Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public UnitEnchantData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetUnitEnchant.TryGetData_Func(this.key, out UnitEnchantData _unitEnchantData);

            return _unitEnchantData;
        }
    }

    public UnitEnchantKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetUnitEnchant.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetUnitEnchant.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(UnitEnchantKey _key)
    {
        return _key.key;
    }
}
