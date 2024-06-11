using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold.DB.TableImporter;
using Cargold.Remocon;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class FrameWorkData
        {
            public partial class DatabaseData
            {
                [FoldoutGroup(TableImporterData.KorStr), Indent(DatabaseData.IndentLv)]
                public partial class TableImporterData : Remocon.ITableImporter_C // 메인
                {
                    public const string KorStr = "테이블 임포터";

                    public const string GetStep1 = "1_구글 접근 권한";
                    public const string GetStep2 = "2_구글 엑셀 다운로드";
                    public const string GetStep3 = "3_엑셀에서 Json으로";
                    public const string GetStep4 = "4_스크립트 생성 및 데이터 임포트";
                    public const string OptionStep4 = OptionS + GetStep4;
                    public const string LibraryTableDataStr = "라이브러리 테이블 데이터";
                    public const string OptionStep4S_LibraryTableData = OptionStep4 + Editor_C.SeparatorStr + LibraryTableDataStr;
                    public const string OptionS = Editor_C.OptionS;
                    public const string Bug_DictionarySerialize = "딕셔너리 직렬화 방법을 정의해야 합니다.";
                    public const string OnTableImportingStr = "갱신 가주왁!!!";
                    public static TableImporterData Instance => DatabaseData.Instance.tableImporterData;

                    [BoxGroup(Editor_C.Mandatory), LabelText("Script 생성 경로"), FolderPath]
                    public string scriptPath = CargoldLibrary_C.GetInitError;
                    [BoxGroup(Editor_C.Mandatory), LabelText("DB 미리보기"), SerializeField, InlineEditor, ShowIf("@dbmClass != null")]
                    private Cargold.FrameWork.DataBase_Manager dbmClass;

                    [FoldoutGroup(OptionS + GetStep2), LabelText("xlsx 파일 이름")] public string excelName = "ImportedTable_C";
                    [FoldoutGroup(OptionS + GetStep2), LabelText("xlsx 저장 폴더 이름")] public string excelFolderName = "Temp";

                    public string GetExcelPath_Func(int _remoconID)
                    {
                        return StringBuilder_C.Append_Func(this.excelFolderName, Editor_C.SeparatorStr, this.excelName, _remoconID.ToString(), ".xlsx");
                    }

                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("칼럼 제외 문자")] private string columnIngoreStr = "@";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("시트 제외 문자")] private string sheetIngoreStr = "@";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("행열 제외 문자")] private string ignoreStr = "IGNORE_C";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("칼럼명(En)")] private string columnNameStr = "Column_Name";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("칼럼명(Kr)")] private string columnKorNameStr = "Column_KorName";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("칼럼 타입")] private string columnTypeStr = "Column_Type";

                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("Json 경로")] private string jsonPath = CargoldLibrary_C.GetInitError;
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("Json 폴더명")] private string jsonFolderName = "Json_C";
                    [FoldoutGroup(OptionS + GetStep3), SerializeField, LabelText("Json 파일명")] private string jsonFileName = "TableJson";

                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("스옵젝 경로")] public string dataSobjPath = CargoldLibrary_C.GetInitError;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("데이터 클래스 접두사")] public string dataClassPrefixStr = string.Empty;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("데이터 클래스 접미사")] public string dataClassSuffixStr = "Data";
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("데이터 그룹 클래스 접두사")] public string dataGroupClassPrefixStr = "DB_";
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("데이터 그룹 클래스 접미사")] public string dataGroupClassSuffixStr = "DataGroup";
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("Data Key 클래스 이름")] public string dataKeyClassName = "TableDataKey_C";
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("라이브러리 Script 생성 폴더명")] public string dbScriptFolderName = CargoldLibrary_C.GetLibraryFolderName;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("Sobj 폴더 이름")] public string dbResourceFolderName;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("DB Sobj 생성 대기"), ReadOnly] public Dictionary<string, DB.TableImporter.SheetData> sheetDataDic = null;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("DB Manager 생성 대기"), ReadOnly] public bool isCallAfterCompiled = false;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("Enum 스크립트 생성 여부"), ReadOnly] public bool isEnumGenerate = false;
                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("Enum 스크립트 생성 대기열")] public Dictionary<string, Dictionary<string, int>> enumGenerateListDic = null; // Key : Enum명, Value : Key - Enum Item, Value - Enum Index

                    [FoldoutGroup(OptionS + GetStep4), SerializeField, LabelText("Nested 스크립트 생성 대기열")] public Dictionary<string, Dictionary<string, ColumnType>> nestedGenerateDic = null; // Key : Class 이름, Value : Key - 변수명, Value - 변수 타입
                    [FoldoutGroup(OptionStep4S_LibraryTableData), SerializeField, LabelText("테이블 데이터와 연동된 카라리 시스템"), ReadOnly] public List<LibraryTableData> libraryTableDataList = new List<LibraryTableData>();

                    [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("딕셔너리 직렬화")] protected ILibrary iLibrary;
