using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.DB.TableImporter
{
    public class TableImporter_Generate
    {
        #region Const String
        public const string GetScriptGen_DataKeyClass = @"
public static partial class {0}
{{{1}
}}
";
        public const string GetScriptGen_DataKeyVar = @"
    public const string {0} = ""{1}"";";

        public const string GetScriptGen_Header =
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

";

        public const string GetScriptGen_DataBaseManager = GetScriptGen_Header +
@"public partial class {0} : Cargold.FrameWork.DataBase_Manager
{{
    private static {0} instance;
    public static new {0} Instance
    {{
        get
        {{
            if (instance == null)
            {{
                instance = Resources.Load<DataBase_Manager>(""{4}"");
            }}

            return instance;
        }}
    }}

    [SerializeField] private Debug_C.PrintLogType printLogType = Debug_C.PrintLogType.Common;
    #region Variable
    {1}
    #endregion

    #region Library
    {5}
    #endregion

    protected override Debug_C.PrintLogType GetPrintLogType => this.printLogType;

    public override void Init_Func(int _layer)
    {{
        base.Init_Func(_layer);
        
        if(_layer == 0)
        {{
            Debug_C.Init_Func(this);

            {2}
        }}
    }}

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(bool _isDataImport = true)
    {{{3}
        
        base.CallEdit_OnDataImport_Func();
    }}
#endif
}}";

        public const string GetScriptGen_DBM_Var = @"
    [InlineEditor, LabelText(""{3}""), SerializeField] private {0} {1};
    public {0} {2}
    {{
        get
        {{
            if (this.{1} == null)
                this.{1} = Resources.Load<{0}>(base.dataGroupSobjPath + ""{0}"");

            return this.{1};
        }}
    }}";

        public const string GetScriptGen_DBM_Init = @"
            this.{0}.Init_Func();";

        public const string GetScriptGen_DBM_Catch = @"
        this.{0}.CallEdit_OnDataImportDone_Func();";


        public const string GetScriptGen_DataGroup_C = GetScriptGen_Header +
@"[System.Serializable]
public partial class {0} : DataGroup_C<{1}>{2}
{{
   {3}
}}";

        public const string GetScriptGen_Data_C = GetScriptGen_Header +
@"[System.Serializable]
public partial class {0} : Data_C{3}
{{{1}

    {4}

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {{{2}
    }}
#endif
}}";

        public const string GetScriptGen_ClientMemo = @"
#if UNITY_EDITOR
    public override void CallEdit_OnDataImportDone_Func()
    {{
        base.CallEdit_OnDataImportDone_Func();

        /* 테이블 임포트가 모두 마무리된 뒤 마지막으로 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }}
#endif
";

        public const string GetScriptGen_Client = GetScriptGen_Header +
@"public partial class {0}
{{
    protected override void Init_Project_Func()
    {{
        base.Init_Project_Func();

        /* 런타임 즉시 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }}
"
   + GetScriptGen_ClientMemo + @"
}}";

        public const string GetScriptGen_LabelText = @"[LabelText(""{0}"")] ";
        public const string GetScriptGen_Var = @"
     {2}public {0} {1};";

        public const string GetScriptGen_IntVar = "int";
        public const string GetScriptGen_FloatVar = "float";
        public const string GetScriptGen_BoolVar = "bool";
        public const string GetScriptGen_StringVar = "string";
        public const string GetScriptGen_InfiniteVar = "Cargold.Infinite.Infinite";
        public const string GetScriptGen_DateTimeTickVar = "DateTimeTick";
        public const string GetScriptGen_IntArrVar = "int[]";
        public const string GetScriptGen_FloatArrVar = "float[]";
        public const string GetScriptGen_BoolArrVar = "bool[]";
        public const string GetScriptGen_StringArrVar = "string[]";
        public const string GetScriptGen_InfiniteArrVar = "Cargold.Infinite.Infinite[]";

        public const string GetScriptGen_IntFunc = @"
        {0} = _cellDataArr[{1}].ToInt();";
        public const string GetScriptGen_FloatFunc = @"
        {0} = _cellDataArr[{1}].ToFloat();";
        public const string GetScriptGen_BoolFunc = @"
        {0} = _cellDataArr[{1}].ToBool();";
        public const string GetScriptGen_StringFunc = @"
        {0} = _cellDataArr[{1}];";
        public const string GetScriptGen_InfiniteFunc = @"
        {0} = Cargold.Infinite.InfiniteExtensionMethod.ToInfinite(_cellDataArr[{1}]);";
        public const string GetScriptGen_IntArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = System.Array.ConvertAll(_strArr{1}, _str => _str.ToInt());";
        public const string GetScriptGen_FloatArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = System.Array.ConvertAll(_strArr{1}, _str => _str.ToFloat());";
        public const string GetScriptGen_BoolArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = System.Array.ConvertAll(_strArr{1}, _str => _str.ToBool());";
        public const string GetScriptGen_StringArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = _strArr{1};";
        public const string GetScriptGen_InfiniteArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = System.Array.ConvertAll(_strArr{1}, _str => Cargold.Infinite.InfiniteExtensionMethod.ToInfinite(_str));";
        public const string GetScriptGen_CustomFunc = @"
        {0} = Editor_C.GetLoadAssetAtPath_Func<{2}>(_cellDataArr[{1}]);";
        public const string GetScriptGen_EnumFunc = @"
        {0} = _cellDataArr[{1}].ToEnum<{2}>();";
        public const string GetScriptGen_Enum = @"
public enum {0}
{{
    None = 0,

{1}}}
";
        public const string GetScriptGen_NestedFunc = @"
        {0} = new {2}();
        {0}.CallEdit_OnDataImport_Func(_cellDataArr[{1}]);";
        public const string GetScriptGen_NestedArrFunc = GetScriptGen_ArrSplitFunc + @"
        {0} = System.Array.ConvertAll(_strArr{1}, _str =>
        {{
            {2} _data = null;
            if (_str.IsNullOrWhiteSpace_Func() == false)
            {{
                _data = new {2}();
                _data.CallEdit_OnDataImport_Func(_str);
                
            }}

            return _data;
        }});";
        public const string GetScriptGen_NestedClass = GetScriptGen_Header + @"
[System.Serializable]
public partial class {0}
{{
{1}

#if UNITY_EDITOR
    public void CallEdit_OnDataImport_Func(string _cellData)
    {{
        string[] _cellDataArr = _cellData.Split(""!"");

        {2}
    }}
#endif
}}
";
        public const string GetScriptGen_DateTimeTickFunc = @"
        {0} = System.Convert.ToDateTime(_cellDataArr[{1}]);";

        public const string GetScriptGen_ArrSplitFunc = @"
        string[] _strArr{1} = _cellDataArr[{1}].Split(',');
        {0} = new {2}[_strArr{1}.Length];";
        #endregion

#if UNITY_EDITOR
        private static string libraryScriptPathS;
        private static string GetLibraryScriptPathS
        {
            get
            {
                if(libraryScriptPathS.IsNullOrWhiteSpace_Func() == true)
                {
                    string _libraryScriptPath = GetData.GetLibraryScriptPath;
                    Editor_C.TryCheckOrGenerateFolder_Func(_libraryScriptPath);

                    string _projectPath = GetData.CallEdit_GetProjectPath_Func();

                    string _path = GetData.CallEdit_GetFullPath_Func(_libraryScriptPath, _projectPath);
                    libraryScriptPathS = StringBuilder_C.Append_Func(_path, Editor_C.SeparatorStr);
                }

                return libraryScriptPathS;
            }
        }

        private static TableImporterData GetData => TableImporter_C.GetData;

        public static void OnGenerateScript_Func()
        {
            TableImporterData.TableImporterRemocon[] _remoconDataArr = GetData.GetRemoconClassArr;
            foreach (TableImporterData.TableImporterRemocon _remoconData in _remoconDataArr)
            {
                int _remoconID = _remoconData.GetRemoconID;
                string _jsonFullPath = TableImporter_ExcelToJson.GetJsonFullPath_Func(_remoconID);
                if (File.Exists(_jsonFullPath) == false)
                {
                    Debug_C.Log_Func($"{_remoconID} 시트 리모콘의 Json 파일을 찾을 수 없습니다. 구글 엑셀을 다운로드 받은 뒤 Json으로 변환해주세요.");
                    return;
                }
            }

            TableImporterData.Instance.tableImportGenerateTime_Edit = DateTime.Now;
            Dictionary<string, SheetData> _sheetDic = new Dictionary<string, SheetData>();
            foreach (TableImporterData.TableImporterRemocon _remoconData in _remoconDataArr)
            {
                int _remoconID = _remoconData.GetRemoconID;
                string _jsonFullPath = TableImporter_ExcelToJson.GetJsonFullPath_Func(_remoconID);
                string _jsonTxt = File.ReadAllText(_jsonFullPath);
                Debug_C.Log_Func("_jsonTxt : " + _jsonTxt, Debug_C.PrintLogType.DB_Generate);
                Dictionary<string, SheetData> _dic = GetData.GetLibrary.GetDerialize_Func(_jsonTxt);

                foreach (var item in _dic)
                {
                    string _sheetNameStr = item.Key;
                    if (_sheetDic.ContainsKey(_sheetNameStr) == false)
                        _sheetDic.Add(item.Key, item.Value);
                    else
                        Debug_C.Error_Func(_remoconID + " 시트리모콘에서 다음 시트명이 중복됩니다. : " + _sheetNameStr);
                }
            }

            TableImporter_Generate.OnGenerateScript_Func(_sheetDic);
        }
        private static void OnGenerateScript_Func(Dictionary<string, SheetData> _sheetDataDic)
        {
            GetData.sheetDataDic = null;
            GetData.nestedGenerateDic = null;
            GetData.isEnumGenerate = false;

            // 생성될 스크립터블 오브젝트가 배치될 폴더 체크
            string _resourcePath = GetData.GetResourceFullPath;
            if (Directory.Exists(_resourcePath) == false)
            {
                Directory.CreateDirectory(_resourcePath);

                Debug_C.Log_Func("_resourcePath 폴더 생성 : " + _resourcePath, Debug_C.PrintLogType.DB_Generate);
            }
            else
            {
                Debug_C.Log_Func("_resourcePath 폴더 확인 : " + _resourcePath, Debug_C.PrintLogType.DB_Generate);
            }

            string _projectPath = GetData.CallEdit_GetProjectPath_Func();
            string _clientScriptPath = GetData.CallEdit_GetFullPath_Func(GetData.scriptPath, _projectPath) + Editor_C.SeparatorStr;
            Editor_C.TryCheckOrGenerateFolder_Func(_clientScriptPath);

            // Data Key
            List<string[]> _dataKeyList = new List<string[]>();

            // 라이브러리용 테이블 데이터
            List<TableImporterData.LibraryTableData> _libraryTableDataList = GetData.libraryTableDataList;

            // Enum용 데이터
            List<string> _checkedEnumStrList = null;

            #region 시트 데이터를 기준으로 스크립트 생성
            foreach (var _sheetDataItem in _sheetDataDic)
            {
                string _sheetNameStr = _sheetDataItem.Key;
                bool _isImport = GetData.IsSheetImport_Func(_sheetNameStr);
                SheetData _sheetData = _sheetDataItem.Value;
                if(_isImport == true)
                {
                    #region UID Key 추가
                    for (int i = 0; i < _sheetData.GetRowNum; i++)
                    {
                        string[] _cellDataArr = _sheetData.GetCellDataArr_Func(i);

                        bool _isVaild = true;

                        if (_cellDataArr[0].IsCompare_Func("#REF!") == true)
                            _isVaild = false;

                        if (_isVaild == true)
                        {
                            string[] _dataKeyArr = new string[]
                            {
                            StringBuilder_C.Append_Func(_sheetNameStr, "_", _cellDataArr[0]),
                            _cellDataArr[0]
                            };

                            _dataKeyList.Add(_dataKeyArr);
                        }
                        else
                        {
                            StringBuilder_C.Append_Func("엑셀에서 참조 오류가 발생했습니다.) 시트 : ", _sheetData.sheetName, " / 행 ID : ", i, " / 셀 : ", _cellDataArr[0]);
                        }
                    }
                    #endregion

                    #region Data Key 스크립트 생성
                    string _dataKeyClassName = GetData.GetDataKeyClassName;
                    OnGenerateConstKey_Func(_dataKeyClassName, _dataKeyList, GetLibraryScriptPathS, StringBuilder_C.Append_Func(_sheetNameStr, "_UID"));
                    _dataKeyList.Clear();
                    #endregion
                }

                string _dataClassName = GetData.GetDataClassName_Func(_sheetNameStr, false);

                if(_isImport == true)
                {
                    #region Group 클래스
                    string _dataGroupClassName = GetData.GetDataGroupClassName_Func(_sheetNameStr, false);
                    string _dataGroupClassNameC = GetData.GetDataGroupClassName_Func(_sheetNameStr, true);
                    string _dataGroupScriptPath = StringBuilder_C.Append_Func(GetLibraryScriptPathS, _dataGroupClassNameC, ".cs");

                    string _libraryDataGroupStr = default;
                    string _libraryDataGroupFuncStr = default;
                    if (_libraryTableDataList.IsHave_Func() == true)
                    {
                        foreach (TableImporterData.LibraryTableData _libraryTableData in _libraryTableDataList)
                        {
                            if (_sheetNameStr.IsCompare_Func(_libraryTableData.sheetName) == true)
                            {
                                _libraryDataGroupStr = _libraryTableData.dataGroupStr;
                                _libraryDataGroupFuncStr = _libraryTableData.dataGroupFuncStr;

                                break;
                            }
                        }
                    }

                    string _dataGroupScriptStr = string.Format(GetScriptGen_DataGroup_C, _dataGroupClassName, _dataClassName, _libraryDataGroupStr, _libraryDataGroupFuncStr);

                    File.WriteAllText(_dataGroupScriptPath, _dataGroupScriptStr);

                    // 클라이언트 스크립트
                    string _clientDataGroupScriptPath = StringBuilder_C.Append_Func(_clientScriptPath, _dataGroupClassName, ".cs");
                    if (File.Exists(_clientDataGroupScriptPath) == false)
                    {
                        Debug_C.Log_Func(_dataGroupClassName + " 스크립트 생성, 경로 : " + _clientDataGroupScriptPath);

                        string _clientDataGroupScriptStr = string.Format(GetScriptGen_Client, _dataGroupClassName);
                        File.WriteAllText(_clientDataGroupScriptPath, _clientDataGroupScriptStr);
                    }
                    else
                    {
                        Debug_C.Log_Func(_dataGroupClassName + " 스크립트 확인, 경로 : " + _clientDataGroupScriptPath, Debug_C.PrintLogType.DB_Generate);
                    }
                    #endregion

                    #region Data 클래스
                    string _dataVarStr = default;
                    string _dataFuncStr = default;
                    for (int _colID = 0; _colID < _sheetData.GetColNum; _colID++)
                    {
                        bool _isWriteVar = true;

                        ColumnInfo _columnInfo = _sheetData.columnInfoArr[_colID];
                        if (_columnInfo.IsHave == false)
                            break;

                        ColumnType _columnType = _columnInfo.type;
                        string _varTypeStr, _onImportFuncStr, _onImportFuncFormatStr = null;
                        Debug_C.Log_Func("칼럼 ID : " + _colID + " / 타입 : " + _columnType, Debug_C.PrintLogType.DB_Generate);
                        if (IsComplexColumnType_Func(_columnType, out _varTypeStr, out _onImportFuncStr, out _onImportFuncFormatStr) == true)
                        {
                            switch (_columnType)
                            {
                                // 복합 칼럼
                                #region Asset
                                case ColumnType.Asset:
                                    {
                                        _varTypeStr = _columnInfo.typeParamArr[0];
                                        _onImportFuncStr = GetScriptGen_CustomFunc;
                                        _onImportFuncFormatStr = _columnInfo.typeParamArr[0];
                                    }
                                    break;
                                #endregion
                                #region Enum
                                case ColumnType.Enum:
                                    {
                                        bool _isEnumAlready = false;
                                        if (_columnInfo.typeParamArr[0].Contains(GetData.GetColumnIngoreStr) == true)
                                        {
                                            _isEnumAlready = true;
                                            _columnInfo.typeParamArr[0] = _columnInfo.typeParamArr[0].Replace(GetData.GetColumnIngoreStr, "");
                                        }

                                        string _enumStr = _columnInfo.typeParamArr[0];

                                        _varTypeStr = _enumStr;
                                        _onImportFuncStr = GetScriptGen_EnumFunc;
                                        _onImportFuncFormatStr = _enumStr;

                                        // 이그노어인가?
                                        if (_isEnumAlready == true)
                                            break;

                                        if (GetData.enumGenerateListDic == null)
                                            GetData.enumGenerateListDic = new Dictionary<string, Dictionary<string, int>>();

                                        Dictionary<string, int> _originEnumDic = GetData.enumGenerateListDic.GetValue_Func(_enumStr, () => new Dictionary<string, int>());
                                        Dictionary<string, int> _newEnumDic = new Dictionary<string, int>();

                                        int _highestIndex = 0; // None이 0이라서 기본값을 0으로 시작
                                        if (_originEnumDic.IsHave_Func() == true)
                                        {
                                            foreach (var item in _originEnumDic)
                                            {
                                                if (_highestIndex < item.Value)
                                                    _highestIndex = item.Value;
                                            }
                                        }

                                        if (_checkedEnumStrList == null)
                                            _checkedEnumStrList = new List<string>();

                                        // 검사한 적 없는 Enum인가요?
                                        if (_checkedEnumStrList.Contains(_enumStr) == false)
                                        {
                                            _isEnumAlready = false;

                                            _checkedEnumStrList.Add(_enumStr);
                                        }
                                        else
                                        {
                                            _isEnumAlready = true;

                                            foreach (var item in _originEnumDic)
                                                _newEnumDic.Add(item.Key, item.Value);
                                        }

                                        int _rowNum = _sheetData.GetRowNum;

                                        for (int i = 0; i < _rowNum; i++)
                                        {
                                            string[] _cellDataArr = _sheetData.GetCellDataArr_Func(i);
                                            string _enumItemStr = _cellDataArr[_colID];

                                            // Item이 비어있나요?
                                            if (_enumItemStr.IsNullOrWhiteSpace_Func() == true)
                                                continue;

                                            // 이미 있는 Item인가요?
                                            if (_newEnumDic.ContainsKey(_enumItemStr) == true)
                                                continue;

                                            // 기존에 없던 Item인가요?
                                            if (_originEnumDic.ContainsKey(_enumItemStr) == false)
                                            {
                                                GetData.isEnumGenerate = true;

                                                _highestIndex += 1;
                                                _newEnumDic.Add(_enumItemStr, _highestIndex);
                                            }

                                            // 기존에 있던 Item
                                            else
                                            {
                                                // 기록된 Index 추가
                                                _newEnumDic.Add(_enumItemStr, _originEnumDic[_enumItemStr]);
                                            }
                                        }

                                        if (GetData.isEnumGenerate == false)
                                        {
                                            if (_newEnumDic.Count != _originEnumDic.Count) // 기존 Item이 제거된 경우를 대비한 코드
                                                GetData.isEnumGenerate = true;
                                        }

                                        if (GetData.isEnumGenerate == true)
                                            GetData.enumGenerateListDic[_enumStr] = _newEnumDic;
                                    }
                                    break;
                                #endregion
                                #region Nested
                                case ColumnType.Nested:
                                    {
                                        bool _isNestedDataGenerate = false;

                                        // 데이터 클래스에 중첩 클래스 변수 선언
                                        if (1 == _columnInfo.typeParamArr.Length)
                                        {
                                            _varTypeStr = _columnInfo.typeParamArr[0];
                                            _onImportFuncStr = GetScriptGen_NestedFunc;
                                            _onImportFuncFormatStr = _columnInfo.typeParamArr[0];

                                            string _nestedClassNameStr = _varTypeStr;
                                            _SetNestedClass_Func(_nestedClassNameStr);
                                        }

                                        // 데이터 클래스에 중첩 클래스 배열 변수 선언
                                        else if (_columnInfo.typeParamArr[0].IsCompare_Func(TableImporter_ExcelToJson.ListColumnTypeStr) == true)
                                        {
                                            string _nestedClassNameStr = _columnInfo.typeParamArr[1];
                                            _varTypeStr = StringBuilder_C.Append_Func(_nestedClassNameStr, "[]");
                                            _onImportFuncStr = GetScriptGen_NestedArrFunc;
                                            _onImportFuncFormatStr = _nestedClassNameStr;

                                            if (_varTypeStr.IsNullOrWhiteSpace_Func() == false)
                                                _varTypeStr = _varTypeStr.Replace(GetData.GetColumnIngoreStr, string.Empty);

                                            if (_onImportFuncFormatStr.IsNullOrWhiteSpace_Func() == false)
                                                _onImportFuncFormatStr = _onImportFuncFormatStr.Replace(GetData.GetColumnIngoreStr, string.Empty);

                                            _SetNestedClass_Func(_nestedClassNameStr);
                                        }
                                        else
                                        {
                                            _isNestedDataGenerate = true;
                                        }

                                        // 중첩 클래스에 멤버 변수 선언
                                        if (_isNestedDataGenerate == true && 2 <= _columnInfo.typeParamArr.Length)
                                        {
                                            Debug_C.Log_Func("변수 추가 안 함", Debug_C.PrintLogType.DB_Generate);

                                            _isWriteVar = false;

                                            if (GetData.nestedGenerateDic.IsHave_Func() == true)
                                            {
                                                string _nestedType = _columnInfo.typeParamArr[0];
                                                if (GetData.nestedGenerateDic.TryGetValue(_nestedType, out Dictionary<string, ColumnType> _dataDic) == true)
                                                {
                                                    string _varName = _columnInfo.name;
                                                    ColumnType _colType = _columnInfo.typeParamArr[1].ToEnum<ColumnType>();
                                                    if (_dataDic.ContainsKey(_varName) == false)
                                                        _dataDic.Add(_varName, _colType);
                                                }
                                            }
                                        }

                                        void _SetNestedClass_Func(string _nestedClassNameStr)
                                        {
                                            // 중첩 클래스 스크립트 생성
                                            if (_nestedClassNameStr.Contains(GetData.GetColumnIngoreStr) == false)
                                            {
                                                if (GetData.nestedGenerateDic == null)
                                                    GetData.nestedGenerateDic = new Dictionary<string, Dictionary<string, ColumnType>>();

                                                if (GetData.nestedGenerateDic.TryGetValue(_nestedClassNameStr, out Dictionary<string, ColumnType> _dataDic) == false)
                                                {
                                                    _dataDic = new Dictionary<string, ColumnType>();
                                                    GetData.nestedGenerateDic.Add(_nestedClassNameStr, _dataDic);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                #endregion

                                default:
                                    Debug_C.Error_Func("다음 칼럼 타입은 지원하지 않습니다. : " + _columnType);
                                    break;
                            }
                        }

                        if (_isWriteVar == true)
                        {
                            string _columnName = _columnInfo.name;
                            string _columnKorName = string.Empty;
                            if (_columnInfo.korName.IsNullOrWhiteSpace_Func() == false)
                                _columnKorName = string.Format(GetScriptGen_LabelText, _columnInfo.korName);

                            string _fieldStr = string.Format(GetScriptGen_Var, _varTypeStr, _columnName, _columnKorName);
                            _dataVarStr = StringBuilder_C.Append_Func(_dataVarStr, _fieldStr);

                            _onImportFuncStr = string.Format(_onImportFuncStr, _columnName, _colID, _onImportFuncFormatStr);
                            _dataFuncStr = StringBuilder_C.Append_Func(_dataFuncStr, _onImportFuncStr);
                        }
                    }

                    string _dataClassNameC = GetData.GetDataClassName_Func(_sheetNameStr, true);

                    string _dataScriptPath = StringBuilder_C.Append_Func(GetLibraryScriptPathS, _dataClassNameC, ".cs");
                    Debug_C.Log_Func("_className : " + _sheetNameStr + " / _dataGroupScriptPath : " + _dataScriptPath, Debug_C.PrintLogType.DB_Generate);

                    string _libraryDataClassStr = default;
                    string _libraryDataClassVarStr = default;
                    if (_libraryTableDataList.IsHave_Func() == true)
                    {
                        foreach (TableImporterData.LibraryTableData _libraryTableData in _libraryTableDataList)
                        {
                            if (_sheetNameStr.IsCompare_Func(_libraryTableData.sheetName) == true)
                            {
                                _libraryDataClassStr = _libraryTableData.dataClassStr;
                                _libraryDataClassVarStr = _libraryTableData.dataClassVarStr;

                                break;
                            }
                        }
                    }

                    string _dataScriptStr = string.Format(GetScriptGen_Data_C, _dataClassName, _dataVarStr, _dataFuncStr, _libraryDataClassStr, _libraryDataClassVarStr);
                    File.WriteAllText(_dataScriptPath, _dataScriptStr);

                    // 클라이언트 스크립트
                    string _clientDataScriptPath = StringBuilder_C.Append_Func(_clientScriptPath, _dataClassName, ".cs");
                    if (File.Exists(_clientDataScriptPath) == true)
                    {
                        Debug_C.Log_Func(_dataClassName + " 스크립트 확인, 경로 : " + _clientDataScriptPath, Debug_C.PrintLogType.DB_Generate);
                    }
                    else
                    {
                        Debug_C.Log_Func(_dataClassName + " 스크립트 생성, 경로 : " + _clientDataScriptPath, Debug_C.PrintLogType.DB_Generate);

                        string _clientDataScriptStr = string.Format(GetScriptGen_Client, _dataClassName);
                        File.WriteAllText(_clientDataScriptPath, _clientDataScriptStr);
                    }
                    #endregion

                    #region UID Key 드롭다운
                    LibraryRemocon.CallEdit_Generate_Script_Func(null, typeof(Cargold.Example.DB_Key드롭다운), _sheetNameStr + "Key", (_codeStr) =>
                    {
                        _codeStr = _codeStr.Replace(typeof(Cargold.Example.드롭다운DB매니저).Name, typeof(Cargold.FrameWork.DataBase_Manager).Name);
                        _codeStr = _codeStr.Replace("Example", _sheetNameStr);

                        string _lowerTypeStr = Editor_C.GetFirstCharLower_Func(_sheetNameStr);
                        _codeStr = _codeStr.Replace("example", _lowerTypeStr);

                        return _codeStr;
                    }, true, null, _clientScriptPath);
                    #endregion
                }
            }

            // 스크립터블 오브젝트
            GetData.sheetDataDic = _sheetDataDic;
            #endregion

            #region Enum 생성
            if (GetData.isEnumGenerate == true)
            {
                GetData.isEnumGenerate = false;

                Debug_C.Log_Func("Enum 생성 O", Debug_C.PrintLogType.DB_Generate);
                TableImporter_Generate.CallEdit_OnGenerateEnum_Func();
            }
            #endregion

            #region Nested 생성
            if (GetData.nestedGenerateDic.IsHave_Func() == true)
            {
                Dictionary<string, Dictionary<string, ColumnType>> _nestedGenerateDic = GetData.nestedGenerateDic;
                foreach (var _item1 in _nestedGenerateDic)
                {
                    string _nestedName = _item1.Key;
                    string _nestedScriptPath = StringBuilder_C.Append_Func(GetLibraryScriptPathS, _nestedName, "_C.cs");

                    string _memberVarStr = default;
                    string _onImportFuncTotalStr = default;
                    int _id = 0;
                    foreach (var _item2 in _item1.Value)
                    {
                        if (_item2.Key.IsNullOrWhiteSpace_Func() == true)
                            continue;

                        string _varTypeStr, _onImportFuncStr, _onImportFuncFormatStr;
                        string _varName = _item2.Key;
                        ColumnType _varType = _item2.Value;
                        if (IsComplexColumnType_Func(_varType, out _varTypeStr, out _onImportFuncStr, out _onImportFuncFormatStr) == true)
                        {
                            Debug_C.Error_Func("Nested에서 다음 칼럼 타입은 지원하지 않습니다. : " + _varType);
                        }

                        _memberVarStr = StringBuilder_C.Append_Func(_memberVarStr, "    public ", _varTypeStr, " ", _varName, ";\n");

                        _onImportFuncStr = string.Format(_onImportFuncStr, _varName, _id, _onImportFuncFormatStr);
                        _onImportFuncTotalStr = StringBuilder_C.Append_Func(_onImportFuncTotalStr, _onImportFuncStr);

                        _id++;
                    }

                    string _structScriptStr = string.Format(GetScriptGen_NestedClass, _nestedName, _memberVarStr, _onImportFuncTotalStr);
                    File.WriteAllText(_nestedScriptPath, _structScriptStr);
                }

                GetData.nestedGenerateDic.Clear();
            }
            #endregion

            #region DB 매니저 스크립트 생성
            // 라이브러리
            string _dbmClassNameC = GetData.GetDBMClassName_Func(true);
            string _dbmScriptPathC = StringBuilder_C.Append_Func(GetLibraryScriptPathS, _dbmClassNameC, ".cs");

            string _dbManagerVar = default;
            string _dbManagerInit = default;
            string _dbManagerCatch = default;

            foreach (var _sheetItem in _sheetDataDic)
            {
                string _sheetName = _sheetItem.Key;

                string _dataGroupClassName = GetData.GetDataGroupClassName_Func(_sheetName, false);
                string _privateVarName = Editor_C.GetFirstCharLower_Func(_sheetName);
                string _publicVarName = StringBuilder_C.Append_Func("Get", _sheetName);
                string _var = string.Format(GetScriptGen_DBM_Var, _dataGroupClassName, _privateVarName, _publicVarName, _sheetName);
                _dbManagerVar = StringBuilder_C.Append_Func(_dbManagerVar, _var);

                string _init = string.Format(GetScriptGen_DBM_Init, _privateVarName);
                _dbManagerInit = StringBuilder_C.Append_Func(_dbManagerInit, _init);

                string _catch = string.Format(GetScriptGen_DBM_Catch, _publicVarName);
                _dbManagerCatch = StringBuilder_C.Append_Func(_dbManagerCatch, _catch);
            }

            string _dbmClassName = GetData.GetDBMClassName_Func(false);
            string _dbmPath = GetData.GetDbmPath_Func(false);
            string _dbmLibrary = default;

            if (_libraryTableDataList.IsHave_Func() == true)
            {
                foreach (TableImporterData.LibraryTableData _libraryTableData in _libraryTableDataList)
                {
                    _dbmLibrary = StringBuilder_C.Append_Func(_dbmLibrary, _libraryTableData.dbmStr);
                }
            }

            string _dbmCScriptStr = string.Format(GetScriptGen_DataBaseManager, _dbmClassName, _dbManagerVar, _dbManagerInit, _dbManagerCatch, _dbmPath, _dbmLibrary);
            File.WriteAllText(_dbmScriptPathC, _dbmCScriptStr);

            // 클라이언트
            string _dbmScriptPath = StringBuilder_C.Append_Func(_clientScriptPath, _dbmClassName, ".cs");
            if (File.Exists(_dbmScriptPath) == true)
            {
                Debug_C.Log_Func(_dbmClassName + " 스크립트 확인, 경로 : " + _dbmScriptPath, Debug_C.PrintLogType.DB_Generate);
            }
            else
            {
                Debug_C.Log_Func(_dbmClassName + " 스크립트 생성, 경로 : " + _dbmScriptPath, Debug_C.PrintLogType.DB_Generate);

                string _dbmScriptStr = string.Format(GetScriptGen_Client, _dbmClassName);
                File.WriteAllText(_dbmScriptPath, _dbmScriptStr);
            }
            #endregion

            TimeSpan _passTime = DateTime.Now - TableImporterData.Instance.tableImportGenerateTime_Edit;
            Debug.Log("스크립트 생성... " + ((float)_passTime.TotalSeconds).ToString_Second_Func(TableImporterData.Instance.exPoint) + "초 소요");

            TableImporterData.Instance.tableImportGenerateTime_Edit = DateTime.Now;
            GetData.isCallAfterCompiled = true;
            LibraryRemocon.Instance.Subscribe_OnCompileDone_Func(OnCompiledDone_Func);
            Editor_C.OnCompile_Func();
            AssetDatabase.Refresh();
        }
        public static void CallEdit_OnGenerateEnum_Func()
        {
            Dictionary<string, Dictionary<string, int>> _enumGenerateListDic = GetData.enumGenerateListDic;
            foreach (var _item1 in _enumGenerateListDic)
            {
                string _enumStr = _item1.Key;
                string _enumScriptPath = StringBuilder_C.Append_Func(GetLibraryScriptPathS, _enumStr, ".cs");

                Dictionary<int, string> _debugDic = new Dictionary<int, string>();

                string _enumItemStr = default;
                foreach (var _item2 in _item1.Value)
                {
                    if (_item2.Key.IsNullOrWhiteSpace_Func() == true)
                        continue;

                    if (_debugDic.ContainsKey(_item2.Value) == false)
                    {
                        _debugDic.Add(_item2.Value, _item1.Key);
                    }
                    else
                    {
                        int _duplicationIndexValue = _item2.Value;
                        string _alreadyItemStr = _debugDic[_duplicationIndexValue];

                        Debug_C.Error_Func($"{_enumStr}에서 다음 Index가 중복 생성되는 문제가 있습니다. : {_duplicationIndexValue}" +
                            $"\n 기 입력 Item / 신규 입력 Item : {_alreadyItemStr} / {_item2.Key}");
                    }

                    _enumItemStr = StringBuilder_C.Append_Func(_enumItemStr, "    ", _item2.Key, " = ", _item2.Value, ",\n", false);
                }

                string _enumScriptStr = string.Format(GetScriptGen_Enum, _item1.Key, _enumItemStr);
                File.WriteAllText(_enumScriptPath, _enumScriptStr);

                if (Debug_C.IsLogType_Func(Debug_C.PrintLogType.DB_Generate) == true)
                    Debug_C.Log_Func("Enum 생성) " + _enumStr + "\n_enumScriptPath : " + _enumScriptPath + "\n _enumScriptStr : " + _enumScriptStr
                        , Debug_C.PrintLogType.DB_Generate);
            }
        }

        public static void OnGenerateConstKey_Func(string _dataKeyClassName, IEnumerable<string[]> _dataKeyList, string _path = null, string _scriptNameStr = null)
        {
            if(_path.IsNullOrWhiteSpace_Func() == true)
            {
                string _libraryScriptPath = GetData.GetLibraryScriptPath;
                string _projectPath = GetData.CallEdit_GetProjectPath_Func();
                _path = GetData.CallEdit_GetFullPath_Func(_libraryScriptPath, _projectPath);
                _path = StringBuilder_C.Append_Func(_path, Editor_C.SeparatorStr);
            }

            if (_scriptNameStr.IsNullOrWhiteSpace_Func() == true)
                _scriptNameStr = _dataKeyClassName;

            string _dataKeyScriptPath = StringBuilder_C.Append_Func(_path, _scriptNameStr, ".cs");

            string _totalDataKeyVarStr = default;
            foreach (string[] _dataKeyArr in _dataKeyList)
            {
                string _dataKeyVarStr = string.Format(GetScriptGen_DataKeyVar, _dataKeyArr[0], _dataKeyArr[1]);
                _totalDataKeyVarStr = StringBuilder_C.Append_Func(_totalDataKeyVarStr, _dataKeyVarStr);
            }

            string _dataKeyScriptStr = string.Format(GetScriptGen_DataKeyClass, _dataKeyClassName, _totalDataKeyVarStr);
            File.WriteAllText(_dataKeyScriptPath, _dataKeyScriptStr);

            Debug_C.Log_Func(_scriptNameStr + "이름의 Const String 스크립트를 생성했습니다. 클래스명 : " + _dataKeyClassName);
        }

        private static bool IsComplexColumnType_Func(ColumnType _columnType, out string _varTypeStr, out string _onImportFuncStr, out string _onImportFuncFormatStr)
        {
            bool _isComplexType = false;

            _varTypeStr = null;
            _onImportFuncStr = null;
            _onImportFuncFormatStr = null;

            switch (_columnType)
            {
                #region 단순 칼럼
                case ColumnType.Bool:
                    {
                        _varTypeStr = GetScriptGen_BoolVar;
                        _onImportFuncStr = GetScriptGen_BoolFunc;
                    }
                    break;

                case ColumnType.Int:
                    {
                        _varTypeStr = GetScriptGen_IntVar;
                        _onImportFuncStr = GetScriptGen_IntFunc;
                    }
                    break;

                case ColumnType.Float:
                    {
                        _varTypeStr = GetScriptGen_FloatVar;
                        _onImportFuncStr = GetScriptGen_FloatFunc;
                    }
                    break;

                case ColumnType.String:
                    {
                        _varTypeStr = GetScriptGen_StringVar;
                        _onImportFuncStr = GetScriptGen_StringFunc;
                    }
                    break;

                case ColumnType.Infinite:
                    {
                        _varTypeStr = GetScriptGen_InfiniteVar;
                        _onImportFuncStr = GetScriptGen_InfiniteFunc;
                    }
                    break;

                case ColumnType.DateTime:
                case ColumnType.DateTimeTick:
                    {
                        _varTypeStr = GetScriptGen_DateTimeTickVar;
                        _onImportFuncStr = GetScriptGen_DateTimeTickFunc;
                    }
                    break;
                #endregion
                #region 배열 칼럼
                case ColumnType.List_Bool:
                    {
                        _varTypeStr = GetScriptGen_BoolArrVar;
                        _onImportFuncStr = GetScriptGen_BoolArrFunc;
                        _onImportFuncFormatStr = "bool";
                    }
                    break;
                case ColumnType.List_Int:
                    {
                        _varTypeStr = GetScriptGen_IntArrVar;
                        _onImportFuncStr = GetScriptGen_IntArrFunc;
                        _onImportFuncFormatStr = "int";
                    }
                    break;
                case ColumnType.List_Float:
                    {
                        _varTypeStr = GetScriptGen_FloatArrVar;
                        _onImportFuncStr = GetScriptGen_FloatArrFunc;
                        _onImportFuncFormatStr = "float";
                    }
                    break;
                case ColumnType.List_String:
                    {
                        _varTypeStr = GetScriptGen_StringArrVar;
                        _onImportFuncStr = GetScriptGen_StringArrFunc;
                        _onImportFuncFormatStr = "string";
                    }
                    break;
                case ColumnType.List_Infinite:
                    {
                        _varTypeStr = GetScriptGen_InfiniteArrVar;
                        _onImportFuncStr = GetScriptGen_InfiniteArrFunc;
                        _onImportFuncFormatStr = "Cargold.Infinite.Infinite";
                    }
                    break;
                #endregion

                default:
                    _isComplexType = true;
                    break;
            }

            return _isComplexType;
        }

        private static void OnCompiledDone_Func()
        {
            LibraryRemocon.Instance.Unsubscribe_OnCompileDone_Func(OnCompiledDone_Func);

            if (GetData.sheetDataDic != null)
            {
                Debug_C.Log_Func("OnCompiledDone_Func) 데이터 그룹 생성");

                Dictionary<string, SheetData> _sheetDataDic = GetData.sheetDataDic;

                foreach (var _sheetDataItem in _sheetDataDic)
                {
                    string _sheetNameStr = _sheetDataItem.Key;
                    bool _isImport = GetData.IsSheetImport_Func(_sheetNameStr);
                    if (_isImport == true)
                    {
                        // 스크립터블 오브젝트 생성
                        string _dataGroupClassName = GetData.GetDataGroupClassName_Func(_sheetNameStr, false);
                        string _dbResourcePath = GetData.GetResourceFullPath;
                        Editor_C.GenerateResult _result =
                            Editor_C.TryGetLoadWithGenerateSobj_Func(_dataGroupClassName, out DataGroup_C _dataGroup, _dataGroupClassName, _dbResourcePath, _isLog: false);

                        if (Editor_C.GenerateResult.Success.HasFlag(_result) == true)
                        {
                            // 데이터 임포트
                            SheetData _sheetData = _sheetDataDic.GetValue_Func(_sheetNameStr);
                            _dataGroup.CallEdit_OnDataImport_Func(_sheetData);

                            Editor_C.SetSaveAsset_Func(_dataGroup);
                        }
                        else
                        {
                            GetData.sheetDataDic = _sheetDataDic;

                            LibraryRemocon.Instance.Subscribe_OnCompileDone_Func(OnCompiledDone_Func);
                        }
                    }
                }

                GetData.sheetDataDic = null;
            }

            if (GetData.isCallAfterCompiled == true)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    Debug_C.Log_Func("OnCompiledDone_Func) DB 매니저 생성");

                    string _path = GetData.GetResourceFullPath;

                    if (Editor_C.GenerateResult.Success.HasFlag(
                        Editor_C.TryGetLoadWithGenerateSobj_Func(
                            FrameWork.DataBase_Manager.Str, out FrameWork.DataBase_Manager _dbm, FrameWork.DataBase_Manager.Str, _path, _isLog: false)) == true)
                    {
                        // 데이터 임포트
                        string _folderName = GetData.GetResourcePath;
                        _dbm.CallEdit_OnDataImport_Func(_folderName, GetData.IsDataGroupImport);

                        GetData.isCallAfterCompiled = false;

                        TableImporterData.Instance.CallEdit_OnPassTimeLog_Func("스옵젝 생성... ");

                        TableImporter_C.OnGenerateAndImportDone_Func();

                        Editor_C.SetSaveAsset_Func(_dbm);
                    }
                    else
                    {
                        if (GetData.isCallAfterCompiled == false)
                        {
                            GetData.isCallAfterCompiled = true;

                            LibraryRemocon.Instance.Subscribe_OnCompileDone_Func(OnCompiledDone_Func);
                        }
                        else
                            Debug_C.Error_Func("?");
                    }
                };
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}