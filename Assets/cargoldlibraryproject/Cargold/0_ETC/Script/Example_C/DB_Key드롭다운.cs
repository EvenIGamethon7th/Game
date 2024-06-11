using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
[System.Serializable, InlineProperty, HideLabel]
public partial class DB_Key드롭다운 : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("Example Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public ExampleData GetData
    {
        get
        {
            드롭다운DB매니저.Instance.GetExample.TryGetData_Func(this.key, out ExampleData _exampleData);

            return _exampleData;
        }
    }

    public DB_Key드롭다운(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return 드롭다운DB매니저.Instance.GetExample.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return 드롭다운DB매니저.Instance.GetExample.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(DB_Key드롭다운 _key)
    {
        return _key.key;
    }
}
} // End