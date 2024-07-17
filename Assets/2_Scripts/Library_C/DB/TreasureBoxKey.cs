using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class TreasureBoxKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("TreasureBox Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public TreasureBoxData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetTreasureBox.TryGetData_Func(this.key, out TreasureBoxData _treasureBoxData);

            return _treasureBoxData;
        }
    }

    public TreasureBoxKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetTreasureBox.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetTreasureBox.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(TreasureBoxKey _key)
    {
        return _key.key;
    }
}
