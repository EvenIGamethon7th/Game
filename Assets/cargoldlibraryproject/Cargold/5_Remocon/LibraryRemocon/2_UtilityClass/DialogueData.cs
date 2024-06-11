using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public DialogueData dialogueData = new DialogueData();

            [FoldoutGroup(DialogueData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class DialogueData : ScriptGenerate // 메인
            {
                public const string KorStr = "다이얼로그";
                public const string Str = "Dialogue";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static DialogueData Instance => UtilityClassData.Instance.dialogueData;

                [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField] private TableData libraryTableData = new TableData();
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("UI")] private string uiClassNameStr = "UI_Dialogue_Script";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("UI 버튼")] private string uiBtnClassNameStr = "UI_Dialogue_Button";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("화자")] private string speakerClassNameStr = "Dialogue_Speaker";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("버튼 데이터")] private string btnDataClassNameStr = "DialogueButtonData";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("버튼 콜백 상수 문자열 스크립트 이름")] private string btnCallBackKeyName = "DialogueCallBackKey";
                [ShowInInspector] private List<string[]> callbackKeyStrList;

                public string GetUiClassNameStr => this.uiClassNameStr;
                public string GetUiBtnClassNameStr => this.uiBtnClassNameStr;
                protected override bool GetIsEnableDefault => false;
                protected override string GetClassNameDefault => "DialogueSystem_Manager";
                protected override Type GetExampleType => typeof(Cargold.Example.다이얼로그시스템);
                protected override bool IsActionAfterCompiled => true;

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { DialogueData.Str };

                    this.libraryTableData.Init_Func(DialogueData.Str, typeof(Cargold.Example.DB_다이얼로그));
                }

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, Func<string, string> _codeModifyDel, string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr);

                    base.CallEdit_Generate_Func(typeof(Cargold.Example.다이얼로그UI), this.uiClassNameStr);
                    base.CallEdit_Generate_Func(typeof(Cargold.Example.다이얼로그UI버튼), this.uiBtnClassNameStr);
                    base.CallEdit_Generate_Func(typeof(Cargold.Example.다이얼로그화자), this.speakerClassNameStr);
                    base.CallEdit_Generate_Func(typeof(Cargold.Example.다이얼로그버튼데이터), this.btnDataClassNameStr);

                    // 프리팹 복제
                    base.CallEdit_Duplicate_Func<GameObject>(Editor_C.AssetType.Prefab, "UI_Dialogue");
                    base.CallEdit_Duplicate_Func<GameObject>(Editor_C.AssetType.Prefab, "UI_Dialogue_Button");
                }
                protected override void CallEdit_GenerateDone_Func()
                {
                    base.CallEdit_GenerateDone_Func();

                    string _btnPath = Editor_C.GetPath_Func(LibraryRemocon.Instance.GetPrefabFolderPath, "UI_Dialogue_Button");
                    GameObject _uiDialBtnObj = Editor_C.GetLoadAssetAtPath_Func<GameObject>(_btnPath, _isAddExtentionStr: true);
                    if (_uiDialBtnObj.TryGetComponent(out Dialogue.PropertyAdapter_UI_Dialogue_Button _propertyDialBtnClass) == true)
                    {
                        _propertyDialBtnClass.CallEdit_AddComponent_Func();
                    }
                    else
                    {
                        Debug_C.Error_Func("다이얼로그 프리팹이 생성되지 않았습니다.");
                    }

                    string _dialPath = Editor_C.GetPath_Func(LibraryRemocon.Instance.GetPrefabFolderPath, "UI_Dialogue");
                    GameObject _uiDialObj = Editor_C.GetLoadAssetAtPath_Func<GameObject>(_dialPath, _isAddExtentionStr: true);
                    if (_uiDialObj.TryGetComponent(out Dialogue.PropertyAdapter_UI_Dialogue_Script _propertyDialClass) == true)
                    {
                        //_propertyDialClass.CallEdit_AddComponent_Func();

                        _propertyDialClass.CallEdit_TryAddComponent_Func<Cargold.Dialogue.UI_Dialogue_Script>(out _);
                    }
                    else
                    {
                        Debug_C.Error_Func("다이얼로그 프리팹이 생성되지 않았습니다.");
                    }

                    //const AnimationClip _clip = default;
                    //const string _generatePathStr = default;
                    //Editor_C.OnGenerateAnimationClip_Func(_clip, _generatePathStr);
                }

                public void CallEdit_AddCallbackKey_Func(string _key)
                {
                    if (this.callbackKeyStrList == null)
                        this.callbackKeyStrList = new List<string[]>();

                    string[] _keyArr = new string[] { _key, _key };
                    this.callbackKeyStrList.Add(_keyArr);
                }
                public void CallEdit_CallbackKeyGenerate_Func(bool _isForce = true)
                {
                    if(this.callbackKeyStrList.IsHave_Func() == true)
                    {
                        DB.TableImporter.TableImporter_Generate.OnGenerateConstKey_Func("DialogueCallBackKey", this.callbackKeyStrList);

                        this.callbackKeyStrList.Clear();

                        Debug_C.Log_Func("다이얼로그 버튼 콜백 Key를 생성했습니다.");
                    }
                    else
                    {
                        if(_isForce == true)
                            Debug_C.Error_Func("다이얼로그 버튼 콜백Key가 없습니다.");
                    }
                }
#endif

                [LabelText(KorStr)]
                public class TableData : LibraryTableData
                {
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("그룹 Key")] public string groupKey = "groupKey";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("이름 Key")] public string nameLczKey = "nameLczKey";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("대사 Key")] public string descLczKey = "descLczKey";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("화자 Type")] public string speakerType = "speakerType";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("딤 비활성화 여부")] public string isDimOffStr = "isDimOff";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("화자 애니 Type")] public string speakerAniTrigger = "speakerAniTrigger";
                    [BoxGroup(LibraryTableData.ColumnNameStr), LabelText("버튼 데이터 배열")] public string dialogueButtonDataArr = "dialogueButtonDataArr";

                    protected override int GetDataVarNum => 7;
                    public override LibraryTableDataType GetLibraryTableDataType => LibraryTableDataType.Dialogue;

                    public override string GetDataVarStr_Func(int _id)
                    {
                        switch (_id)
                        {
                            case 0:
                                return base.GetDataVarStr_Func(this.nameLczKey);

                            case 1:
                                return base.GetDataVarStr_Func(this.descLczKey);

                            case 2:
                                return base.GetDataVarStr_Func(this.speakerType);

                            case 3:
                                return base.GetDataVarStr_Func(this.speakerAniTrigger);

                            case 4:
                                return base.GetDataVarStr_Func(this.dialogueButtonDataArr);

                            case 5:
                                return base.GetDataVarStr_Func(this.groupKey);

                            case 6:
                                return base.GetDataVarStr_Func(this.isDimOffStr);

                            default:
                                Debug_C.Error_Func("_id : " + _id);
                                return default;
                        }
                    }
                }
            }
        }
    }
}