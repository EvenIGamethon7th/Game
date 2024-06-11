using Cargold;
using Cargold.DB.TableImporter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.DB.TableImporter
{
    public class TableImporter_ExcelDownload
    {
        public const string GetImportUrlFormat = "https://docs.google.com/spreadsheets/export?id={0}&exportFormat=xlsx&access_token={1}";

        private static TableImporterData GetData => TableImporter_C.GetData;

#if UNITY_EDITOR
        public static void ExcelDownload(int _remoconID)
        {
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(ExcelDownload_Cor(_remoconID), LibraryRemocon.Instance);
        }
        private static IEnumerator ExcelDownload_Cor(int _remoconID)
        {
            TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(_remoconID);

            string _importUrl = string.Format(TableImporter_ExcelDownload.GetImportUrlFormat, _remoconData.GetExcelID, _remoconData.GetAccessToken);

            using (UnityWebRequest _www = UnityWebRequest.Get(_importUrl))
            {
                DateTime _beginTime = DateTime.Now;

                _www.SendWebRequest();

                while (_www.isDone == false)
                {
                    float _progress = _www.downloadProgress;

                    UnityEditor.EditorUtility.DisplayProgressBar("구글 엑셀 다운로드 중...", "금방 끝날 거다 ~~", _progress);

                    yield return null;
                }

                UnityEditor.EditorUtility.ClearProgressBar();

                if (_www.error.IsNullOrWhiteSpace_Func() == true)
                {
                    string _excelPath = GetData.GetExcelPath_Func(_remoconID);
                    File.WriteAllBytes(_excelPath, _www.downloadHandler.data);

                    TimeSpan _passTime = DateTime.Now - _beginTime;
                    float _totalSeconds = (float)_passTime.TotalSeconds;
                    Debug.Log(_remoconID + ") 테이블 다운로드 끝... " + _totalSeconds.ToString_Second_Func(TableImporterData.Instance.exPoint) + "초 소요");

                    TableImporter_C.ExcelDownloadDone_Func(_remoconID);
                }
                else
                    Debug.LogError(_www.error);
            }
        }
#endif
    } 
}
