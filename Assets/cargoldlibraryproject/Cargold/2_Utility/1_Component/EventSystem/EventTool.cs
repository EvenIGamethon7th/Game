using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using static Cargold.LibraryRemocon.UtilityClassData;
using UnityEditor;
using System.Windows;

namespace Cargold.EventSystem
{
    public abstract class EventTool : SerializedScriptableObject
    {
        public const string ResourcePathStr = "3_Resources/Data/Event/Resources/";
        public const string EventS = "Event" + Editor_C.SeparatorStr;
        public const string EventDataPathStr = EventS + "Data";
        public const string EventGroupSobjPathStr = EventS + "Sobj";
        public const string EventFullPathStr = ResourcePathStr + EventDataPathStr;
        public const string EventApathStr = Editor_C.GetUnityStartPathStr + Editor_C.SeparatorStr + EventFullPathStr;
        public const string EventGroupPathStr = EventTool.ResourcePathStr + EventTool.EventGroupSobjPathStr;
        public const string EventGroupApathStr = Editor_C.GetUnityStartPathStr + Editor_C.SeparatorStr + EventGroupPathStr;

#if UNITY_EDITOR
        public const string One = "1";
        public const string OneS = One + "/";
        public const string Left = "left";
        public const string LeftS = Left + "/";
        public const string Edit = "편집";
        public const string EditS = Edit + "/";
        public const string List = "목록";
        public const string ListS = List + "/";
        public const string Filter = "필터";
        public const string OLE = OneS + LeftS + Edit;
        public const string OLES = OLE + "/";
        public const string OLL = OneS + LeftS + List;
        public const string Excel = "엑셀";

        public virtual System.Type GetEventGroupSobjType => typeof(Cargold.EventSystem.EventGroupSobj);
#endif
    }

    public abstract class EventTool<EventType> : EventTool where EventType : Event_Script
    {
#if UNITY_EDITOR
        [HorizontalGroup(One, Width = .4f)]
        [VerticalGroup(OneS + Left)]
        [FoldoutGroup(OLE), LabelText("다중 선택 ㄱ?"), LabelWidth(90f)]
        [OnValueChanged("SyncPreview_Func", InvokeOnUndoRedo = false, InvokeOnInitialize = false)]
        public bool isMuitiSelect;

        /// <summary>
        /// 이벤트툴을 통해 생성될 템플릿 이벤트 프리팹
        /// </summary>
        /// <returns></returns>
        protected abstract EventType CallEdit_GetTemplate_Func();

        [FoldoutGroup(OLE), LabelText(FilterSystem.Str)]
        public FilterSystem filter;

        [FoldoutGroup(OLE), LabelText(EventGroupSobjController.Str)]
        public EventGroupSobjController eventGroupSobjController;

        [FoldoutGroup(OLE), LabelText("자동 캐싱 여부"), LabelWidth(90f), InlineButton("CallEdit_OnCatching_Func", "수동 캐싱")]
        public bool isCatchingAuto = true;

        [FoldoutGroup(OLE), Button(CargoldLibrary_C.UnitTestStr)]
        protected virtual void CallEdit_UnitTest_Func()
        {
            this.eventGroupSobjController.UnitTest_Func();

            Debug_C.Log_Func("이벤트툴 유닛테스트 끝");
        }

        /// <summary>
        /// 모든 이벤트 프리팹 저장하기
        /// </summary>
        protected abstract void CallEdit_OnCatching_Func();

        [FoldoutGroup(OLES + Excel), Button("엑셀 데이터로 복사하기!")]
        public void GetSerialize_Func()
        {
            string _str = string.Empty;

            foreach (Event_Script _eventClass in this.eventList)
            {
                _str = StringBuilder_C.Append_Func(_str, _eventClass.CallEdit_GetSerialize_Func(), "\n");
            }

            Editor_C.SetClipboard_Func(_str);
        }

