using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        [FoldoutGroup(UtilityClassData.KorStr), Indent(LibraryRemocon.IndentLv)]
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public StringDropdownData stringDropdownData = new StringDropdownData();

            [FoldoutGroup(StringDropdownData.KorStr), Indent(UtilityClassData.IndentLv)]
            public class StringDropdownData : ScriptGenerate // 메인
            {
                public const string KorStr = "스트링 드롭다운";
                public const string Str = "StringDropdownData";
                [FoldoutGroup(Editor_C.Optional), LabelText("스옵젝 생성 경로"), SerializeField, FolderPath] protected string sobjFolderPath = null;
                [HideInInspector, SerializeField] private List<InitData> initDataList = null;

                protected override bool GetIsEnableDefault => false;
                protected override string GetClassNameDefault => StringDropdownData.Str;
                protected override Type GetExampleType
                {
                    get
                    {
                        bool _isLibrary = this.initDataList.IsHave_Func();
                        return this.GetExampleType_Func(_isLibrary);
                    }
                }
                protected override bool IsActionAfterCompiled => true;
                protected override bool IsOverWrite => false;

                public StringDropdownData() : base()
                {
                    this.initDataList = new List<InitData>();
                }

                private Type GetExampleType_Func(string _typeNameStr = null)
                {
                    bool _isLibrary = _typeNameStr.IsNullOrWhiteSpace_Func() == false;
                    return this.GetExampleType_Func(_isLibrary);
                }
                private Type GetExampleType_Func(bool _isLibrary = false)
                {
                    return _isLibrary == false ? typeof(Cargold.Example.스트링드롭다운) : typeof(Cargold.Example.스트링드롭다운_카라리);
                }

#if UNITY_EDITOR
                public void CallEdit_Generate_Func(string _typeName, params SobjDropdown.DropdownData[] _initDataArr)
                {
                    InitData _initData = new InitData(_typeName, _initDataArr);

                    if (this.initDataList == null)
                        this.initDataList = new List<InitData>();

                    this.initDataList.Add(_initData);

                    base.CallEdit_Generate_Func(true, _typeName, _typeName);
                }

                protected override void CallEdit_Generate_Func
                    (string _exampleScriptFolderPath, Func<string, string> _codeModifyDel, string _className, string _scriptName, params string[] _subFolderStrArr)
                {
                    string _typeName = this.initDataList.IsHave_Func() == true ? this.initDataList.GetLastItem_Func().typeNameStr : null;
                    _scriptName = this.GetScriptName_Func(_typeName);

                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr: _subFolderStrArr);
                }

                protected override void CallEdit_GenerateDone_Func()
                {
                    if(this.initDataList.IsHave_Func() == true)
                    {
                        foreach (InitData _initData in this.initDataList)
                        {
                            string _typeNameStr = _initData.typeNameStr;
                            SobjDropdown.DropdownData[] _initDataArr = _initData.initDataArr;
                            _GenerateSobj_Func(_typeNameStr, _initDataArr);
                        }

                        this.initDataList.Clear();
                    }
                    else
                    {
                        _GenerateSobj_Func();
                    }

                    void _GenerateSobj_Func(string _typeNameStr = null, SobjDropdown.DropdownData[] _initDataArr = null)
                    {
                        string _scriptName = this.GetScriptName_Func(_typeNameStr);
                        string _sobjFolderPath = this.sobjFolderPath.IsNullOrWhiteSpace_Func() == true ? LibraryRemocon.Instance.GetResourcesFolderPath : this.sobjFolderPath;
                        string _sobjName = null;
                        bool _isLibrary = false;
                        if (_typeNameStr.IsNullOrWhiteSpace_Func() == true)
                        {
                            _sobjName = base.className;
                            _isLibrary = false;
                        }
                        else
                        {
                            _sobjName = _typeNameStr;
                            _isLibrary = true;
                        }

                        if(Editor_C.TryGetLoadWithGenerateSobj_Func(_sobjName, out Cargold.SobjDropdown _stringDropdown, _scriptName, _sobjFolderPath)
                            == Editor_C.GenerateResult.Success_Generate) 
                            _stringDropdown.Init_Func(_sobjName, base.scriptFolderPath, _isLibrary, _initDataArr);
                    }
                }

                public void CallEdit_GenerateConstString_Func(List<SobjDropdown.DropdownData> _dataList, string _dataTypeStr, string _scriptFolderPath,
                    bool _isLibrary = false)
                {
                    string _constStringStr = default;

                    foreach (SobjDropdown.DropdownData _data in _dataList)
                    {
                        string _constVar = string.Format("\n    public const int {0} = {1};", _data.nameStr, _data.uid);
                        _constStringStr = StringBuilder_C.Append_Func(_constStringStr, _constVar);
                    }

                    string _scriptName = GetScriptName_Func(_dataTypeStr);

                    if (_scriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                        _scriptFolderPath = base.scriptFolderPath;

                    Type _exampleType = this.GetExampleType_Func(_isLibrary);
                    LibraryRemocon.CallEdit_Generate_Script_Func(null, _exampleType, _dataTypeStr, (string _codeStr) =>
                    {
                        _codeStr = _codeStr.Replace("// Const", _constStringStr);

                        return _codeStr;
                    }, true, _scriptName: _scriptName, _scriptFolderPath: _scriptFolderPath);
                } 
#endif

                private string GetScriptName_Func(string _typeName = null)
                {
                    Type _sobjType = _typeName.IsNullOrWhiteSpace_Func() == true
                        ? typeof(Cargold.Example.스트링드롭다운Sobj)
                        : typeof(Cargold.Example.스트링드롭다운_카라리Sobj);

                    string _sobjTypeName = _sobjType.Name;
                    string _exampleTypeStr = this.GetExampleType_Func(_typeName).Name;

                    if (_typeName.IsNullOrWhiteSpace_Func() == true)
                        _typeName = base.className;

                    return _sobjTypeName.Replace(_exampleTypeStr, _typeName);
                }

                [System.Serializable]
                public class InitData
                {
                    public string typeNameStr;
                    public SobjDropdown.DropdownData[] initDataArr;

                    public InitData(string _typeName, params SobjDropdown.DropdownData[] _initDataArr)
                    {
                        this.typeNameStr = _typeName;

                        this.initDataArr = _initDataArr;
                    }
                }
            }
        }
    }
}