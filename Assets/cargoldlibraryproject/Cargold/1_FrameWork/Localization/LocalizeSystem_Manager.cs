using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Cargold.FrameWork;

/*
'22.07.09
Tmp에 컴포넌트를 다는 쪽으로 개선 ㄱ

1. 언어 변경 대응을 해야 하는 걸 별도로 신경 쓸 필요 없음
2. 에디터 모드 때 언어 변경 결과물을 확인할 수 있음 (이걸 타 직군도 해볼 수 있음) + (테스트 매니저를 통해서 언어 변경 테스트 해볼 수 있도록 하면 좋을 듯)

 */

namespace Cargold.FrameWork
{
    public abstract class LocalizeSystem_Manager : MonoBehaviour, GameSystem_Manager.IInitializer
    {
        private const string FormatMHMS = "{0}{1} {2:00}{3} {4:00}{5} {6:00}{7}";
        private const string DayStr = "D";
        private const string HourStr = "H";
        private const string MinuteStr = "M";
        private const string SecondStr = "S";

        public static LocalizeSystem_Manager Instance;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ReadOnly, ShowInInspector]
        private SystemLanguage GetLanguageType =>
            Cargold.FrameWork.UserSystem_Manager.Instance is null == false
            ? Cargold.FrameWork.UserSystem_Manager.Instance.GetCommon.GetLanguageType_Func()
            : SystemLanguage.Unknown;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                
            }
        }

        public string GetLcz_Func(string _lczID, SystemLanguage _languageType = SystemLanguage.Unknown)
        {
            if (_languageType == SystemLanguage.Unknown)
                _languageType = this.GetLanguageType;

            if (Cargold.FrameWork.DataBase_Manager.Instance.GetLocalize_C.TryGetData_Func(_lczID, out ILczData _lczData) == true)
                return _lczData.GetLczStr_Func(_languageType);
            else
            {
                Debug_C.Error_Func("다음 로컬키는 존재하지 않습니다. : " + _lczID);
                return default;
            }
        }
        public virtual string GetLcz_Func(DateTime _dateTime, bool _isContainDay = false, string _dayStr = null, string _hourStr = null, string _minStr = null, string _secStr = null)
        {
            return GetLczStatic_Func(_dateTime, _isContainDay, _dayStr, _hourStr, _secStr);
        }

        public string GetLcz_Format_Func(string _lczID, object _obj, SystemLanguage _languageType = SystemLanguage.Unknown)
        {
            string _lczStr = this.GetLcz_Func(_lczID, _languageType);
            return string.Format(_lczStr, _obj);
        }
        public string GetLcz_Format_Func(string _lczID, object _obj1, object _obj2, SystemLanguage _languageType = SystemLanguage.Unknown)
        {
            string _lczStr = this.GetLcz_Func(_lczID, _languageType);
            return string.Format(_lczStr, _obj1, _obj2);
        }
        public string GetLcz_Format_Func(string _lczID, SystemLanguage _languageType = SystemLanguage.Unknown, params object[] _objArr)
        {
            string _lczStr = this.GetLcz_Func(_lczID, _languageType);
            return string.Format(_lczStr, _objArr);
        }

        public string GetLczSecond_Func(float _sec, int _pointNumber = 0, SystemLanguage _languageType = SystemLanguage.Unknown)
        {
            if (_languageType == SystemLanguage.Unknown)
                _languageType = this.GetLanguageType;

            return StringBuilder_C.Append_Func(_sec.ToString_Func(_pointNumber), this.GetLczSecond_Func(_languageType));
        }
        protected virtual string GetLczSecond_Func(SystemLanguage _languageType)
        {
            switch (_languageType)
            {
                case SystemLanguage.Korean:
                    return "초";

                default:
                    return "s";
            }
        }

        public static string GetLczStatic_Func(DateTime _dateTime, bool _isContainDay = false, string _dayStr = null, string _hourStr = null, string _minStr = null, string _secStr = null)
        {
            string _dayValue = string.Empty;
            _hourStr = string.IsNullOrEmpty(_hourStr) == true ? HourStr : _hourStr;
            _minStr = string.IsNullOrEmpty(_minStr) == true ? MinuteStr : _minStr;
            _secStr = string.IsNullOrEmpty(_secStr) == true ? SecondStr : _secStr;

            int _hour = default;

            if (_isContainDay == false)
            {
                _dayValue = string.Empty;
                _dayStr = string.Empty;

                _hour = (_dateTime.Day - 1) * 24;
            }
            else
            {
                int _day = _dateTime.Day - 1;

                _dayValue = _day.ToString();
                _dayStr = string.IsNullOrEmpty(_dayStr) == true ? DayStr : _dayStr;
            }

            _hour += _dateTime.Hour;
            return string.Format(FormatMHMS, _dayValue, _dayStr, _hour, _hourStr, _dateTime.Minute, _minStr, _dateTime.Second, _secStr);
        }
        public static string GetGoogleLangCodeStr_Func(SystemLanguage _langType)
        {
            switch (_langType)
            {
                case SystemLanguage.Afrikaans: return "af";
                case SystemLanguage.Arabic: return "ar";
                case SystemLanguage.Basque: return "eu";
                case SystemLanguage.Belarusian: return "be";
                case SystemLanguage.Bulgarian: return "bg";
                case SystemLanguage.Catalan: return "ca";
                case SystemLanguage.Czech: return "cs";
                case SystemLanguage.Danish: return "da";
                case SystemLanguage.Dutch: return "nl";
                case SystemLanguage.Estonian: return "et";
                case SystemLanguage.Finnish: return "fi";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Greek: return "el";
                case SystemLanguage.Hebrew: return "he";
                case SystemLanguage.Hungarian: return "hu";
                case SystemLanguage.Icelandic: return "is";
                case SystemLanguage.Indonesian: return "id";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.Japanese: return "ja";
                case SystemLanguage.Korean: return "ko";
                case SystemLanguage.Latvian: return "lv";
                case SystemLanguage.Lithuanian: return "It";
                case SystemLanguage.Norwegian: return "no";
                case SystemLanguage.Polish: return "pl";
                case SystemLanguage.Romanian: return "ro";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.Slovak: return "sk";
                case SystemLanguage.Slovenian: return "sl";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.Swedish: return "sv";
                case SystemLanguage.Thai: return "th";
                case SystemLanguage.Turkish: return "tr";
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Vietnamese: return "vi";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.ChineseTraditional: return "tw"; // 원래는 zh-TW인데 변수명에 타이푼이 들어가면 안 돼서...

                case SystemLanguage.SerboCroatian: return "sr"; // 확실치 않음

                case SystemLanguage.Chinese: // 확실치 않음
                case SystemLanguage.ChineseSimplified: return "cn"; // 원래는 zh-CN. tw와 동일한 이유

                default:
                case SystemLanguage.Faroese:
                case SystemLanguage.Unknown:
                case SystemLanguage.English: return "en";
            }
        }

        /// <summary>
        /// 지원하는 언어 (라이브러리 리모콘의 Lcz 테이블 데이터 + 한국어 + 영어)
        /// </summary>
        /// <param name="_langTypeArr"></param>
        /// <returns>테이블 라이브러리와 연동된 경우 True</returns>
        public static bool TryGetLangType_Func(out SystemLanguage[] _langTypeArr)
        {
            LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData _tableImporterData = LibraryRemocon.Instance.frameWorkData.databaseData.tableImporterData;
            if (_tableImporterData.libraryTableDataList.IsHave_Func() == true)
            {
                foreach (LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData.LibraryTableData _libraryTableData in _tableImporterData.libraryTableDataList)
                {
                    if (_libraryTableData is LibraryRemocon.FrameWorkData.LocalizeSystemData.TableData)
                    {
                        LibraryRemocon.FrameWorkData.LocalizeSystemData.TableData _tableData = _libraryTableData as LibraryRemocon.FrameWorkData.LocalizeSystemData.TableData;

                        _langTypeArr = _tableData.GetSystemLanguageArr;

                        return true;
                    }
                }
            }

            _langTypeArr = null;
            return false;
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        } 
#endif
    }
}

public static class LczLibrary
{
    public static void SetLcz_Func(this TMPro.TextMeshProUGUI _tmp, string _lczKey)
    {
        _tmp.text = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
    }
    public static void SetLcz_Format_Func(this TMPro.TextMeshProUGUI _tmp, string _lczKey, object _param)
    {
        string _lczStr = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
        _tmp.text = string.Format(_lczStr, _param);
    }
    public static void SetLcz_Format_Func(this TMPro.TextMeshProUGUI _tmp, string _lczKey, object _param1, object _param2)
    {
        string _lczStr = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
        _tmp.text = string.Format(_lczStr, _param1, _param2);
    }
    public static void SetLcz_Format_Func(this TMPro.TextMeshProUGUI _tmp, string _lczKey, object _param1, object _param2, object _param3)
    {
        string _lczStr = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
        _tmp.text = string.Format(_lczStr, _param1, _param2, _param3);
    }
}