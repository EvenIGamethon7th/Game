#if UNITY_EDITOR
using OfficeOpenXml; 
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.DB.TableImporter
{
    [System.Serializable]
	public class ExcelPackage_C
	{
#if UNITY_EDITOR
        [SerializeField] private ExcelPackage excelPackage;

        public bool IsExist => this.excelPackage != null;
        private static TableImporterData GetData => TableImporter_C.GetData;

        public ExcelPackage_C(string _filePath)
        {
            FileInfo _fileInfo = new FileInfo(_filePath);
            if (_fileInfo.Exists == false)
            {
                Debug_C.Error_Func("엑셀 파일을 찾을 수 없습니다.");
                this.excelPackage = null;
            }
            else
                this.excelPackage = new ExcelPackage(_fileInfo);
        }

        public string[] GetSheetNameArr_Func()
        {
            List<string> _sheetNameList = new List<string>();

            foreach (ExcelWorksheet _excelWS in this.excelPackage.Workbook.Worksheets)
            {
                if (_excelWS.Name.Contains(GetData.GetSheetIngoreStr) == true)
                    continue;

                _sheetNameList.Add(_excelWS.Name);
            }

            return _sheetNameList.ToArray();
        }

        public bool TryGetExcelRange_Func(string _sheetName, out ExcelRange _excelRange)
        {
            ExcelWorksheet _worksheet = this.excelPackage.Workbook.Worksheets[_sheetName];
            if (_worksheet == null)
            {
                _excelRange = null;
                return false;
            }

            _excelRange = _worksheet.Cells;

            return true;
        } 
#endif
    }
}
