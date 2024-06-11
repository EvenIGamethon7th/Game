using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.DB.TableImporter;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData.TableImporterRemocon;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class FrameWorkData
        {
            public partial class DatabaseData
            {
                public partial class TableImporterData
                {
                    [SerializeField, LabelText("테이블 리모콘")] private TableImporterRemocon[] remoconClassArr = null;
                    [SerializeField, LabelText("테이블 임포트 여부")] private bool isDataGroupImport = true;

                    public TableImporterRemocon[] GetRemoconClassArr => this.remoconClassArr;
                    public bool IsDataGroupImport => this.isDataGroupImport;

                    public TableImporterRemocon GetRemoconData_Func(int _id)
                    {
                        return this.remoconClassArr[_id];
                    }

                    public bool IsSheetImport_Func(string _sheetNameStr)
                    {
                        if (this.TryGetSheetRemocon_Func(_sheetNameStr, out var _sheetRemocon) == true)
                        {
                            return _sheetRemocon.isImport;
                        }

                        return false;
                    } 
                    public bool TryGetSheetRemocon_Func(string _sheetNameStr, out SheetRemocon _sheetRemocon)
                    {
                        foreach (TableImporterRemocon _remoconClass in this.remoconClassArr)
                        {
                            if(_remoconClass.TryGetSheetRemocon_Func(_sheetNameStr, out _sheetRemocon) == true)
                            {
                                return _sheetRemocon.isImport;
                            }
                        }

                        _sheetRemocon = null;

                        Debug_C.Error_Func("다음 시트 리모콘은 찾을 수 없습니다. : " + _sheetNameStr);

                        return false;
                    }

#if UNITY_EDITOR
                    [Button("일괄 세팅")]
                    public void CallEdit_SetAll_Func(bool _isOn)
                    {
                        this.isDataGroupImport = _isOn;

                        foreach (TableImporterRemocon _remoconClass in this.remoconClassArr)
                            _remoconClass.CallEdit_SetAll_Func(_isOn);
                    }
#endif

                    [System.Serializable]
                    public class TableImporterRemocon
                    {
                        private static List<string> RemoveSheetList = new List<string>();

                        [SerializeField] private int remoconID;
                        [SerializeField] private string tableNameStr;
                        [SerializeField, LabelText("구글 엑셀 Url"), OnValueChanged("CallEdit_ExcelUrlChanged_Func")] public string googleExcelUrl;
                        [FoldoutGroup(Editor_C.Optional)]
                        [SerializeField, FoldoutGroup(OptionS + GetStep1), LabelText("포트"), PropertyRange(1000, 9999)] private int port = 8000;
                        [SerializeField, FoldoutGroup(OptionS + GetStep1), ReadOnly, LabelText("구글 시트 ID")] private string excelID = null;
                        [SerializeField, FoldoutGroup(OptionS + GetStep1), ReadOnly, LabelText("구글 API 접근 토큰")] private string accessToken;

                        [SerializeField, LabelText("시트 리모콘")] private Dictionary<string, SheetRemocon> sheetRemoconDic = null;

                        public int GetPort => this.port;
                        public string GetExcelID => this.excelID;
                        public int GetRemoconID => this.remoconID;
                        public string GetTableNameStr => this.tableNameStr;
                        public string GetAccessToken => this.accessToken;

                        public Dictionary<string, SheetRemocon> GetSheetRemoconDic => this.sheetRemoconDic;

                        public TableImporterRemocon()
                        {
                            this.remoconID = LibraryRemocon.instance.frameWorkData.databaseData.tableImporterData.GetRemoconClassArr.Length;
                            this.sheetRemoconDic = new Dictionary<string, SheetRemocon>();
                        }

                        public void SetAccessToken_Func(string _accessToken)
                        {
                            this.accessToken = _accessToken;
                        }

                        [BoxGroup(Editor_C.Mandatory), Button(GetStep1)]
                        public void OnGoogleAccessVaild_Func()
                        {
                            Debug_C.Init_Func(LibraryRemocon.Instance);

                            TableImporter_C.CheckGoogleAccessVaild_Func(this.remoconID);
                        }

                        [BoxGroup(Editor_C.Mandatory), Button(OnTableImportingStr)]
                        public void OnExcelDownload_Func()
                        {
                            Debug_C.Init_Func(LibraryRemocon.Instance);

                            TableImporter_C.ExcelDownload(true, this.remoconID);
                        }

                        public bool TryGetSheetRemocon_Func(string _sheetNameStr, out SheetRemocon _sheetRemocon)
                        {
                            return this.sheetRemoconDic.TryGetValue(_sheetNameStr, out _sheetRemocon);
                        }

                        public void SetSheetRemocon_Func(Dictionary<string, SheetData> _sheetDataDic)
                        {
                            foreach (var item in _sheetDataDic)
                            {
                                string _key = item.Key;
                                if (this.sheetRemoconDic.ContainsKey(_key) == false)
                                    this.sheetRemoconDic.Add(_key, new SheetRemocon());
                            }
                        }

#if UNITY_EDITOR
                        private void CallEdit_ExcelUrlChanged_Func()
                        {
                            this.excelID = default;

                            string[] _urlArr = this.googleExcelUrl.Split(Editor_C.SeparatorChar);

                            for (int i = 0; i < _urlArr.Length; i++)
                            {
                                if (_urlArr[i].IsCompare_Func("d") == true)
                                {
                                    this.excelID = _urlArr[i + 1];
                                    Debug.Log("구글 엑셀 ID 세팅 완료");
                                    return;
                                }
                            }

                            if (this.excelID.IsNullOrWhiteSpace_Func() == true)
                                Debug.LogError("구글 엑셀 ID 못 찾음");
                        }

                        [Button("일괄 세팅")]
                        public void CallEdit_SetAll_Func(bool _isOn)
                        {
                            foreach (var item in this.sheetRemoconDic)
                                item.Value.CallEdit_SetAll_Func(_isOn);
                        }
#endif

                        [System.Serializable]
                        public class SheetRemocon
                        {
                            [LabelText("임포트 여부")] public bool isImport;

                            public SheetRemocon()
                            {
                                this.isImport = true;
                            }

#if UNITY_EDITOR
                            [Button("일괄 세팅")]
                            public void CallEdit_SetAll_Func(bool _isOn)
                            {
                                this.isImport = _isOn;
                            }
#endif
                        }
                    }
                }
            }
        }
    }
}