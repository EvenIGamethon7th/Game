using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class AssetLibraryData
        {
            [FoldoutGroup(InappPurchaseData.KorStr), Indent(InappPurchaseData.IndentLv)]
            public partial class InappPurchaseData // 메인
            {
                public const string KorStr = "인앱 결제";
                public const string Str = "Inapp";
                public const int IndentLv = AssetLibraryData.IndentLv + 1;

                [LabelText("무반응 강제 종료 시간")] public float unresponseCancleTime = 10f;
                [InlineProperty, HideLabel] public UnityPurchaseData unityPurhcaseData = new UnityPurchaseData();

                public void Init_Func()
                {
                    this.unityPurhcaseData.Init_Func();
                }

                [FoldoutGroup(UnityPurchaseData.KorStr), Indent(UnityPurchaseData.IndentLv)]
                public class UnityPurchaseData : ScriptGenerate // 메인
                {
                    public const string KorStr = "유니티 인앱";
                    public const int IndentLv = InappPurchaseData.IndentLv + 1;

                    [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField] private TableData libraryTableData = new TableData();

                    protected override string GetClassNameDefault => "InappSystem_Manager";
#if Purchase_Unity_C
                    protected override Type GetExampleType => typeof(Cargold.Example.인앱결제매니저_유니티인앱);
#else
                    protected override Type GetExampleType => throw new Exception("디파인 심볼에 Purchase_Unity_C을 추가하세요");
#endif

                    public override void Init_Func()
                    {
                        base.Init_Func();

                        base.subFolderPathArr = new string[1] { InappPurchaseData.Str };

                        this.libraryTableData.Init_Func(InappPurchaseData.Str, typeof(Cargold.Example.DB_인앱));
                    }

                    [LabelText(KorStr)]
                    public class TableData : LibraryTableData
                    {
                        [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("구글 ID")] public string googleID = "googleID";
                        [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("애플 ID")] public string appleID = "appleID";
                        [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("프로덕트 ID")] public string productTypeID = "productTypeID";

                        protected override int GetDataVarNum => 3;
                        public override LibraryTableDataType GetLibraryTableDataType => LibraryTableDataType.Inapp;

                        public override string GetDataVarStr_Func(int _id)
                        {
                            switch (_id)
                            {
                                case 0:
                                    return base.GetDataVarStr_Func(this.googleID);

                                case 1:
                                    return base.GetDataVarStr_Func(this.appleID);

                                case 2:
                                    return base.GetDataVarStr_Func(this.productTypeID);

                                default:
                                    Debug_C.Error_Func("_id : " + _id);
                                    return default;
                            }
                        }
                    }
                }
            }
        }
    }
}