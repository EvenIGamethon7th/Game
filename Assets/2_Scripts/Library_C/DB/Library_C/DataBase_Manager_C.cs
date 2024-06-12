using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

public partial class DataBase_Manager : Cargold.FrameWork.DataBase_Manager
{
    private static DataBase_Manager instance;
    public static new DataBase_Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<DataBase_Manager>("DataBase_Manager");
            }

            return instance;
        }
    }

    [SerializeField] private Debug_C.PrintLogType printLogType = Debug_C.PrintLogType.Common;
    #region Variable
    
    [InlineEditor, LabelText("Define"), SerializeField] private DB_DefineDataGroup define;
    public DB_DefineDataGroup GetDefine
    {
        get
        {
            if (this.define == null)
                this.define = Resources.Load<DB_DefineDataGroup>(base.dataGroupSobjPath + "DB_DefineDataGroup");

            return this.define;
        }
    }
    [InlineEditor, LabelText("Localize"), SerializeField] private DB_LocalizeDataGroup localize;
    public DB_LocalizeDataGroup GetLocalize
    {
        get
        {
            if (this.localize == null)
                this.localize = Resources.Load<DB_LocalizeDataGroup>(base.dataGroupSobjPath + "DB_LocalizeDataGroup");

            return this.localize;
        }
    }
    [InlineEditor, LabelText("Item"), SerializeField] private DB_ItemDataGroup item;
    public DB_ItemDataGroup GetItem
    {
        get
        {
            if (this.item == null)
                this.item = Resources.Load<DB_ItemDataGroup>(base.dataGroupSobjPath + "DB_ItemDataGroup");

            return this.item;
        }
    }
    #endregion

    #region Library
    
    #endregion

    protected override Debug_C.PrintLogType GetPrintLogType => this.printLogType;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);
        
        if(_layer == 0)
        {
            Debug_C.Init_Func(this);

            
            this.define.Init_Func();
            this.localize.Init_Func();
            this.item.Init_Func();
        }
    }

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(bool _isDataImport = true)
    {
        this.GetDefine.CallEdit_OnDataImportDone_Func();
        this.GetLocalize.CallEdit_OnDataImportDone_Func();
        this.GetItem.CallEdit_OnDataImportDone_Func();
        
        base.CallEdit_OnDataImport_Func();
    }
#endif
}