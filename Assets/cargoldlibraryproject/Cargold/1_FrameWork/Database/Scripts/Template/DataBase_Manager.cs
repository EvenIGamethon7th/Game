using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Cargold.Debug_C;
using DatabaseData = Cargold.LibraryRemocon.FrameWorkData.DatabaseData;

namespace Cargold.FrameWork
{
    public abstract partial class DataBase_Manager : SerializedScriptableObject, GameSystem_Manager.IInitializer, IDebug_C, Remocon.IDatabase_C
    {
        public const string Fomula = "수식";
        public const string SobjPath = "Cargold/DB/";
        public const string Str = "DataBase_Manager";

        public static string[] GetDataGroupTypeStrArr
        {
            get
            {
                return new string[]
                {
                    "Define",
                    "Localize",
                    "UI",
                    "Sound",
                    "Log",
                };
            }
        }

        private static DataBase_Manager instance;
        public static DataBase_Manager Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = Resources.Load<Cargold.FrameWork.DataBase_Manager>(DataBase_Manager.Str);
                }

                return instance;
            }
        }

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), InlineEditor, SerializeField, LabelText(Define_C.KrStr)] private Define_C define_C = null;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), InlineEditor, SerializeField, LabelText(UI_C.KrStr)] private UI_C ui_C = null;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), InlineEditor, SerializeField, LabelText(Sound_C.KrStr)] private Sound_C sound_C = null;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), InlineEditor, SerializeField, LabelText(Log_C.KrStr)] private Log_C log_C = null;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("데이터 그룹 스옵젝 경로")] protected string dataGroupSobjPath;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("캐싱 여부")] protected bool isCatched = false;

        public Define_C GetDefine_C => this.GetTemplate_Func<Define_C>(TemplateType.Define);
        public UI_C GetUi_C => this.GetTemplate_Func<UI_C>(TemplateType.UI);
        public Sound_C GetSound_C => this.GetTemplate_Func<Sound_C>(TemplateType.Sound);
        public Log_C GetLog_C => this.GetTemplate_Func<Log_C>(TemplateType.Log);

        protected abstract PrintLogType GetPrintLogType { get; }

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                instance = this;

                this.GetDefine_C.Init_Func();
                this.GetUi_C.Init_Func();
                this.GetSound_C.Init_Func();
                this.GetLog_C.Init_Func();

                this.Init_Project_Func();
            }
            else if (_layer == 1)
            {

            }
        }
        protected virtual void Init_Project_Func()
        {

        }

        public bool IsLogType_Func(PrintLogType _logType = PrintLogType.Common)
        {
            return GetPrintLogType.HasFlag(_logType);
        }

        private T GetTemplate_Func<T>(TemplateType _templateType) where T : DataGroupTemplate
        {
            T _templateClass = _GetTemplate_Func(_templateType);

            if (_templateClass == null)
            {
                string _nameStr = default;
                switch (_templateType)
                {
                    case TemplateType.Define:   _nameStr = Define_C.Str;    break;
                    case TemplateType.UI:       _nameStr = UI_C.Str;        break;
                    case TemplateType.Sound:    _nameStr = Sound_C.Str;     break;
                    case TemplateType.Log:      _nameStr = Log_C.Str;       break;

                    case TemplateType.None:
                    default:
                        Debug_C.Error_Func("_templateType : " + _templateType);
                        break;
                }

                _templateClass = Resources.Load<T>(_nameStr);

                if (_templateClass != null)
                {
                    Debug.Log("DB Template 로드 : " + _templateClass);
                }
                else
                {
#if UNITY_EDITOR
                    string _dbPath = DatabaseData.Instance.tableImporterData.GetResourceFullPath;
                    string _typeName = StringBuilder_C.Append_Func("Cargold.FrameWork.", _nameStr);
                    if(Editor_C.GenerateResult.Success.HasFlag(Editor_C.TryGetLoadWithGenerateSobj_Func<T>(_nameStr, out _templateClass, _typeName, _dbPath)) == false)
                    {
                        Debug_C.Error_Func("?");
                    }
#endif
                }

                switch (_templateType)
                {
                    case TemplateType.Define:   this.define_C = _templateClass as Define_C;     break;
                    case TemplateType.UI:       this.ui_C = _templateClass as UI_C;             break;
                    case TemplateType.Sound:    this.sound_C = _templateClass as Sound_C;       break;
                    case TemplateType.Log:      this.log_C = _templateClass as Log_C;           break;

                    case TemplateType.None:
                    default:
                        Debug_C.Error_Func("_templateType : " + _templateType);
                        return null;
                }
            }

            return _templateClass;

            T _GetTemplate_Func(TemplateType _type)
            {
                switch (_type)
                {
                    case TemplateType.Define:   return this.define_C as T;
                    case TemplateType.UI:       return this.ui_C as T;
                    case TemplateType.Sound:    return this.sound_C as T;
                    case TemplateType.Log:      return this.log_C as T;

                    case TemplateType.None:
                    default:
                        Debug_C.Error_Func("_templateType : " + _templateType);
                        return null;
                }
            }
        }

#if UNITY_EDITOR
        [Button("캐싱 ㄱㄱ ~")]
        private void CallEdit_Catch_Func()
        {
            this.CallEdit_OnDataImport_Func(string.Empty);
        }

        public void CallEdit_OnDataImport_Func(string _dataGroupSobjPath, bool _isDataImport = true)
        {

            if(_dataGroupSobjPath.IsNullOrWhiteSpace_Func() == false)
                this.dataGroupSobjPath = _dataGroupSobjPath + Editor_C.SeparatorStr;

            if (this.isCatched == false)
            {
                this.GetTemplate_Func<Define_C>(TemplateType.Define);
                this.GetTemplate_Func<UI_C>(TemplateType.UI);
                this.GetTemplate_Func<Sound_C>(TemplateType.Sound);
                this.GetTemplate_Func<Log_C>(TemplateType.Log);
            }

            this.CallEdit_OnDataImport_Func(_isDataImport);

            Editor_C.SetSaveAsset_Func(this);
        }

        public virtual void CallEdit_OnDataImport_Func(bool _isDataImport = true)
        {
            if (this.isCatched == false)
            {
                this.isCatched = true;

                this.GetDefine_C.CallEdit_OnDataImport_Func();
                this.GetUi_C.CallEdit_OnDataImport_Func();
                this.GetSound_C.CallEdit_OnDataImport_Func();
                this.GetLog_C.CallEdit_OnDataImport_Func();
            }
            
            if(_isDataImport == true)
                this.CallEdit_OnDataImportDone_Func();
        }

        /// <summary>
        /// 데이터 임포트 완료 후 호출
        /// </summary>
        public virtual void CallEdit_OnDataImportDone_Func() { }
#endif

        public static void DataError_Func(string _key, System.Exception _e)
        {
            Debug_C.Error_Func("DB Load 오류) base.Key : " + _key);
            Debug_C.Warning_Func("_e Data : " + _e.Data);
            Debug_C.Warning_Func("_e Message : " + _e.Message);
            Debug_C.Warning_Func("_e Source : " + _e.Source);
            Debug_C.Warning_Func("_e String : " + _e.ToString());
        }

        public enum TemplateType
        {
            None,

            Define,
            UI,
            Sound,
            Log,
        }
    }
}