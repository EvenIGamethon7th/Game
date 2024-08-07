using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable, InlineProperty, HideLabel]
public partial class ProductDetailsKey : Cargold.FrameWork.TableDataKeyDropdown
{
    [LabelWidth(DropdownDefine.LabelWidth)]
    [SerializeField, LabelText("ProductDetails Key"), ValueDropdown("CallEdit_KeyDropdown_Func")] private string key = null;

    public string GetKey => this.key;

    [ShowInInspector, ReadOnly, HideLabel, FoldoutGroup("Preview")]
    public ProductDetailsData GetData
    {
        get
        {
            DataBase_Manager.Instance.GetProductDetails.TryGetData_Func(this.key, out ProductDetailsData _productDetailsData);

            return _productDetailsData;
        }
    }

    public ProductDetailsKey(string _keyStr = null)
    {
        this.key = _keyStr;
    }

#if UNITY_EDITOR
    private IEnumerable<string> CallEdit_KeyDropdown_Func()
    {
        return DataBase_Manager.Instance.GetProductDetails.GetKeyArr;
    }
    public bool CallEdit_IsUnitTestDone_Func()
    {
        if(this.key.IsNullOrWhiteSpace_Func() == false)
            return DataBase_Manager.Instance.GetProductDetails.IsContain_Func(this.key);

        return false;
    }
#endif

    public static implicit operator string(ProductDetailsKey _key)
    {
        return _key.key;
    }
}
