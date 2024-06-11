using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class FrameWorkData
        {
            [FoldoutGroup(LocalizeSystemData.KorStr), Indent(FrameWorkData.IndentLv)]
            public partial class LocalizeSystemData : ScriptGenerate // 메인
            {
                public const string KorStr = "로컬라이징";
                public const int IndentLv = FrameWorkData.IndentLv + 1;

                [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField] private TableData libraryTableData = new TableData();

                protected Type GetTableExampleType => typeof(Cargold.Example.DB_로컬라이즈);
                protected override bool GetIsEnableDefault => true;
                protected override string GetClassNameDefault => "LocalizeSystem_Manager";
                protected override System.Type GetExampleType => typeof(Cargold.Example.로컬라이즈매니저);

                public override void Init_Func()
                {
                    base.Init_Func();

                    this.libraryTableData.Init_Func("Localize", GetTableExampleType);
                }

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Func<string, string> _codeModifyDel, string _className, string _scriptName, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _subFolderStrArr: "FrameWork");
                }
#endif

                [LabelText(KorStr)]
                public class TableData : DatabaseData.TableImporterData.LibraryTableData
                {
                    private const string formatStr = @"case SystemLanguage.{0}:            return {1};";

                    [SerializeField, OnValueChanged("CallEdit_SetLangTypeArr_Func"), LabelText("테이블 연동"), DictionaryDrawerSettings(KeyLabel = "언어", ValueLabel = "칼럼명")]
                    private Dictionary<SystemLanguage, string> langCodeDic = new Dictionary<SystemLanguage, string>();
                    [SerializeField] private SystemLanguage defaultLangType = SystemLanguage.Korean;
                    [SerializeField] private string defaultLangStr = "ko";
                    [SerializeField, ReadOnly] private SystemLanguage[] systemLanguageArr;

                    protected override int GetDataVarNum => this.langCodeDic.Count + 1;
                    protected override bool GetFormatStrContainDefault => false;
                    public override LibraryTableDataType GetLibraryTableDataType => LibraryTableDataType.Localize;

                    public SystemLanguage[] GetSystemLanguageArr => this.systemLanguageArr;

                    public override void Init_Func(string _sheetName, Type _exampleType)
                    {
                        if (this.langCodeDic == null)
                            this.langCodeDic = new Dictionary<SystemLanguage, string>();

                        this.SetLangTypeArr_Func();
                        
                        base.Init_Func(_sheetName, _exampleType);
                    }
                    private void SetLangTypeArr_Func()
                    {
                        if (this.defaultLangType != SystemLanguage.Korean)
                            _Set_Func(SystemLanguage.Korean);

                        if (this.defaultLangType != SystemLanguage.English)
                            _Set_Func(SystemLanguage.English);

                        this.systemLanguageArr = this.langCodeDic.GetKeys_Func();

                        void _Set_Func(SystemLanguage _langType)
                        {
                            if (this.langCodeDic.ContainsKey(_langType) == false)
                            {
                                string _googleLagnCodeStr = FrameWork.LocalizeSystem_Manager.GetGoogleLangCodeStr_Func(_langType);
                                this.langCodeDic.Add(_langType, _googleLagnCodeStr);
                            }
                        }
                    }

                    public override string GetDataVarStr_Func(int _id)
                    {
                        SystemLanguage _langType = default;
                        string _columnStr = default;

                        if (_id == 0)
                        {
                            _langType = this.defaultLangType;
                            _columnStr = this.defaultLangStr;
                        }
                        else
                        {
                            Debug_C.Log_Func("_id : " + _id + " / Length : " + this.systemLanguageArr.Length);

                            _langType = this.systemLanguageArr[_id - 1];
                            _columnStr = this.langCodeDic.GetValue_Func(_langType);
                        }

                        string _formatStr = string.Format(formatStr, _langType, _columnStr);
                        return base.GetDataVarStr_Func(_formatStr);
                    }

#if UNITY_EDITOR
                    [Button("구글 번역 코드 추가하기")]
                    private void CallEdit_AddLangType_Func(SystemLanguage _langType)
                    {
                        string _str = Cargold.FrameWork.LocalizeSystem_Manager.GetGoogleLangCodeStr_Func(_langType);
                        this.langCodeDic.Add_Func(_langType, _str);

                        this.SetLangTypeArr_Func();
                    } 

                    private void CallEdit_SetLangTypeArr_Func()
                    {
                        this.SetLangTypeArr_Func();
                    }
#endif
                }
            }
        }
    }
}