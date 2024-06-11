using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor; 
#endif

namespace Cargold.EventSystem
{
    public abstract class Event_Script : MonoBehaviour // C
    {
        public const string MainStr = "메인";
        public const string BeginStr = "시작 시점";
        public const string CondtionStr = "발동 조건";
        public const string StepStr = "스텝";
        public const string ActionStr = "이벤트 액션";
        protected const string BeginGroupTrfName = "Begin_Group";
        protected const string ConGroupTrfName = "Condition_Group";
        protected const string StepGroupTrfName = "Step_Group";
        protected const string KeyDropDownStr = "Key 드롭다운";

        [SerializeField, FoldoutGroup(CargoldLibrary_C.Optional)] private EventKeyItem eventKeyItem;

        [SerializeField, HideLabel, LabelWidth(100f), LabelText("반복 여부"), BoxGroup(MainStr), OnValueChanged("CallEdit_Save_Func")] protected bool isRepeat = false;
        [SerializeField, HideLabel, LabelWidth(100f), LabelText("우선 순위"), InfoBox("높을 수록 먼저"), BoxGroup(MainStr)] protected int layer = 0;

#if UNITY_EDITOR
        [HideLabel, ShowInInspector, GUIColor(0f, 0f, 0f, 0f), ReadOnly, PropertySpace(-5f)] private string fuckOdin1;
#endif
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), FoldoutGroup(BeginStr), LabelText(" "), GUIColor("CallEdit_GetBeginColor_Func")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, Expanded = true
            , OnBeginListElementGUI = "CallEdit_BeginDrawListElement_Begin_Func"
            , CustomRemoveElementFunction = "CallEdit_RemoveElem_Begin_Func"
            , OnEndListElementGUI = "CallEdit_EndDrawListElement_Func")]
        protected Event_Begin[] beginClassArr;

#if UNITY_EDITOR
        [HideLabel, ShowInInspector, GUIColor(0f, 0f, 0f, 0f), ReadOnly, PropertySpace(-5f)] private string fuckOdin2; 
#endif
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), FoldoutGroup(CondtionStr), LabelText(" "), GUIColor("CallEdit_GetConColor_Func")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, Expanded = true
                        , OnBeginListElementGUI = "CallEdit_BeginDrawListElement_Con_Func"
            , CustomRemoveElementFunction = "CallEdit_RemoveElem_Con_Func"
            , OnEndListElementGUI = "CallEdit_EndDrawListElement_Func")]
        protected Event_Condition[] conditionClassArr;

        [FoldoutGroup(CondtionStr), LabelText("논리식"), SerializeField, LabelWidth(100f)] private LogicType logicType = LogicType.AND;

#if UNITY_EDITOR
        [HideLabel, ShowInInspector, GUIColor(0f, 0f, 0f, 0f), PropertySpace(-5f)] private string fuckOdin3;