        [BoxGroup(OLL), HorizontalGroup(OLL + "/1"), Button("생성하기", ButtonSizes.Gigantic)]
        private void CreateData_Func()
        {
            if(this.eventGroupSobjController.eventGroupSobjList.Count != 1)
            {
                Debug_C.Error_Func("이벤트를 삽입할 이그젝을 하나만 선택해주세요.");
                return;
            }

            EventType _templateObj = this.CallEdit_GetTemplate_Func();
            GameObject _gobj = Editor_C.OnGeneratePrefab_Func(_templateObj.gameObject, EventApathStr);

            EventType _evtClass = _gobj.GetComponent<EventType>();
            this.eventGroupSobjController.eventGroupSobjList[0].CallEdit_AddEvt_Func(_evtClass);

            this.CallEdit_OnCatching_Func();
        }

        [BoxGroup(OLL), HorizontalGroup(OLL + "/1"), Button("삭제하기", ButtonSizes.Gigantic)]
        private void DeleteData_Func()
        {
            List<EventGroupSobj> _eventGroupSobjList = this.eventGroupSobjController.eventGroupSobjList;
            if (_eventGroupSobjList.IsHave_Func() == false)
            {
                Debug_C.Error_Func("이벤트를 제거할 이그젝을 선택해주세요.");
                return;
            }

            if (this.eventList.IsHave_Func() == true)
            {
                this.isPass = true;

                for (int i = this.eventList.Count - 1; i >= 0; i--)
                {
                    EventType _eventClass = this.eventList[i];

                    foreach (EventGroupSobj _evtGroupSobj in _eventGroupSobjList)
                    {
                        if (_evtGroupSobj.CallEdit_TryRemoveEvt_Func(_eventClass) == true)
                        {
                            Editor_C.DeleteAsset_Func(_eventClass.gameObject);

                            this.eventList.Remove(_eventClass);

                            break;
                        }
                    }
                }

                this.CallEdit_OnCatching_Func();

                this.SetSystemMsg_Func();
            }
            else
            {
                Debug_C.Log_Func("선택된게 없는뎅?");
            }
        }

        [BoxGroup(OLL), AssetList(Path = EventFullPathStr, CustomFilterMethod = "IsFilter_Func"), LabelText(" ")]
        [OnValueChanged("SyncPreview_Func", InvokeOnUndoRedo = false, InvokeOnInitialize = false)]
        public List<EventType> eventList;

        [VerticalGroup(OneS + "right"), BoxGroup("1/right/이벤트 데이터"), ShowIf("@this.eventList.Count == 1")]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.CompletelyHidden)]
        public EventType eventClass;

        [BoxGroup("1/right/이벤트 데이터"), ShowIf("@eventClass == null"), ReadOnly]
        public string systemMsg = "다중 선택 중 ~";
        private bool isPass = false;

        private void SyncPreview_Func()
        {
            // 의역 : 로직을 무시할 건가요?
            if (this.isPass == true)
            {
                this.isPass = false;
                return;
            }

            if (this.isMuitiSelect == false)
            {
                // 의역 : 선택한 이벤트가 아닌 다른 이벤트를 선택했는가? (직역 : 선택된 이벤트가 있는가?) (해석 : 동일 이벤트를 반복 선택했을 때 로직 문제가 있어서...)
                if(this.eventList.IsHave_Func() == true)
                {
                    // 의역 : 선택 이벤트 개수를 1개로 만들기 (직역 : 선택된 이벤트 개수가 2개 이상인가?)
                    while (2 <= this.eventList.Count)
                        this.eventList.RemoveAt(0); // 첫번째로 선택된 이벤트 제거

                    if (this.eventList.Count == 1)
                    {
                        this.eventClass = this.eventList[0];

                        this.eventClass.CallEdit_OnSelection_Func();
                    }

                    if (this.eventList.IsHave_Func() == false)
                    {
                        this.eventList.Add(this.eventClass);

                        this.isPass = true;
                    }
                }
            }
            else
            {
                this.eventClass = null;
                
                this.SetSystemMsg_Func();
            }

            if (this.isCatchingAuto == true)
            {
                this.CallEdit_OnCatching_Func();
            }
        }

        private void SetSystemMsg_Func()
        {
            if (this.isMuitiSelect == true)
                this.systemMsg = "다중 선택 모드입니다.";
            else
                this.systemMsg = "선택된게 없네용~";
        }

        private bool IsFilter_Func(EventType _eventClass)
        {
            if (this.filter == null)
                return true;

            return this.filter.IsFilter_Func(_eventClass) == true && this.eventGroupSobjController.IsFilter_Func(_eventClass) == true;
        }
