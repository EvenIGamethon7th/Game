using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class StoryKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("Story Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public StoryData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetStory.TryGetData_Func(this.key, out StoryData _storyData);

            return _storyData;
        }
    }

    public StoryKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetStory.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetStory.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(StoryKey _key)
    {
        return _key.key;
    }
}
