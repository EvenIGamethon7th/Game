using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class AcademyClassKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("AcademyClass Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public AcademyClassData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetAcademyClass.TryGetData_Func(this.key, out AcademyClassData _academyClassData);

            return _academyClassData;
        }
    }

    public AcademyClassKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetAcademyClass.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetAcademyClass.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(AcademyClassKey _key)
    {
        return _key.key;
    }
}
