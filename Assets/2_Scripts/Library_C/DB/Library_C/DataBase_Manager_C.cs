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
    [InlineEditor, LabelText("Monster"), SerializeField] private DB_MonsterDataGroup monster;
    public DB_MonsterDataGroup GetMonster
    {
        get
        {
            if (this.monster == null)
                this.monster = Resources.Load<DB_MonsterDataGroup>(base.dataGroupSobjPath + "DB_MonsterDataGroup");

            return this.monster;
        }
    }
    [InlineEditor, LabelText("Wave"), SerializeField] private DB_WaveDataGroup wave;
    public DB_WaveDataGroup GetWave
    {
        get
        {
            if (this.wave == null)
                this.wave = Resources.Load<DB_WaveDataGroup>(base.dataGroupSobjPath + "DB_WaveDataGroup");

            return this.wave;
        }
    }
    [InlineEditor, LabelText("Stage"), SerializeField] private DB_StageDataGroup stage;
    public DB_StageDataGroup GetStage
    {
        get
        {
            if (this.stage == null)
                this.stage = Resources.Load<DB_StageDataGroup>(base.dataGroupSobjPath + "DB_StageDataGroup");

            return this.stage;
        }
    }
    #endregion

    #region Library
    
            public override Cargold.FrameWork.IDB_Localize GetLocalize_C => this.localize;
            
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
            this.monster.Init_Func();
            this.wave.Init_Func();
            this.stage.Init_Func();
        }
    }

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(bool _isDataImport = true)
    {
        this.GetDefine.CallEdit_OnDataImportDone_Func();
        this.GetLocalize.CallEdit_OnDataImportDone_Func();
        this.GetItem.CallEdit_OnDataImportDone_Func();
        this.GetMonster.CallEdit_OnDataImportDone_Func();
        this.GetWave.CallEdit_OnDataImportDone_Func();
        this.GetStage.CallEdit_OnDataImportDone_Func();
        
        base.CallEdit_OnDataImport_Func();
    }
#endif
}