#endif
    }

    #region FilterSystem
#if UNITY_EDITOR
    [System.Serializable]
    public class FilterSystem
    {
        public const string Str = "필터";

        [LabelText("이름 검색"), LabelWidth(100f)] public string searchNameStr;
        [LabelText(Event_Script.BeginStr)] public List<BeginType> beginTypeList;
        [LabelText(Event_Script.CondtionStr)] public List<ConditionType> conditionTypeList;
        [LabelText(Event_Script.ActionStr)] public List<ActionType> actionTypeList;

        public bool IsFilter_Func(Event_Script _eventClass)
        {
            bool _isDone = true;

            #region 이름 검색
            if (this.searchNameStr.IsNullOrWhiteSpace_Func() == false)
            {
                if (_eventClass.gameObject.name.Contains(this.searchNameStr) == false)
                    return false;
            }
            #endregion

            #region 시작 시점
            if (this.beginTypeList.IsHave_Func() == true)
            {
                foreach (BeginType _beginType in this.beginTypeList)
                {
                    bool _isFindType = false;

                    if (_eventClass.GetBeginClassArr.IsHave_Func() == true)
                    {
                        foreach (Event_Begin _beginClass in _eventClass.GetBeginClassArr)
                        {
                            if (_beginType == _beginClass.GetBeginType)
                            {
                                _isFindType = true;
                                break;
                            }
                        }
                    }

                    if (_isFindType == false)
                    {
                        _isDone = false;
                        break;
                    }
                }
            }

            if (_isDone == false) return false;
            #endregion

            #region 발동 조건
            if (this.conditionTypeList.IsHave_Func() == true)
            {
                foreach (ConditionType _conType in this.conditionTypeList)
                {
                    if (_eventClass.GetConditionClassArr.IsHave_Func() == true)
                    {
                        if (_eventClass.GetLogicType == LogicType.AND)
                        {
                            foreach (Event_Condition _conClass in _eventClass.GetConditionClassArr)
                            {
                                if (_conType != _conClass.GetConditionType)
                                {
                                    _isDone = false;
                                    break;
                                }
                            }
                        }
                        else if (_eventClass.GetLogicType == LogicType.OR)
                        {
                            bool _isTrue = false;
                            foreach (Event_Condition _conClass in _eventClass.GetConditionClassArr)
                            {
                                if (_conType == _conClass.GetConditionType)
                                {
                                    _isTrue = true;
                                    break;
                                }
                            }

                            if (_isTrue == false)
                                _isDone = false;
                        }
                        else
                        {
                            Debug_C.Error_Func("LogicType : " + _eventClass.GetLogicType);
                        }
                    }
                    else
                    {
                        _isDone = false;
                    }

                    if (_isDone == false)
                        break;
                }
            }

            if (_isDone == false) return false;
            #endregion

            #region 액션
            if (this.actionTypeList.IsHave_Func() == true)
            {
                foreach (ActionType _actionType in this.actionTypeList)
                {
                    bool _isFindType = false;

                    if (_eventClass.GetStepClassArr.IsHave_Func() == true)
                    {
                        foreach (Event_Step _stepClass in _eventClass.GetStepClassArr)
                        {
                            if (_stepClass.GetActionClassArr.IsHave_Func() == true)
                            {
                                foreach (EventStep_Action _actionClass in _stepClass.GetActionClassArr)
                                {
                                    if (_actionType == _actionClass.GetActionType)
                                    {
                                        _isFindType = true;
                                        break;
                                    }
                                }
                            }

                            if (_isFindType == true)
                                break;
                        }
                    }

                    if (_isFindType == false)
                    {
                        _isDone = false;
                        break;
                    }
                }
            }

            if (_isDone == false) return false;
            #endregion

            return _isDone;
        }
    }
