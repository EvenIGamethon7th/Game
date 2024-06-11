#if UNITY_EDITOR
using OfficeOpenXml; 
#endif
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Cargold;
using System;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.DB.TableImporter
{
	public class TableImporter_ExcelToJson
	{
        public const char ColumnSeparateChar = '_';
        public const string ColumnSeparateStr = "_";
        public const string AssetColumnTypeStr = "asset";
        public const string ListColumnTypeStr = "list";
        public const string NestedColumnTypeStr = "nested";

        private static TableImporterData GetData => TableImporter_C.GetData;

        public static string GetJsonFullPath_Func(int _remoconID)
        {
            TableImporterData _data = GetData;
            string _pathStr = Editor_C.GetPath_Func(_data.GetJsonFolderPath, _data.GetJsonFileName);
            return StringBuilder_C.Append_Func(_pathStr, _remoconID.ToString(), ".txt");
        }

#if UNITY_EDITOR
        public static void ToJson_Func(int _remoconID)
        {
            DateTime _beginTime = DateTime.Now;

            #region 엑셀 데이터를 string으로 변환한다.
            string _excelPath = GetData.GetExcelPath_Func(_remoconID);
            ExcelPackage_C _excelPackage = new ExcelPackage_C(_excelPath);
            if (_excelPackage.IsExist == false)
            {
                Debug_C.Error_Func("엑셀 파일이 존재하지 않습니다. 경로 : " + _excelPath);
                return;
            }

            Dictionary<string, Dictionary<int, List<string>>> _excelDataDic = new Dictionary<string, Dictionary<int, List<string>>>();
            string[] _sheetNameArr = _excelPackage.GetSheetNameArr_Func();
            foreach (string _sheetName in _sheetNameArr)
            {
                Debug_C.Log_Func("_sheetName : " + _sheetName, Debug_C.PrintLogType.DB_ExcelToString);

                if (_excelPackage.TryGetExcelRange_Func(_sheetName, out ExcelRange _sheetData) == false)
                {
                    Debug_C.Error_Func("다음 시트에 오류가 있습니다. : " + _sheetName);
                    continue;
                }

                int _realRowID = 0;
                Dictionary<int, List<string>> _excelStrDataByRowIdDic = null;
                object[,] _cellDataArrArr = _sheetData.Value as object[,];
                int _rowLength = _cellDataArrArr.GetLength(0);
                Debug_C.Log_Func("_rowLength : " + _rowLength, Debug_C.PrintLogType.DB_ExcelToString);

                List<int> _colIndexList = new List<int>();
                int _colLength = _cellDataArrArr.GetLength(1);
                for (int _colID = 0; _colID < _colLength; _colID++)
                    _colIndexList.Add(_colID);

                #region 제외할 열 검사...
                for (int _rowID = 0; _rowID < _rowLength; _rowID++)
                {
                    // 공백 칼럼 찾기 (두번째 이상의 행)
                    if (0 < _rowID)
                    {
                        ExcelRange _cellData = _sheetData[_rowID + 1, 1];
                        bool _isEmptyData = true;
                        string _dataStr = string.Empty;
                        if (_cellData != null && string.IsNullOrEmpty(_cellData.Text) == false)
                        {
                            _isEmptyData = false;
                            _dataStr = _cellData.Text;
                        }

                        // 제외 행인가?
                        if (_isEmptyData == false && _dataStr.Contains(GetData.GetIngoreStr) == true)
                        {
                            Debug_C.Log_Func("제외 행) " + _dataStr, Debug_C.PrintLogType.DB_ExcelToString_IgnoreCol);
                            continue;
                        }

                        if (_dataStr.IsCompare_Func(GetData.GetColumnNameStr) == true
                            || _dataStr.IsCompare_Func(GetData.GetColumnTypeStr) == true)
                        {
                            for (int i = _colIndexList.Count - 1; i >= 1; i--)
                            {
                                int _colID = _colIndexList[i];

                                _cellData = _sheetData[_rowID + 1, _colID + 1];
                                if (_cellData != null && string.IsNullOrEmpty(_cellData.Text) == true)
                                {
                                    _colIndexList.RemoveAt(i);

                                    Debug_C.Log_Func("공백 칼럼) " + _colID, Debug_C.PrintLogType.DB_ExcelToString_IgnoreCol);
                                }
                            }

                            Debug_C.Log_Func("제외 열 찾기 종료) 마지막 검사 행 : " + _rowID, Debug_C.PrintLogType.DB_ExcelToString_IgnoreCol);

                            break;
                        }
                    }

                    // 제외 열 찾기 (첫번째 행)
                    else
                    {
                        for (int _colID = 0; _colID < _colLength; _colID++)
                        {
                            ExcelRange _cellData = _sheetData[_rowID + 1, _colID + 1];
                            if (_cellData != null && string.IsNullOrEmpty(_cellData.Text) == false)
                            {
                                string _dataStr = _cellData.Text;

                                if (_dataStr.Contains(GetData.GetIngoreStr) == true)
                                {
                                    _colIndexList.Remove(_colID);
                                    Debug_C.Log_Func("제외 열) " + _colID, Debug_C.PrintLogType.DB_ExcelToString_IgnoreCol);
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 두번째 행부터...
                for (int _rowID = 1; _rowID < _rowLength; _rowID++)
                {
                    //Debug_C.Log_Func("_rowID : " + _rowID + " / _colLength : " + _colLength, Debug_C.PrintLogType.DB_ExcelToString);

                    List<string> _cellDataStrList = null;

                    bool _isRowEnd = false;

                    // 검사 가능한 열 순회
                    for (int i = 0; i < _colIndexList.Count; i++)
                    {
                        int _colID = _colIndexList[i];

                        ExcelRange _cellData = _sheetData[_rowID + 1, _colID + 1];
                        bool _isEmptyData = true;
                        string _dataStr = string.Empty;
                        if (_cellData != null && string.IsNullOrEmpty(_cellData.Text) == false)
                        {
                            _isEmptyData = false;
                            _dataStr = _cellData.Text;
                        }

                        //Debug_C.Log_Func("값) " + _rowID + "_" + _colID + " : " + _dataStr, Debug_C.PrintLogType.DB_ExcelToString);

                        // 첫번째 열인가?
                        if (_colID == 0)
                        {
                            // 공백인가?
                            if (_isEmptyData == true)
                            {
                                _isRowEnd = true;

                                //Debug_C.Log_Func("제외 행) 공백 키", Debug_C.PrintLogType.DB_ExcelToString);
                                break;
                            }

                            // 제외 행인가?
                            if (_dataStr.Contains(GetData.GetIngoreStr) == true)
                            {
                                //Debug_C.Log_Func("제외 행) 값 : " + _dataStr, Debug_C.PrintLogType.DB_ExcelToString);
                                break;
                            }
                        }

                        if (_cellDataStrList == null)
                            _cellDataStrList = new List<string>();

                        _cellDataStrList.Add(_dataStr);
                        //Debug_C.Log_Func("ADD) _colID : " + _colID + " / _dataStr : " + _dataStr, Debug_C.PrintLogType.DB_ExcelToString);
                    }

                    if (_cellDataStrList != null)
                    {
                        if (_excelStrDataByRowIdDic == null)
                            _excelStrDataByRowIdDic = new Dictionary<int, List<string>>();

                        _excelStrDataByRowIdDic.Add(_realRowID, _cellDataStrList);
                        _realRowID++;
                    }

                    if (_isRowEnd == true)
                    {
                        Debug_C.Log_Func("행 순회를 중단합니다.", Debug_C.PrintLogType.DB_ExcelToString);

                        break;
                    }
                } 
                #endregion

                if (_excelStrDataByRowIdDic != null)
                    _excelDataDic.Add_Func(_sheetName, _excelStrDataByRowIdDic);
            }
            #endregion

            #region 변환한 string 데이터를 기준으로 시트 데이터(칼럼의 이름, 타입 그리고 셀 데이터)를 정의한다.
            Enum_C.EnumContainer<ColumnType>.Init_Func();
            Dictionary<string, SheetData> _sheetDataDic = new Dictionary<string, SheetData>();
            foreach (var _sheetItem in _excelDataDic)
            {
                string _sheetName = _sheetItem.Key;

                // Key : 행 ID
                // Value : 레코드의 셀 데이터들
                Dictionary<int, List<string>> _rawSheetDataDic = _sheetItem.Value;
                Debug_C.Log_Func("_sheetName : " + _sheetName + " / 행 개수 : " + _rawSheetDataDic.Keys.Count, Debug_C.PrintLogType.DB_StringToJson);

                int _colNum = -1;
                foreach (var _rawItem in _rawSheetDataDic)
                {
                    List<string> _rawDataList = _rawItem.Value;
                    if (_rawDataList[0].IsCompare_Func(GetData.GetColumnNameStr) == true)
                    {
                        _colNum = _rawDataList.Count;
                        break;
                    }
                }

                if (_colNum == -1)
                {
                    Debug_C.Error_Func($"다음 시트에서 '{GetData.GetColumnNameStr}'을 찾지 못했습니다. : " + _sheetName);
                    return;
                }

                Debug_C.Log_Func("SheetData) _colNum : " + _colNum, Debug_C.PrintLogType.DB_StringToJson);
                SheetData _sheetData = new SheetData(_sheetName, _colNum);

                int _realRowID = 0;

                foreach (var _dicItem in _rawSheetDataDic)
                {
                    int _rowID = _dicItem.Key;

                    // 레코드의 셀 데이터들
                    List<string> _keyList = _dicItem.Value;

                    string _keyData = _keyList[0];
                    if (_keyList.Count <= 0 || _keyData.IsNullOrWhiteSpace_Func() == true)
                        continue;

                    Debug_C.Log_Func("_rowID : " + _rowID + " / Key List Num : " + _keyList.Count + " / _keyData : " + _keyData
                        , Debug_C.PrintLogType.DB_StringToJson);

                    #region 영문 칼럼명
                    if (_keyData.IsCompare_Func(GetData.GetColumnNameStr) == true)
                    {
                        Debug_C.Log_Func("Find Field Name) _rowID : " + _dicItem.Key, Debug_C.PrintLogType.DB_StringToJson);

                        for (int i = 1; i < _keyList.Count; i++)
                        {
                            ColumnInfo _fieldInfo = _sheetData.columnInfoArr[i];
                            string _cellData = _keyList[i];
                            _fieldInfo.name = _cellData;
                        }
                    } 
                    #endregion
                    #region 칼럼 타입
                    else if (_keyData.IsCompare_Func(GetData.GetColumnTypeStr) == true)
                    {
                        Debug_C.Log_Func("Find Field Type) _rowID : " + _dicItem.Key, Debug_C.PrintLogType.DB_StringToJson);

                        for (int i = 1; i < _keyList.Count; i++)
                        {
                            ColumnInfo _fieldInfo = _sheetData.columnInfoArr[i];

                            string[] _typeStrArr = _keyList[i].Split(ColumnSeparateChar);
                            string _typeStr = _typeStrArr[0];

                            Debug_C.Log_Func("_keyList[i] : " + _keyList[i] + " / 0 : " + _typeStrArr[0], Debug_C.PrintLogType.DB_StringToJson);

                            // 단순 칼럼이 아닌 경우
                            if (1 < _typeStrArr.Length)
                            {
                                // 배열 칼럼인 경우
                                if (_typeStr.IsCompare_Func(ListColumnTypeStr) == true)
                                {
                                    _typeStr = _keyList[i];
                                }

                                // 복합 칼럼인 경우
                                else
                                {
                                    string[] _complexTypeStrArr = null;

                                    // 엑셀 칼럼명에 _가 3개 이상인 경우
                                    switch (_typeStr)
                                    {
                                        #region Asset
                                        case AssetColumnTypeStr:
                                            {
                                                if (3 <= _typeStrArr.Length)
                                                {
                                                    // 칼럼명을 복구하기 위해 다시 _ 붙여주기

                                                    _complexTypeStrArr = new string[1];
                                                    int j = 1;
                                                    while (true)
                                                    {
                                                        _complexTypeStrArr[0] = StringBuilder_C.Append_Func(_complexTypeStrArr[0], _typeStrArr[j]);

                                                        j++;

                                                        if (j < _typeStrArr.Length)
                                                        {
                                                            _complexTypeStrArr[0] = StringBuilder_C.Append_Func(_complexTypeStrArr[0], ColumnSeparateStr);
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region Struct
                                        case NestedColumnTypeStr:
                                            {
                                                _complexTypeStrArr = new string[_typeStrArr.Length - 1];

                                                for (int j = 0; j < _complexTypeStrArr.Length; j++)
                                                    _complexTypeStrArr[j] = _typeStrArr[j + 1];
                                            }
                                            break;
                                        #endregion
                                    }

                                    if (_complexTypeStrArr.IsHave_Func() == false)
                                    {
                                        _complexTypeStrArr = new string[1];
                                        _complexTypeStrArr[0] = _typeStrArr[1];
                                    }

                                    _fieldInfo.typeParamArr = _complexTypeStrArr;
                                }
                            }

                            ColumnType _columnType = default;
                            try
                            {
                                _columnType = _typeStr.ToEnum<ColumnType>();
                            }
                            catch
                            {
                                _columnType = ColumnType.Undefined;
                            }

                            _fieldInfo.type = _columnType;
                        }
                    } 
                    #endregion
                    #region 한글 칼럼명
                    else if (_keyData.IsCompare_Func(GetData.GetColumnKorNameStr) == true)
                    {
                        Debug_C.Log_Func("Find Field Kor Name) _rowID : " + _dicItem.Key, Debug_C.PrintLogType.DB_StringToJson);

                        for (int i = 1; i < _keyList.Count; i++)
                        {
                            ColumnInfo _fieldInfo = _sheetData.columnInfoArr[i];
                            string _cellData = _keyList[i];
                            _fieldInfo.korName = _cellData;
                        }
                    } 
                    #endregion
                    else
                    {
                        Debug_C.Log_Func("Find Cell Data) _realRowID : " + _realRowID + " / _rowID : " + _dicItem.Key, Debug_C.PrintLogType.DB_StringToJson);

                        string[] _cellDataArr = _sheetData.GetCellDataArr_Func(_realRowID);

                        for (int i = 0; i < _keyList.Count; i++)
                        {
                            string _cellData = _keyList[i];

                            _cellDataArr[i] = _cellData;
                        }

                        _realRowID++;
                    }
                }

                _sheetDataDic.Add_Func(_sheetName, _sheetData);
            }
            #endregion

            // 임포트 시트 선별
            TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(_remoconID);
            _remoconData.SetSheetRemocon_Func(_sheetDataDic);

            #region 시트 데이터를 Json으로 변환한 뒤 텍스트 파일로 로컬 저장
            Editor_C.TryCheckOrGenerateFolder_Func(GetData.GetJsonFolderPath);

            string _jsonStr = GetData.GetLibrary.GetSerialize_Func(_sheetDataDic);

            File.WriteAllText(GetJsonFullPath_Func(_remoconID), _jsonStr);
            Debug_C.Log_Func(_jsonStr, Debug_C.PrintLogType.DB_Json);
            AssetDatabase.Refresh();
#endregion

            TimeSpan _passTime = DateTime.Now - _beginTime;
            Debug.Log(_remoconID + ") 엑셀에서 Json으로... " + ((float)_passTime.TotalSeconds).ToString_Second_Func(TableImporterData.Instance.exPoint) + "초 소요");

            TableImporter_C.OnXlsxToJsonDone_Func(_remoconID);
        }
#endif
    }
}