#endif
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), FoldoutGroup(StepStr), LabelText(" "), GUIColor("CallEdit_ActionColor_Func")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, Expanded = true
                        , OnBeginListElementGUI = "CallEdit_BeginDrawListElement_Step_Func"
            , CustomRemoveElementFunction = "CallEdit_RemoveElem_Step_Func"
            , OnEndListElementGUI = "CallEdit_EndDrawListElement_Func")]
        protected Event_Step[] stepClassArr;

        public EventKeyItem GetEventKeyItem
        {
            get
            {
#if UNITY_EDITOR
                if (this.eventKeyItem == null)
                {
                    this.eventKeyItem = new EventKeyItem(this);
                    Editor_C.SetSaveAsset_Func(this);
                }
                else if (this.eventKeyItem.GetObj == null)
                {
                    this.eventKeyItem.Init_Func(this);
                    Editor_C.SetSaveAsset_Func(this);
                }
#endif

                return this.eventKeyItem;
            }
        }

        public Event_Begin[] GetBeginClassArr => this.beginClassArr;
        public Event_Condition[] GetConditionClassArr => this.conditionClassArr;
        public Event_Step[] GetStepClassArr => this.stepClassArr;
        public bool IsRepeat => this.isRepeat;
        public int GetLayer => this.layer;
        public abstract string GetEventKey { get; }
        public LogicType GetLogicType => this.logicType;

        public bool IsConditionDone_Func()
        {
            if (this.conditionClassArr.IsHave_Func() == true)
            {
                return this.conditionClassArr.IsLogicallyTrue_Func(this.logicType);
            }
            else
            {
                return true;
            }
        }

        public bool TryGetAction_Func<T>(ActionType _actionType, out T _returnActionClass) where T : EventStep_Action
        {
            foreach (Event_Step _stepClass in this.stepClassArr)
            {
                foreach (EventStep_Action _actionClass in _stepClass.GetActionClassArr)
                {
                    if (_actionClass.GetActionType == _actionType)
                    {
                        _returnActionClass = _actionClass as T;

                        return true;
                    }
                }
            }

            _returnActionClass = null;
            return false;
        }
        public void TryGetAction_Func<T>(ActionType _actionType, Action<T> _returnActionClass) where T : EventStep_Action
        {
            foreach (Event_Step _stepClass in this.stepClassArr)
            {
                foreach (EventStep_Action _actionClass in _stepClass.GetActionClassArr)
                {
                    if (_actionClass.GetActionType == _actionType)
                    {
                        _returnActionClass(_actionClass as T);
                    }
                }
            }
        }

