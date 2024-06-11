using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Cargold;
using System.Collections.Generic;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.DB.TableImporter
{
    public class TableImporter_C
    {
        public const string Str = "TableImport";

        public static bool isProccessAll = true;
        public static TableImporterData GetData => LibraryRemocon.Instance != null ? LibraryRemocon.Instance.frameWorkData.databaseData.tableImporterData : null;

        public static void CheckGoogleAccessVaild_Func(int _remoconID)
        {
#if UNITY_EDITOR
            TableImporter_GoogleAPI.GoogleAccessVaild_Func(_remoconID);
#endif
        }
        public static void CheckGoogleAccessVaildDone_Func(int _remoconID)
        {
#if UNITY_EDITOR
            TableImporterData.Instance.tableImportTime_Edit = DateTime.Now;
#endif

            TableImporter_C.ExcelDownload(true, _remoconID);
        }

        public static void ExcelDownload(bool _isProccessAll, int _remoconID)
        {
            TableImporter_C.isProccessAll = _isProccessAll;

#if UNITY_EDITOR
            TableImporterData.Instance.tableImportTime_Edit = DateTime.Now;

            TableImporter_ExcelDownload.ExcelDownload(_remoconID); 
#endif
        }
        public static void ExcelDownloadDone_Func(int _remoconID)
        {
            if(TableImporter_C.isProccessAll == true)
            {
                TableImporter_C.OnXlsxToJson(_remoconID);
            }
        }

        public static void OnXlsxToJson(int _remoconID)
        {
#if UNITY_EDITOR
            TableImporter_ExcelToJson.ToJson_Func(_remoconID); 
#endif
        }
        public static void OnXlsxToJsonDone_Func(int _remoconID)
        {
            if (TableImporter_C.isProccessAll == true)
            {
                TableImporter_C.OnGenerateAndImport();
            }
        }

        public static void OnGenerateAndImport()
        {
#if UNITY_EDITOR
            TableImporter_Generate.OnGenerateScript_Func();
#endif
        }
        public static void OnGenerateAndImportDone_Func()
        {
#if UNITY_EDITOR
            if (TableImporter_C.isProccessAll == true)
            {
                TimeSpan _passTime = DateTime.Now - TableImporterData.Instance.tableImportTime_Edit;
                Debug_C.Log_Func("테이블 임포트 가주왁 끝... 총 " + ((float)_passTime.TotalSeconds).ToString_Second_Func(TableImporterData.Instance.exPoint) + "초 소요");
            }

            TableImporter_C.isProccessAll = false;

            TableImporterData _tableImporterData = GetData;

            string _dbManagerName = _tableImporterData.GetDBMClassName_Func(false);
            string _dbmPath = StringBuilder_C.Append_Func(_tableImporterData.GetResourceFullPath, _dbManagerName, ".asset");
            FrameWork.DataBase_Manager _dbmClass = Editor_C.GetLoadAssetAtPath_Func<FrameWork.DataBase_Manager>(_dbmPath);
            _tableImporterData.SetDbmManager_Func(_dbmClass);

            // 테이블 연동한 라이브러리
            foreach (TableImporterData.LibraryTableData _libraryTableData in _tableImporterData.libraryTableDataList)
            {
                TableImporterData.LibraryTableData.LibraryTableDataType _libraryTableDataType = _libraryTableData.GetLibraryTableDataType;

                // 230813 현재는 하나니까 if문이지만, 늘어나면 switch로 ㄱㄱ
                if (_libraryTableDataType == TableImporterData.LibraryTableData.LibraryTableDataType.Dialogue)
                {
                    LibraryRemocon.UtilityClassData.DialogueData.Instance.CallEdit_CallbackKeyGenerate_Func(false);

                    break;
                }
            }
#endif
        }
    }

#region Member
    [System.Serializable]
    public class SheetData
    {
        public string sheetName;
        public ColumnInfo[] columnInfoArr;
        public List<string[]> cellDataArrList; // 리스트 ID = 행 ID, 배열 ID = 열 ID

        public int GetRowNum => this.cellDataArrList.Count;
        public int GetColNum => this.columnInfoArr.Length;

        public SheetData()
        {

        }
        public SheetData(string _sheetName, int _colNum)
        {
            this.sheetName = _sheetName;

            ColumnInfo[] _columnInfoArr = new ColumnInfo[_colNum];

            for (int i = 0; i < _colNum; i++)
                _columnInfoArr[i] = new ColumnInfo();

            ColumnInfo _columnInfo = _columnInfoArr[0];
            _columnInfo.name = "Key";
            _columnInfo.type = ColumnType.String;

            this.columnInfoArr = _columnInfoArr;

            this.cellDataArrList = new List<string[]>();
        }

        public string[] GetCellDataArr_Func(int _rowID)
        {
            while (this.cellDataArrList.Count <= _rowID)
            {
                string[] _cellDataArr = new string[this.GetColNum];
                this.cellDataArrList.Add(_cellDataArr);
            }

            return this.cellDataArrList[_rowID];
        }
    }
    [System.Serializable]
    public class ColumnInfo
    {
        public string korName;
        public string name;
        public ColumnType type;
        public string[] typeParamArr;

        public bool IsHave => this.name.IsNullOrWhiteSpace_Func() == false && this.type != ColumnType.Undefined;
    }

    [System.Flags]
    public enum ColumnType
    {
        Undefined = 0,
        Bool = 1 << 1,
        Int = 1 << 2,
        Float = 1 << 3,
        String = 1 << 4,

        List_Bool = 1 << 5,
        List_Int = 1 << 6,
        List_Float = 1 << 7,
        List_String = 1 << 8,
        List_Infinite = 1 << 13,
        List = List_Bool | List_Int | List_Float | List_String | List_Infinite,

        Asset = 1 << 9,
        Enum = 1 << 10,
        Nested = 1 << 11,

        Infinite = 1 << 12,
        DateTime = 1 << 14,
        DateTimeTick = 1 << 15,
    }
#endregion
}