#if UNITY_EDITOR
                    [FoldoutGroup(Editor_C.Optional), LabelText("테이블 소요 시간 표기 자릿수")] public int exPoint = 3;
                    [HideInInspector] public System.DateTime tableImportTime_Edit;
                    [HideInInspector] public System.DateTime tableImportGenerateTime_Edit; 
#endif

                    #region Property
                    public string GetColumnIngoreStr => this.columnIngoreStr;
                    public string GetSheetIngoreStr => this.sheetIngoreStr;
                    public string GetIngoreStr => this.ignoreStr;
                    public string GetColumnNameStr => this.columnNameStr;
                    public string GetColumnKorNameStr => this.columnKorNameStr;
                    public string GetColumnTypeStr => this.columnTypeStr;
                    public string GetResourceFullPath => Editor_C.GetPath_Func(this.dataSobjPath, CargoldLibrary_C.GetResourcesStr, this.dbResourceFolderName);
                    public string GetResourcePath => this.dbResourceFolderName;
                    public string GetJsonFolderPath => Editor_C.GetPath_Func(this.jsonPath, this.jsonFolderName);
                    public string GetJsonFileName => this.jsonFileName;
                    public string GetDataKeyClassName => this.dataKeyClassName;
                    public string GetLibraryScriptPath => Editor_C.GetPath_Func(this.scriptPath, this.dbScriptFolderName);
                    public ILibrary GetLibrary
                    {
                        get
                        {
                            if (this.iLibrary == null)
                            {
                                this.iLibrary = new LibraryClass();
                            }

                            return this.iLibrary;
                        }
                        set
                        {
                            this.iLibrary = value;
                        }
                    }
                    #endregion

                    public void Init_Func()
                    {
#if UNITY_EDITOR
                        if (LibraryRemocon.Instance.isProejctSettingCargoldStyle == true)
                        {
                            string _resourcesApath = Editor_C.GetPath_Func(
                                CargoldLibrary_C.GetAseetsStr, CargoldLibrary_C.GetFolderName_3Resources, CargoldLibrary_C.GetLibraryFolderName, DatabaseData.Str);
                            this.jsonPath = _resourcesApath;
                            this.scriptPath = Editor_C.GetPath_Func(
                                CargoldLibrary_C.GetAseetsStr, CargoldLibrary_C.GetFolderName_2Scripts, CargoldLibrary_C.GetLibraryFolderName, DatabaseData.Str);
                            this.dataSobjPath = _resourcesApath;
                        }
                        else
                        {
                            string _resourcesApath = Editor_C.GetPath_Func(
                                CargoldLibrary_C.GetAseetsStr, CargoldLibrary_C.GetLibraryFolderName, DatabaseData.Str);
                            this.jsonPath = _resourcesApath;
                            this.scriptPath = Editor_C.GetPath_Func(
                                CargoldLibrary_C.GetAseetsStr, CargoldLibrary_C.GetLibraryFolderName, CargoldLibrary_C.GetScriptsStr, DatabaseData.Str);
                            this.dataSobjPath = _resourcesApath;
                        } 
#endif
                    }

                    [FoldoutGroup(OptionS + GetStep4), Button("Enum 수동 생성")]
                    private void OnGenerateEnum_Func()
                    {
#if UNITY_EDITOR
                        TableImporter_Generate.CallEdit_OnGenerateEnum_Func(); 
#endif
                    }

                    public void SetDbmManager_Func(FrameWork.DataBase_Manager _dbmClass)
                    {
                        this.dbmClass = _dbmClass;
                    }

                    public string GetDataClassName_Func(string _baseName, bool _isContainSignature)
                    {
                        return _isContainSignature == false
                            ? StringBuilder_C.Append_Func(this.dataClassPrefixStr, _baseName, this.dataClassSuffixStr)
                            : StringBuilder_C.Append_Func(this.dataClassPrefixStr, _baseName, this.dataClassSuffixStr, Cargold_Library.Signature);
                    }
                    public string GetDataGroupClassName_Func(string _baseName, bool _isContainSignature)
                    {
                        return _isContainSignature == false
                            ? StringBuilder_C.Append_Func(this.dataGroupClassPrefixStr, _baseName, this.dataGroupClassSuffixStr)
                            : StringBuilder_C.Append_Func(this.dataGroupClassPrefixStr, _baseName, this.dataGroupClassSuffixStr, Cargold_Library.Signature);
                    }
                    public string GetDBMClassName_Func(bool _isContainSignature)
                    {
                        return _isContainSignature == false
                            ? DatabaseData.Instance.dbManagerClassName
                            : StringBuilder_C.Append_Func(DatabaseData.Instance.dbManagerClassName, Cargold_Library.Signature);
                    }
                    public string GetDbmPath_Func(bool _isContainSignature)
                    {
                        string _dbmClassName = this.GetDBMClassName_Func(_isContainSignature);

                        return this.dbResourceFolderName.IsNullOrWhiteSpace_Func() == true
                            ? _dbmClassName
                            : StringBuilder_C.Append_Func(this.dbResourceFolderName, Editor_C.SeparatorStr, _dbmClassName);
                    }

    #if UNITY_EDITOR
                    public void CallEdit_OnPassTimeLog_Func(string _descStr)
                    {
                        System.TimeSpan _passTime = System.DateTime.Now - tableImportGenerateTime_Edit;
                        float _totalSec = (float)_passTime.TotalSeconds;
                        string _totalSecStr = _totalSec.ToString_Second_Func(Instance.exPoint);
                        Debug.Log(StringBuilder_C.Append_Func(_descStr, _totalSecStr, "초 소요"));
                    }

                    [FoldoutGroup(Editor_C.Optional), Button("xlsx 다운로드 폴더 열기")]
                    private void CallEdit_OpenFolder_Func()
                    {
                        string _path = CallEdit_GetProjectPath_Func();

                        _path += Editor_C.SeparatorStr + this.excelFolderName;
                        Debug_C.Log_Func("_path : " + _path);
                        System.Diagnostics.Process.Start(_path);
                    }
                    public string CallEdit_GetFullPath_Func(string _remainPath, string _projectPath = null)
                    {
                        if (_projectPath == null)
                            _projectPath = CallEdit_GetProjectPath_Func();

                        return Editor_C.GetPath_Func(_projectPath, _remainPath);
                    }
                    public string CallEdit_GetProjectPath_Func()
                    {
                        string _path = Application.dataPath;

                        string[] _arr = _path.Split(Editor_C.SeparatorChar);
                        _path = string.Empty;
                        for (int i = 0; i < _arr.Length - 1; i++)
                        {
                            if (0 < i)
                                _path += Editor_C.SeparatorStr;

                            _path += _arr[i];
                        }

                        return _path;
                    }
                    public void CallEdit_SetLibraryTableData_Func(LibraryTableData _libraryTableData)
                    {
                        if(this.libraryTableDataList == null)
                            this.libraryTableDataList = new List<LibraryTableData>();

                        if(this.libraryTableDataList.IsHave_Func() == true)
                        {
                            for (int i = libraryTableDataList.Count - 1; i >= 0; i--)
                            {
                                if (this.libraryTableDataList[i].sheetName.IsCompare_Func(_libraryTableData.sheetName) == true)
                                {
                                    Debug_C.Log_Func("다음 라이브러리의 테이블 데이터가 이미 등록되어 었어서 제거했습니다. : " + _libraryTableData.sheetName);

                                    this.libraryTableDataList.RemoveAt(i);
                                    break;
                                }
                            }
                        }

                        Debug_C.Log_Func("다음 라이브러리가 테이블 데이터로 등록되었습니다. : " + _libraryTableData.sheetName);
                        this.libraryTableDataList.Add(_libraryTableData);
                    }

                    [FoldoutGroup(OptionStep4S_LibraryTableData), Button("초기화")]
                    public void CallEdit_LibraryTableDataClear_Func()
                    {
                        if (this.libraryTableDataList != null)
                        {
                            Debug_C.Log_Func("라이브러리 테이블 데이터가 초기화되었습니다.");

                            this.libraryTableDataList.Clear();
                        }
                    }
