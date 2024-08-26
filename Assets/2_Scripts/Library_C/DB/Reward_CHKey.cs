using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class Reward_CHKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("Reward_CH Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public Reward_CHData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetReward_CH.TryGetData_Func(this.key, out Reward_CHData _reward_CHData);

            return _reward_CHData;
        }
    }

    public Reward_CHKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetReward_CH.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetReward_CH.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(Reward_CHKey _key)
    {
        return _key.key;
    }
}
