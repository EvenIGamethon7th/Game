using Cargold.DB.TableImporter;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class FrameWorkData
        {
            [FoldoutGroup(DatabaseData.KorStr), Indent(FrameWorkData.IndentLv)]
            public partial class DatabaseData // 메인
            {
                public const string KorStr = "데이터베이스";
                public const string Str = "DB";
                public const int IndentLv = FrameWorkData.IndentLv + 1;

                public static DatabaseData Instance => FrameWorkData.Instance.databaseData;

                [InlineProperty, HideLabel] public TableImporterData tableImporterData = new TableImporterData();

                [LabelText("DB 매니저 클래스 이름")] public string dbManagerClassName = "DataBase_Manager";

                public void Init_Func()
                {
                    this.tableImporterData.Init_Func();
                }
            }
        }
    }
}