#if UNITY_EDITOR
        [SerializeField, HideLabel, LabelWidth(100f), LabelText("이벤트 Key"), BoxGroup(MainStr), OnValueChanged("CallEdit_SetKey_Func")] protected string eventKey = null;
        private void CallEdit_SetKey_Func()
        {
            string _nameStr = this.eventKey;
            Editor_C.SetRenameAsset_Func(this, this.eventKey, false, false);
            this.eventKey = _nameStr;

            this.CallEdit_Save_Func();
        }

        public void CallEdit_OnSelection_Func()
        {
            if (this.eventKey.IsCompare_Func(this.gameObject.name) == false)
            {
                this.eventKey = this.gameObject.name;

                this.CallEdit_Save_Func();
            }
        }

        #region 시작 시점
        private Color CallEdit_GetBeginColor_Func()
        {
            return new Color(1f, 0.9f, .9f);
        }
        private void CallEdit_BeginDrawListElement_Begin_Func(int index)
        {
            Event_Begin _beginClass = this.beginClassArr[index];
            BeginType _beginType = _beginClass.GetBeginType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetBeginData_Func(_beginType);
            string _typeStr = _data.typeStr;
            SirenixEditorGUI.BeginBox(_typeStr);
        }
        private void CallEdit_RemoveElem_Begin_Func(Event_Begin _beginClass)
        {
            BeginType _beginType = _beginClass.GetBeginType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetBeginData_Func(_beginType);
            Debug_C.Log_Func("(제거) 시작 시점 : " + _data.typeStr);

            this.beginClassArr = this.beginClassArr.GetRemove_Func(_beginClass);

            Component.DestroyImmediate(_beginClass, true);

            this.CallEdit_Save_Func();
        }

        [FoldoutGroup(BeginStr), PropertySpace(20f), Button("추가하기", Style = ButtonStyle.Box), GUIColor("CallEdit_GetBeginColor_Func")]
        private void CallEdit_AddBegin_Func(BeginType _beginType)
        {
            if (_beginType == BeginType.None)
            {
                Debug_C.Warning_Func("안 골랐는뎁쇼?");

                return;
            }

            Transform _beginGroupTrf = this.beginClassArr.IsHave_Func() == true ? this.beginClassArr[0].transform : this.transform.Find(BeginGroupTrfName);
            if (_beginGroupTrf == null)
            {
                Debug_C.Error_Func("?");
                return;
            }

            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetBeginData_Func(_beginType);
            Type _type = _data.type;
            Event_Begin _beginClass = _beginGroupTrf.gameObject.AddComponent(_type) as Event_Begin;
            this.beginClassArr = this.beginClassArr.GetAdd_Func(_beginClass);

            Debug_C.Log_Func("(추가) 시작 시점 : " + _data.typeStr);

            this.CallEdit_Save_Func();
        }
        #endregion
        #region 발동 조건
        private Color CallEdit_GetConColor_Func()
        {
            return new Color(.9f, 1f, .9f);
        }
        private void CallEdit_BeginDrawListElement_Con_Func(int index)
        {
            Event_Condition _conClass = this.conditionClassArr[index];
            ConditionType _conType = _conClass.GetConditionType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetConData_Func(_conType);
            string _typeStr = _data.typeStr;
            SirenixEditorGUI.BeginBox(_typeStr);
        }
        private void CallEdit_RemoveElem_Con_Func(Event_Condition _conClass)
        {
            ConditionType _conType = _conClass.GetConditionType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetConData_Func(_conType);
            Debug_C.Log_Func("(제거) 발동 조건 : " + _data.typeStr);

            this.conditionClassArr = this.conditionClassArr.GetRemove_Func(_conClass);

            Component.DestroyImmediate(_conClass, true);

            this.CallEdit_Save_Func();
        }

        [FoldoutGroup(CondtionStr), PropertySpace(20f), Button("추가하기", Style = ButtonStyle.Box), GUIColor("CallEdit_GetConColor_Func")]
        private void CallEdit_AddCon_Func(ConditionType _conType)
        {
            if (_conType == ConditionType.None)
            {
                Debug_C.Warning_Func("안 골랐는뎁쇼?");

                return;
            }

            Transform _conGroupTrf = this.conditionClassArr.IsHave_Func() == true ? this.conditionClassArr[0].transform : this.transform.Find(ConGroupTrfName);
            if (_conGroupTrf == null)
                Debug_C.Error_Func("?");

            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetConData_Func(_conType);
            Type _type = _data.type;
            Event_Condition _conClass = _conGroupTrf.gameObject.AddComponent(_type) as Event_Condition;
            this.conditionClassArr = this.conditionClassArr.GetAdd_Func(_conClass);

            Debug_C.Log_Func("(추가) 발동 조건 : " + _data.typeStr);

            this.CallEdit_Save_Func();
        }
        #endregion
        #region 스텝
        private void CallEdit_BeginDrawListElement_Step_Func(int index)
        {
            SirenixEditorGUI.BeginBox((index + 1).ToString());
        }
        private void CallEdit_RemoveElem_Step_Func(Event_Step _stepClass)
        {
            Debug_C.Log_Func("(제거) 스텝");

            _stepClass.CallEdit_RemoveAll_Func();

            this.stepClassArr = this.stepClassArr.GetRemove_Func(_stepClass);

            GameObject.DestroyImmediate(_stepClass, true);

            this.CallEdit_Save_Func();
        }

        private Color CallEdit_ActionColor_Func()
        {
            return new Color(.9f, .9f, 1f);
        }
        [FoldoutGroup(StepStr), PropertySpace(20f), Button("추가하기", Style = ButtonStyle.Box), GUIColor("CallEdit_ActionColor_Func")]
        private void CallEdit_AddStep_Func()
        {
            this.CallEdit_AddStep_Func(true);
        }
        public void CallEdit_AddStep_Func(bool _isLog)
        {
            Transform _stepGroupTrf = this.stepClassArr.IsHave_Func() == true ? this.stepClassArr[0].transform : this.transform.Find(StepGroupTrfName);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
            if (_stepGroupTrf == null)
                Debug_C.Error_Func("?");

            Event_Step _stepClass = _stepGroupTrf.gameObject.AddComponent<Event_Step>();
            this.stepClassArr = this.stepClassArr.GetAdd_Func(_stepClass);

            if(_isLog == true)
                Debug_C.Log_Func("(추가) 스텝 ID : " + this.stepClassArr.Length);

            this.CallEdit_Save_Func(_isLog);
        }
        #endregion
        private void CallEdit_EndDrawListElement_Func(int index)
        {
            SirenixEditorGUI.EndBox();
        }

        void Reset()
        {
            this.CallEdit_Catching_Func();
        }
        protected virtual void CallEdit_Catching_Func()
        {
            this.beginClassArr = _SetTrf_Func<Event_Begin>(BeginGroupTrfName);
            this.conditionClassArr = _SetTrf_Func<Event_Condition>(ConGroupTrfName);
            _SetTrf_Func<Event_Step>(StepGroupTrfName, false);

            this.stepClassArr = this.GetComponentsInChildren<Event_Step>();

            this.CallEdit_Save_Func();

            T[] _SetTrf_Func<T>(string _objName, bool _isGetComponent = true) where T : Component
            {
                Transform _groupTrf = this.transform.Find(_objName);

                if(_groupTrf != null)
                {
                    if (_isGetComponent == true)
                        return _groupTrf.GetComponents<T>();
                    else
                        return null;
                }
                else
                {
                    Transform _trf = new GameObject(_objName).transform;
                    _trf.SetParent(this.transform);

                    return null;
                }
            }
        }

        public virtual string CallEdit_GetSerialize_Func()
        {
            LibraryRemocon.UtilityClassData.EventSystemData.SerializeData _serializeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.serializeData;
            Func<int, string> _getParseStrFunc = _serializeData.GetParseStr_Func;

            string _returnStr = StringBuilder_C.GetPath_Func(_getParseStrFunc(0), this.gameObject.name, this.GetEventKey, this.isRepeat.ToString());

            string _beginChar = _serializeData.GetBeginChar;
            int _beginComponentNum = _serializeData.beginComponentNum;
            int _beginVarNum = _serializeData.beginVarNum;
            CallEdit_GetSerialize_Func(ref _returnStr, _beginChar, _beginComponentNum, _beginVarNum, this.beginClassArr, (_beginClass) =>
            {
                BeginType _beginType = _beginClass.GetBeginType;
                _returnStr = StringBuilder_C.Append_Func(_returnStr, LibraryRemocon.UtilityClassData.EventSystemData.Insance.eventToolControl.GetBeginData_Func(_beginType).typeStr,
                    _getParseStrFunc(1), _beginClass.CallEdit_GetSerialize_Func(_beginVarNum));
            });

            string _conditionChar = _serializeData.GetConditionChar;
            int _conditionComponentNum = _serializeData.conditionComponentNum;
            int _conditionVarNum = _serializeData.conditionVarNum;
            CallEdit_GetSerialize_Func(ref _returnStr, _conditionChar, _conditionComponentNum, _conditionVarNum, this.conditionClassArr, (_conditionClass) =>
            {
                ConditionType _conType = _conditionClass.GetConditionType;
                _returnStr = StringBuilder_C.Append_Func(_returnStr, LibraryRemocon.UtilityClassData.EventSystemData.Insance.eventToolControl.GetConData_Func(_conType).typeStr,
                    _getParseStrFunc(1), _conditionClass.CallEdit_GetSerialize_Func(_conditionVarNum));
            });

            string _stepChar = _serializeData.GetStepChar;
            int _stepNum = _serializeData.stepNum;
            int _actionComponentNum = _serializeData.actionComponentNum;
            int _actionVarNum = _serializeData.actionVarNum;
            CallEdit_GetSerialize_Func(ref _returnStr, _stepChar, _stepNum, 0, this.stepClassArr, (_stepClass) =>
            {
                _returnStr = StringBuilder_C.Append_Func(_returnStr, _stepClass.CallEdit_GetSerialize_Func(_actionComponentNum, _actionVarNum));
            }, () =>
            {
                for (int i = 0; i < _actionComponentNum; i++)
                {
                    _returnStr = StringBuilder_C.GetPath_Func(_getParseStrFunc(2), _actionVarNum, _returnStr);
                    _returnStr = StringBuilder_C.Append_Func(_returnStr, _getParseStrFunc(1));
                }
            }, 0);

            return _returnStr;
        }

        public void CallEdit_SetRepeat_Func(bool _isOn)
        {
            this.isRepeat = _isOn;

            this.CallEdit_Save_Func();
        }
        private void CallEdit_Save_Func(bool _isLog = true)
        {
            if(_isLog == true)
                Debug_C.Log_Func("이벤트 프리팹 저장 : " + this.eventKey);

            if (this.eventKeyItem == null)
                this.eventKeyItem = new EventKeyItem(this);

            if (this.eventKeyItem.GetObj == null)
                this.eventKeyItem.Init_Func(this);

            Editor_C.SetSaveAsset_Func(this.gameObject);

            Editor_C.SetSelection_Func(this.gameObject);
        }

        public virtual void CallEdit_UnitTest_Func()
        {
            foreach (Event_Begin _beginClass in this.beginClassArr)
            {
                if (_beginClass.CallEdit_IsUnitTestDone_Func() == false)
                    Event_Script.CallEdit_OnUnitTestLog_Func(_beginClass, _beginClass.GetBeginType);
            }

            foreach (Event_Condition _conditionClass in this.conditionClassArr)
            {
                if (_conditionClass.CallEdit_IsUnitTestDone_Func() == false)
                    Event_Script.CallEdit_OnUnitTestLog_Func(_conditionClass, _conditionClass.GetConditionType);
            }

            foreach (Event_Step _stepClass in this.stepClassArr)
                _stepClass.CallEdit_UnitTest_Func();
        }

        public static string CallEdit_GetParseStr_Func(int _id)
        {
            LibraryRemocon.UtilityClassData.EventSystemData.SerializeData _serializeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.serializeData;
            return _serializeData.GetParseStr_Func(_id);
        }
        public static void CallEdit_GetSerialize_Func<T>(ref string _str, string _parseChar, int _componentNum, int _varNum, T[] _classArr
            , Action<T> _trueDel, Action _falseDel = null, int _addParseCharID = 0) where T : Component
        {
            LibraryRemocon.UtilityClassData.EventSystemData.SerializeData _serializeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.serializeData;

            _str = StringBuilder_C.Append_Func(_str, _parseChar);
            for (int i = 0;;)
            {
                if (_classArr.TryGetItem_Func(i, out T _beginClass) == true)
                {
                    _trueDel(_beginClass);
                }
                else
                {
                    if (_falseDel == null)
                        _str = StringBuilder_C.GetPath_Func(_serializeData.GetParseStr_Func(_addParseCharID + 1), _varNum + 1, _str);
                    else
                        _falseDel();
                }

                i++;
                if (_componentNum <= i)
                    break;

                _str = StringBuilder_C.Append_Func(_str, _serializeData.GetParseStr_Func(_addParseCharID));
            }
        }
        public static void CallEdit_OnUnitTestLog_Func<ComponentType>(ComponentType _componentType, int _typeStr) where ComponentType : MonoBehaviour
        {
            string _pathStr = _componentType.transform.parent.GetPath_Func();
            string _componentTypeStr = null;
            LibraryRemocon.UtilityClassData.EventSystemData.EventToolControl.Data _typeData = null;
            if (_componentType is Event_Begin)
            {
                _componentTypeStr = Event_Script.BeginStr;
                _typeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetBeginData_Func(_typeStr);
            }
            else if (_componentType is Event_Condition)
            {
                _componentTypeStr = Event_Script.CondtionStr;
                _typeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetConData_Func(_typeStr);
            }
            else if (_componentType is EventStep_Action)
            {
                _componentTypeStr = Event_Script.ActionStr;
                _typeData = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_typeStr);
            }
            else
            {
                Type _componentTypeClass = typeof(ComponentType);
                Debug_C.Error_Func(_componentTypeClass.ToString());
            }
            string _valueTypeStr = _typeData.typeStr;

            Debug_C.Error_Func("유닛테스트 실패) " + _pathStr + " " + _componentTypeStr + " '" + _valueTypeStr + "'에 문제가 있습니다.");
        }
#endif
    }

    [HideLabel]
    public abstract class BaseEventKey : Cargold.ObjDropdownKey<Cargold.EventSystem.Event_Script>
    {
        public BaseEventKey(Cargold.EventSystem.Event_Script _key) : base(_key) { }

#if UNITY_EDITOR
        protected override string CallEdit_GetKeyName => "이벤트 Key";
#endif
    }
    [System.Serializable]
    public class EventKeyItem : Cargold.ObjDropdownItem<Cargold.EventSystem.Event_Script>
    {
        public EventKeyItem(Cargold.EventSystem.Event_Script _obj) : base(_obj) { }
    }
}