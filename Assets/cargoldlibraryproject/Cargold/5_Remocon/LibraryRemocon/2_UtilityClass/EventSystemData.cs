using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    using Cargold.EventSystem;

    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public EventSystemData eventSystemData = new EventSystemData();

            [FoldoutGroup(EventSystemData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class EventSystemData : ScriptGenerate // 메인
            {
                public const string KorStr = "이벤트 시스템";
                public const string Str = "EventSystem";
                public const int IndentLv = UtilityClassData.IndentLv + 1;
                public const string TemplateNameStr = "EventTemplate";
                private const string EditorStr = "편집 툴";

                public static EventSystemData Insance => LibraryRemocon.Instance.utilityClassData.eventSystemData;

#if UNITY_EDITOR
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "커스텀 직렬화"), HideLabel] public SerializeData serializeData = new SerializeData(); 
#endif
                [FoldoutGroup(CargoldLibrary_C.OptionalS + EventToolControl.Str), HideLabel] public EventToolControl eventToolControl = new EventToolControl();

                [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("이벤트 스크립트 이름")]
                private string eventClassNameStr = typeof(Cargold.EventSystem.Event_Script).Name;
                [FoldoutGroup(CargoldLibrary_C.Optional), LabelText("템플릿 프리팹 경로"), FilePath, SerializeField] private string templatePathStr =
                    Editor_C.GetUnityStartPathStr + Editor_C.SeparatorChar + Cargold.EventSystem.EventTool.ResourcePathStr;

                public string GetTemplatePathStr => this.templatePathStr;
                public string GetEventClassNameStr => this.eventClassNameStr;
                protected override bool GetIsEnableDefault => false;
                protected override string GetClassNameDefault => "EventSystem_Manager";
                protected override Type GetExampleType => typeof(Cargold.Example.이벤트시스템매니저);
                protected override bool IsActionAfterCompiled => true;

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { EventSystemData.Str };
                }

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, Func<string, string> _codeModifyDel
                    , string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr);

                    UtilityClassData.StringDropdownData _stringDropdownData = LibraryRemocon.Instance.utilityClassData.stringDropdownData;

                    string _beginTypeName = typeof(BeginType).Name;
                    _stringDropdownData.CallEdit_Generate_Func(_beginTypeName);

                    string _conTypeName = typeof(ConditionType).Name;
                    _stringDropdownData.CallEdit_Generate_Func(_conTypeName);

                    string _actiomTypeName = typeof(ActionType).Name;
                    SobjDropdown.DropdownData[] _actionTypeDataArr = new SobjDropdown.DropdownData[]
                    {
                        new SobjDropdown.DropdownData(nameof(ActionType.다이얼로그), ActionType.다이얼로그),
                        //new StringDropdown.DropdownData(nameof(ActionType.포커싱), ActionType.포커싱),
                    };
                    _stringDropdownData.CallEdit_Generate_Func(_actiomTypeName, _actionTypeDataArr);

                    base.CallEdit_Generate_Func(typeof(Cargold.Example.이벤트스크립트), this.eventClassNameStr);

                    base.CallEdit_Generate_Func(typeof(Cargold.Example.이벤트툴), EventToolControl.NameStr, (_codeStr) =>
                    {
                        _codeStr = _codeStr.Replace("Cargold.Example.카골드.", "");
                        return _codeStr.Replace(typeof(Cargold.Example.이벤트스크립트).Name, this.eventClassNameStr);
                    });

                    base.CallEdit_Generate_Func(
                        typeof(Cargold.Example.카골드.LibraryRemocon.UtilityClassData.이벤트DB)
                        , typeof(LibraryRemocon.UtilityClassData.EventSystemData).Name
                        , (_codeStr) =>
                        {
                            _codeStr = _codeStr.Replace("Cargold.Example.", "");
                            _codeStr = _codeStr.Replace("카골드", "Cargold");

                            _codeStr = _codeStr.Replace("public static LibraryRemocon Instance;", "");
                            _codeStr = _codeStr.Replace("public UtilityClassData utilityClassData;", "");
                            _codeStr = _codeStr.Replace("public EventSystemData eventSystemData;", "");

                            _codeStr = _codeStr.Replace(typeof(Cargold.Example.이벤트키드롭다운컨테이너).Name, "EventKeyDropdownContainer");
                            _codeStr = _codeStr.Replace(typeof(Cargold.Example.드롭다운아이템).Name, "EventDropdownItem");
                            _codeStr = _codeStr.Replace(typeof(Cargold.Example.드롭다운키).Name, "EventDropdownKey");

                            _codeStr = _codeStr.Replace(typeof(Cargold.Example.이벤트스크립트).Name, this.eventClassNameStr);

                            return _codeStr;
                        });
                }

                protected override void CallEdit_GenerateDone_Func()
                {
                    base.CallEdit_GenerateDone_Func();

                    GameObject _emptyObj = new GameObject(this.eventClassNameStr);
                    Component _component = _emptyObj.AddComponent(Type.GetType(this.eventClassNameStr));
                    Editor_C.TryCheckOrGenerateFolder_Func(Cargold.EventSystem.EventTool<Cargold.EventSystem.Event_Script>.EventApathStr);
                    GameObject _prefab = Prefab_C.CreatePrefabAsset_Func(_emptyObj, this.templatePathStr, TemplateNameStr, out bool _isSuccess);
                    GameObject.DestroyImmediate(_emptyObj, true);



                    string _resourceName =  EventToolControl.NameStr;
                    string _typeName = EventToolControl.NameStr;
                    string _path = this.eventToolControl.GetToolPathStr;
                    Editor_C.TryGetLoadWithGenerateSobj_Func(_resourceName, out Cargold.EventSystem.EventTool _loadObj, _typeName, _path);
                }

                #region SerializeData
                [System.Serializable]
                public class SerializeData
                {
                    [SerializeField] private bool isExcel = false;
                    [SerializeField] private string beginChar = "(B)";

                    public int beginComponentNum = 5;
                    public int beginVarNum = 5;

                    [SerializeField] private string conditionChar = "(C)";
                    public int conditionComponentNum = 5;
                    public int conditionVarNum = 5;

                    [SerializeField] private string stepChar = "(S)";
                    public int stepNum = 5;
                    public int actionComponentNum = 5;
                    public int actionVarNum = 5;

                    [SerializeField] private string[] parseStrArr = new string[] { "!", "@", "#", "$", "%" };

                    public string GetBeginChar => this.isExcel == true ? "\t" : this.beginChar;
                    public string GetConditionChar => this.isExcel == true ? "\t" : this.conditionChar;
                    public string GetStepChar => this.isExcel == true ? "\t" : this.stepChar;

                    public string GetParseStr_Func(int _id)
                    {
                        if (this.isExcel == true)
                            return "\t";

                        if (this.parseStrArr.TryGetItem_Func(_id, out string _str) == false)
                            _str = this.parseStrArr[0];

                        return _str;
                    }
                }
                #endregion