#endif
    #endregion
    #region EventGroupSobjController
#if UNITY_EDITOR
    [System.Serializable]
    public class EventGroupSobjController
    {
        public const string Str = "이그젝";

        [ShowInInspector, LabelText("이름"), LabelWidth(100f)] private string sobjNameStr = null;
        [AssetList(Path = EventTool.EventGroupPathStr), LabelText(" "), OnValueChanged("SyncPreview_Func")]
        public List<EventGroupSobj> eventGroupSobjList;

        [HorizontalGroup("1"), Button("생성", ButtonSizes.Gigantic)]
        private void OnGenerate_Func()
        {
            if(this.sobjNameStr.IsNullOrWhiteSpace_Func() == true)
            {
                Debug_C.Error_Func("이그젝에 이름을 넣어주세요.");
                return;
            }

            System.Type _type = typeof(Cargold.EventSystem.EventGroupSobj);
            Editor_C.TryGetLoadWithGenerateSobj_Func(this.sobjNameStr, out ScriptableObject _sobj, _type, EventTool.EventGroupApathStr);

            this.sobjNameStr = null;
        }

        [HorizontalGroup("1"), Button("제거", ButtonSizes.Gigantic)]
        private void OnDelete_Func()
        {
            List<EventGroupSobj> _eventGroupSobjList = this.eventGroupSobjList;
            if (_eventGroupSobjList.IsHave_Func() == false)
            {
                Debug_C.Error_Func("이벤트를 제거할 이그젝을 선택해주세요.");
                return;
            }

            if (this.eventGroupSobjList.IsHave_Func() == true)
            {
                for (int i = this.eventGroupSobjList.Count - 1; i >= 0; i--)
                {
                    EventGroupSobj _eventGroupSobj = this.eventGroupSobjList[i];

                    if(_eventGroupSobj.evtClassList.IsHave_Func() == false)
                    {
                        this.eventGroupSobjList.Remove(_eventGroupSobj);

                        Editor_C.DeleteAsset_Func(_eventGroupSobj);
                    }
                    else
                    {
                        Debug_C.Error_Func(_eventGroupSobj.name + " 이그젝 내 이벤트 프리팹이 남아있습니다.");
                    }
                }
            }
            else
            {
                Debug_C.Log_Func("선택된게 없는뎅?");
            }
        }

        private void SyncPreview_Func()
        {
            if (this.eventGroupSobjList.IsHave_Func() == true)
            {
                while (2 <= this.eventGroupSobjList.Count)
                    this.eventGroupSobjList.RemoveAt(0);
            }
        }

        public bool IsFilter_Func(Event_Script _eventClass)
        {
            bool _isDone = true;

            #region 이그젝
            if (this.eventGroupSobjList.IsHave_Func() == true)
            {
                bool _isDone2 = false;

                foreach (var _eventGroupSobj in this.eventGroupSobjList)
                {
                    if (_eventGroupSobj.evtClassList.Contains(_eventClass) == true)
                    {
                        _isDone2 = true;
                        break;
                    }
                }

                if (_isDone2 == false)
                    _isDone = false;
            }
            #endregion

            return _isDone;
        }

        public void UnitTest_Func()
        {
            foreach (EventGroupSobj _eventGroupSobj in this.eventGroupSobjList)
            {
                _eventGroupSobj.CallEdit_UnitTest_Func();
            }
        }
    }
#endif
    #endregion
}

public partial class CargoldTool
{
#if UNITY_EDITOR
    [MenuItem("Cargold/이벤트 툴 %#&1")]
    private static void SelectTool_Func()
    {
        LibraryRemocon.UtilityClassData.EventSystemData.EventToolControl _eventToolControl = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl;
        string _path = _eventToolControl.GetToolPathStr;
        string _name = LibraryRemocon.UtilityClassData.EventSystemData.EventToolControl.NameStr;
        _path = Editor_C.GetPath_Func(_path, _name);
        Selection.activeObject = Editor_C.GetLoadAssetAtPath_Func<ScriptableObject>(_path, _isAddExtentionStr: true);
    } 
#endif
}