#endif
                    void ITableImporter_C.OnGoogleAccess_Func()
                    {
                        Debug_C.Log_Func("OnGoogleAccess_Func");
                    }
                    void ITableImporter_C.OnTableImporting_Func()
                    {
                        Debug_C.Log_Func("OnTableImporting_Func");

                        if (this.remoconClassArr.IsHave_Func() == true)
                        {
                            int _firstRemoconID = this.remoconClassArr[0].GetRemoconID;
                            TableImporter_C.CheckGoogleAccessVaild_Func(_firstRemoconID);
                        }
                    }

                    [System.Serializable]
                    public class LibraryClass : ILibrary
                    {
                        [ShowInInspector, LabelText("재정의 여부")] public bool IsOverride => false;

                        public virtual string GetSerialize_Func(Dictionary<string, SheetData> _dic)
                        {
    #if UNITY_2020_1_OR_NEWER
                            return Newtonsoft.Json.JsonConvert.SerializeObject(_dic);
    #else
                            Debug_C.Error_Func(Bug_DictionarySerialize);
                            return default;
    #endif

                        }
                        public virtual Dictionary<string, SheetData> GetDerialize_Func(string _jsonStr)
                        {
    #if UNITY_2020_1_OR_NEWER
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SheetData>>(_jsonStr);
    #else
                            Debug_C.Error_Func(Bug_DictionarySerialize);
                            return default;
    #endif
                        }
                    }

                    public interface ILibrary : IOverride
                    {
                        string GetSerialize_Func(Dictionary<string, SheetData> _dic);
                        Dictionary<string, SheetData> GetDerialize_Func(string _jsonStr);
                    }

                    #region LibraryTableData
                    public abstract class LibraryTableData
                    {
                        public const string ColumnNameStr = "칼럼명";
                        public const string FormatingStrWithDefault = "/*{0}*/default";
                        public const string FormatingStr = "/*{0}*/";

                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("시트 이름")] public string sheetName = CargoldLibrary_C.GetInitError;
                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), ReadOnly, LabelText("DBM"), TextArea(3, 1000)] public string dbmStr = CargoldLibrary_C.GetInitError;
                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), ReadOnly, LabelText("Data Group 상속")] public string dataGroupStr = CargoldLibrary_C.GetInitError;
                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), ReadOnly, LabelText("Data Group 함수"), TextArea(3, 1000)] public string dataGroupFuncStr = CargoldLibrary_C.GetInitError;
                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), ReadOnly, LabelText("Data Class 상속")] public string dataClassStr = CargoldLibrary_C.GetInitError;
                        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), ReadOnly, LabelText("Data Class 변수"), TextArea(3, 1000)] public string dataClassVarStr = CargoldLibrary_C.GetInitError;

                        public abstract LibraryTableDataType GetLibraryTableDataType { get; } // 테이블 타입
                        protected abstract int GetDataVarNum { get; } // 데이터 클래스의 변수 재정의 때 Replace할 개수
                        protected virtual bool GetFormatStrContainDefault => true; // 데이터 클래스의 변수 재정의 때 쓸 포매팅 문자열

                        public virtual void Init_Func(string _sheetName, System.Type _exampleType)
                        {
#if UNITY_EDITOR
                            this.sheetName = _sheetName;
                            string _exampleScriptName = _exampleType.Name;

                            string _exampleScriptFolderPath = LibraryRemocon.Instance.GetExampleScriptFolderPath;
                            string _scriptContent = Editor_C.GetScriptContent_Func(_exampleScriptFolderPath, _exampleScriptName);

                            string[] _codeArr = _scriptContent.Split(new string[] { "/**/" }, System.StringSplitOptions.None);

                            for (int i = 1; i < _codeArr.Length;)
                            {
                                string _str = _codeArr[i];

                                switch (i)
                                {
                                    // 테이블 시트명 적용
                                    case 1:
                                        {
                                            string _varNameStr = Editor_C.GetFirstCharLower_Func(this.sheetName);
                                            _str = _str.Replace("재정의", _varNameStr);

                                            this.dbmStr = _str;
                                        }
                                        break;

                                    // 데이터 그룹 - 인터페이스 상속
                                    case 3:
                                        this.dataGroupStr = _str;
                                        break;

                                    // 데이터 그룹 - 재정의 선언
                                    case 5:
                                        this.dataGroupFuncStr = _str;
                                        break;

                                    // 데이터 - 인터페이스 상속
                                    case 7:
                                        this.dataClassStr = _str;
                                        break;

                                    // 데이터 - 재정의 선언
                                    case 9:
                                        {
                                            string _formatStr = this.GetFormatStrContainDefault == true ? FormatingStrWithDefault : FormatingStr;

                                            for (int _id = 0; _id < this.GetDataVarNum; _id++)
                                            {
                                                string _replaceStr = string.Format(_formatStr, _id);
                                                string _dataStr = this.GetDataVarStr_Func(_id);
                                                _str = _str.Replace(_replaceStr, _dataStr);
                                            }

                                            this.dataClassVarStr = _str;
                                        }
                                        break;
                                }

                                i += 2;
                            }
#endif
                        }

                        public abstract string GetDataVarStr_Func(int _id);
                        protected string GetDataVarStr_Func(string _contentStr)
                        {
                            return this.GetFormatStrContainDefault == true
                                ? StringBuilder_C.Append_Func("this.", _contentStr)
                                : _contentStr;
                        }

#if UNITY_EDITOR
                        [Button("테이블 임포터에 등록")]
                        public void CallEdit_SetTableData_Func()
                        {
                            TableImporterData.Instance.CallEdit_SetLibraryTableData_Func(this);
                        }
#endif

                        public enum LibraryTableDataType
                        {
                            None = 0,

                            Localize,
                            Dialogue,
                            Inapp,
                        }
                    } 
                    #endregion
                }
            }
        }
    }
}