#endif
                #region EventToolControl
                [System.Serializable]
                public class EventToolControl
                {
                    public const string Str = "이벤트 툴";
                    public const string NameStr = "EventTool";

                    [SerializeField, LabelText(Event_Script.BeginStr)] private Dictionary<BeginType, Data> beginTypeByDataDic = new Dictionary<BeginType, Data>();
                    [SerializeField, LabelText(Event_Script.CondtionStr)] private Dictionary<ConditionType, Data> conditionTypeByDataDic = new Dictionary<ConditionType, Data>();
                    [SerializeField, LabelText(Event_Script.ActionStr)] private Dictionary<ActionType, Data> actionTypeByDataDic = new Dictionary<ActionType, Data>();

                    [BoxGroup("툴"), SerializeField, LabelText("툴 경로"), FolderPath, DisableIf("IsHideToolPath_Func")]
                    private string toolPathStr = null;

                    private bool IsHideToolPath_Func()
                    {
                        return this.toolPathStr.IsNullOrWhiteSpace_Func() == true;
                    }

                    public string GetToolPathStr
                    {
                        get
                        {
                            if (this.toolPathStr.IsNullOrWhiteSpace_Func() == true)
                                this.toolPathStr = StringBuilder_C.GetPath_Func(Editor_C.SeparatorStr, LibraryRemocon.instance.GetAssetExampleFolderPath, NameStr);

                            return this.toolPathStr;
                        }
                    }

                    public Data GetBeginData_Func(BeginType _beginType)
                    {
                        if (this.beginTypeByDataDic.TryGetValue(_beginType, out Data _data) == false)
                        {
#if UNITY_EDITOR
                            this.CallEdit_SetName_Func(); 
#endif

                            if (this.beginTypeByDataDic.TryGetValue(_beginType, out _data) == false)
                            {
                                Debug_C.Error_Func("다음 타입의 클래스가 정의되지 않은 듯. " + _beginType);
                            }
                        }

                        return _data;
                    }
                    public Data GetConData_Func(ConditionType _conType)
                    {
                        if (this.conditionTypeByDataDic.TryGetValue(_conType, out Data _data) == false)
                        {
#if UNITY_EDITOR
                            this.CallEdit_SetName_Func(); 
#endif

                            if (this.conditionTypeByDataDic.TryGetValue(_conType, out _data) == false)
                            {
                                Debug_C.Error_Func("다음 타입의 클래스가 정의되지 않은 듯. " + _conType);
                            }
                        }

                        return _data;
                    }
                    public Data GetActionData_Func(ActionType _actionType)
                    {
                        if (this.actionTypeByDataDic.TryGetValue(_actionType, out Data _data) == false)
                        {
#if UNITY_EDITOR
                            this.CallEdit_SetName_Func(); 
#endif

                            if (this.actionTypeByDataDic.TryGetValue(_actionType, out _data) == false)
                            {
                                Debug_C.Error_Func("다음 타입의 클래스가 정의되지 않은 듯. " + _actionType);
                            }
                        }

                        return _data;
                    }

#if UNITY_EDITOR
                    [Button(CargoldLibrary_C.CatchingStr)]
                    private void CallEdit_SetName_Func()
                    {
                        if (this.beginTypeByDataDic == null)
                            this.beginTypeByDataDic = new Dictionary<BeginType, Data>();

                        if (this.conditionTypeByDataDic == null)
                            this.conditionTypeByDataDic = new Dictionary<ConditionType, Data>();

                        if (this.actionTypeByDataDic == null)
                            this.actionTypeByDataDic = new Dictionary<ActionType, Data>();

                        GameObject _obj = new GameObject();

                        _Set_Func("Cargold.EventSystem.Event_Begin", (Type _type, Event_Begin _begin) =>
                        {
                            BeginType _beginType = _begin.GetBeginType;

                            if (this.beginTypeByDataDic.ContainsKey(_beginType) == false)
                                this.beginTypeByDataDic.Add(_beginType, new Data(_type));
                        });

                        _Set_Func("Cargold.EventSystem.Event_Condition", (Type _type, Event_Condition _begin) =>
                        {
                            ConditionType _conditionType = _begin.GetConditionType;

                            if (this.conditionTypeByDataDic.ContainsKey(_conditionType) == false)
                                this.conditionTypeByDataDic.Add(_conditionType, new Data(_type));
                        });

                        _Set_Func("Cargold.EventSystem.EventStep_Action", (Type _type, EventStep_Action _action) =>
                        {
                            ActionType _actionType = _action.GetActionType;

                            if (this.actionTypeByDataDic.ContainsKey(_actionType) == false)
                                this.actionTypeByDataDic.Add(_actionType, new Data(_type));
                        });

                        void _Set_Func<T>(string _typeName, Action<Type, T> _del) where T : Component
                        {
                            Type _baseType = System.Type.GetType(_typeName);

                            UnityEditor.TypeCache.TypeCollection _typeCollection = UnityEditor.TypeCache.GetTypesDerivedFrom(_baseType);

                            foreach (Type _type in _typeCollection)
                            {
                                T _component = _obj.AddComponent(_type) as T;
                                _del(_type, _component);
                            }
                        }

                        GameObject.DestroyImmediate(_obj);
                    }

                    [BoxGroup("툴"), Button("이벤트 툴 생성")]
                    private void CallEdit_GenerateTool_Func(System.Type _type)
                    {
                        string _toolPathStr = this.GetToolPathStr;
                        Editor_C.TryGetLoadWithGenerateSobj_Func(NameStr, out ScriptableObject _sobj, _type, _toolPathStr);
                    }

                    // Cargold : 이 함수만 호출하면 자동으로 스트링 드롭다운 스옵젝에 아이템이 추가되고, 스크립트도 만들어지고, 라이브러리 리모콘 이벤트에 해당 아이템이 추가되게끔
                    private void CallEdit_Generate_Func(EventType _eventType, string _typeStr, string _className)
                    {
                        SobjDropdown _sobjDropdown = null;
                    }

                    public enum EventType
                    {
                        None = 0,
                        Begin,
                        Condition,
                        Action,
                    }
#endif

                    [System.Serializable]
                    public class Data
                    {
                        public Type type;
                        public string typeStr;
                        public EventSystem_Manager.LayerType layerType = EventSystem_Manager.LayerType.CallActionByHighestCondition;

                        public Data(Type _type)
                        {
                            this.type = _type;
                            this.typeStr = type.Name;
                            this.layerType = EventSystem_Manager.LayerType.CallActionByHighestCondition;
                        }
                    }
                } 
                #endregion
            }
        }
    }
}