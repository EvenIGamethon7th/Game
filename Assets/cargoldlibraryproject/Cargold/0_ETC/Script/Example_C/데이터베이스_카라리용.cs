using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;
using LibraryTableDataType = Cargold.LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData.LibraryTableData.LibraryTableDataType;
using Cargold.Dialogue;
using Cargold.SDK.Purchase;

public class 데이터베이스_카라리용 : Cargold.FrameWork.DataBase_Manager
{
    /**/
    public override IDB_Localize GetLocalize_C
    {
        get
        {
            return default;
        }
    }
    /**/
    public override IDB_Dialogue GetDialogue_C
    {
        get
        {
            return default;
        }
    }
    /**/
    public override IDB_Inapp GetInapp_C
    {
        get
        {
            return default;
        }
    }
    /**/

    protected override Debug_C.PrintLogType GetPrintLogType => throw new System.NotImplementedException();
}