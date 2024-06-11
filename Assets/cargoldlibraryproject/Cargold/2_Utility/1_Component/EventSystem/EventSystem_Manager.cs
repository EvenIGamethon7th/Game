using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.EventSystem
{
    using Cargold.Observer;

    public abstract class EventSystem_Manager : MonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public static EventSystem_Manager Instance;

        [ReadOnly, ShowInInspector] private Event_Script currentEvtClass;
        [ReadOnly, ShowInInspector] private Dictionary<BeginType, List<Event_Script>> beginTypeByEvtClassListDic;
        [ReadOnly, ShowInInspector] private Dictionary<string, Event_Script> clearEvtClassDic;
        [ReadOnly, ShowInInspector] private int currentStepID;
        [ReadOnly, ShowInInspector] private List<Event_Script> activeReserveEvtClassList;
        [ReadOnly, ShowInInspector] private Queue<System.Action> onBeginReserveQueue;
        [ReadOnly, ShowInInspector] private Observer_Action<string> evtClearObs;
        [ReadOnly, ShowInInspector] private List<Event_Script> activateEvtClassList;

        public bool IsActivate => this.currentEvtClass is null == false;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                this.beginTypeByEvtClassListDic = new Dictionary<BeginType, List<Event_Script>>();
                this.clearEvtClassDic = new Dictionary<string, Event_Script>();
                this.activeReserveEvtClassList = new List<Event_Script>();
                this.onBeginReserveQueue = new Queue<System.Action>();
                this.evtClearObs = new Observer_Action<string>();
                this.activateEvtClassList = new List<Event_Script>();

                this.Deactivate_Func(true);
            }
        }

        public virtual void Activate_Func() { }

        public void OnBegin_Func(BeginType _beginType)
        {
            Debug_C.Log_Func("_beginType : " + _beginType.ToString(), Debug_C.PrintLogType.Event);

            this.OnBegin_Func(_beginType, (Event_Begin _beginClass) => true);
        }
        protected void OnBegin_Func<T>(BeginType _beginType, System.Func<T, bool> _del) where T : Event_Begin
        {
            this.OnBegin_Func(_beginType, _del, false, out _);
        }
        protected void OnBegin_Func<T>(BeginType _beginType, System.Func<T, bool> _del, bool _isJustCheck, out ResultType _resultType) where T : Event_Begin
        {
            this.OnBegin_Func(_beginType, _del, _isJustCheck, out _resultType, out _);
        }
        protected virtual void OnBegin_Func<T>(BeginType _beginType, System.Func<T, bool> _del, bool _isJustCheck, out ResultType _resultType, out Event_Script _activateEvtClass)
            where T : Event_Begin
        {
            _activateEvtClass = null;

            // 단순히 확인만 하려는게 아니고, 만약 현재 진행 중인 이벤트가 있다면?
            if (_isJustCheck == false && this.currentEvtClass is null == false)
            {
                _resultType = ResultType.None;

                // 큐 등록
                this.onBeginReserveQueue.Enqueue(() =>
                {
                    this.OnBegin_Func(_beginType, _del, false, out _, out _);
                });

                return; // 무시하기
            }

            var _libraryBeginData = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetBeginData_Func(_beginType);

            // 시작 시점에 해당하는 이벤트 목록이 있는가?
            Debug_C.Log_Func("--- 이벤트 검사를 시작합니다 ---", Debug_C.PrintLogType.Event);
            Debug_C.Log_Func($"{_beginType.ToString()} 시작 시점에 해당하는 이벤트 목록이...", Debug_C.PrintLogType.Event);
            if (this.beginTypeByEvtClassListDic.TryGetValue(_beginType, out List<Event_Script> _evtClassList) == true)
            {
                Debug_C.Log_Func($"있습니다!", Debug_C.PrintLogType.Event);

                // 이벤트 목록이 채워져 있는가?
                Debug_C.Log_Func("이벤트 목록에 이벤트가...", Debug_C.PrintLogType.Event);
                if (_evtClassList.IsHave_Func() == true)
                {
                    Debug_C.Log_Func("있습니다.", Debug_C.PrintLogType.Event);

                    this.activeReserveEvtClassList.Clear();

                    // 시작 시점에 해당하는 이벤트 목록 중에서 참인 것만 골라내기
                    foreach (Event_Script _evtClass in _evtClassList)
                    {
                        foreach (Event_Begin _beginClass in _evtClass.GetBeginClassArr)
                        {
                            if (_beginClass is T == true && _del(_beginClass as T) == true)
                            {
                                Debug_C.Log_Func($"{_evtClass.GetEventKey} 이벤트의 시작 시점은 참입니다.", Debug_C.PrintLogType.Event);

                                this.activeReserveEvtClassList.AddNewItem_Func(_evtClass);
                            }
                        }
                    }

                    // 이벤트 목록이 채워져 있는가?
                    Debug_C.Log_Func("참인 이벤트가 1개라도...", Debug_C.PrintLogType.Event);
                    if (this.activeReserveEvtClassList.IsHave_Func() == true)
                    {
                        Debug_C.Log_Func("있습니다.", Debug_C.PrintLogType.Event);

                        if (_libraryBeginData.layerType == LayerType.CallConditionByHighestBegin)
                        {
                            _SetLayer_Func();

                            Event_Script _evtClass = this.activeReserveEvtClassList[0];
                            _activateEvtClass = this.activeReserveEvtClassList[0];

                            this.activeReserveEvtClassList.Clear();

                            this.activeReserveEvtClassList.Add(_evtClass);
                        }

                        // 이벤트 목록 중 발동 조건에 해당하지 않으면 빼기
                        for (int i = this.activeReserveEvtClassList.Count - 1; i >= 0; i--)
                        {
                            Event_Script _evtClass = this.activeReserveEvtClassList[i];
                            Debug_C.Log_Func($"{_evtClass.GetEventKey} 이벤트의 발동 조건은...", Debug_C.PrintLogType.Event);
                            if (_evtClass.IsConditionDone_Func() == false)
                            {
                                Debug_C.Log_Func("거짓", Debug_C.PrintLogType.Event);

                                this.activeReserveEvtClassList.Remove(_evtClass);
                            }
#if UNITY_EDITOR
                            else
                            {
                                Debug_C.Log_Func("참", Debug_C.PrintLogType.Event);
                            } 
#endif
                        }

                        // 이벤트 목록이 채워져 있는가?
                        if (this.activeReserveEvtClassList.IsHave_Func() == true)
                        {
                            if(_libraryBeginData.layerType == LayerType.CallActionByHighestCondition)
                                _SetLayer_Func();

                            _activateEvtClass = this.activeReserveEvtClassList[0];
                            _resultType = ResultType.Actionable;

                            this.activeReserveEvtClassList.Clear();

                            // 단순히 확인만 하려고 하는게 아닌가?
                            if(_isJustCheck == false)
                            {
                                this.currentEvtClass = _activateEvtClass;
                                this.currentStepID = 0;

                                this.OnEvent_Func(_activateEvtClass);
                            }
                        }
                        else
                        {
                            _resultType = ResultType.ConditionFail;
                        }
                    }
                    else
                    {
                        Debug_C.Log_Func("없습니다.", Debug_C.PrintLogType.Event);

                        _resultType = ResultType.Empty;
                    }
                }
                else
                {
                    _resultType = ResultType.Empty;

                    Debug_C.Log_Func("없습니다.", Debug_C.PrintLogType.Event);
                }
            }
            else
            {
                _resultType = ResultType.Empty;

                Debug_C.Log_Func($"없습니다!", Debug_C.PrintLogType.Event);
            }

            Debug_C.Log_Func($"--- 이벤트 검사의 결과는 '{_resultType}'입니다. ---", Debug_C.PrintLogType.Event);

            void _SetLayer_Func()
            {
                // 이벤트 목록의 개수가 2개 이상인가?
                if (2 <= this.activeReserveEvtClassList.Count)
                {
                    // 우선 순위가 더 높은 이벤트로 정렬
                    this.activeReserveEvtClassList.Sort_Func((Event_Script _left, Event_Script _right) =>
                    {
                        return _left.GetLayer < _right.GetLayer;
                    });
                }
            }
        }
        public bool IsHaveNextStep_Func(ActionType _actionType)
        {
            Debug_C.Log_Func("IsHaveNextStep_Func) " + this.currentStepID + " / _actionType : " + _actionType, Debug_C.PrintLogType.Event);

            if (this.currentEvtClass != null && this.currentEvtClass.GetStepClassArr.TryGetItem_Func(this.currentStepID, out Event_Step _nextStepClass) == true)
            {
                foreach (EventStep_Action _actionClass in _nextStepClass.GetActionClassArr)
                {
                    if (_actionClass.GetActionType == _actionType)
                    {
                        Debug_C.Log_Func("다음 스텝에 찾는 액션 있음 O", Debug_C.PrintLogType.Event);
                        return true;
                    }
                }
            }

            Debug_C.Log_Func("다음 스텝에 찾는 액션 없음 X", Debug_C.PrintLogType.Event);
            return false;
        }
        protected virtual void OnEvent_Func(Event_Script _evtClass)
        {
            this.OnStep_Func();
        }
        public void OnStep_Func()
        {
            if(this.currentEvtClass == null)
            {
                Debug_C.Warning_Func("이 로그를 보신 분은 상황과 함께 카골한테 제보해주세요 ㅇㅅㅇ");
                return;
            }

            // 다음 스텝
            if(this.currentEvtClass.GetStepClassArr.TryGetItem_Func(this.currentStepID, out Event_Step _nextStepClass) == true)
            {
                this.currentStepID++;

                Debug_C.Log_Func("스텝 실행 : " + this.currentStepID, Debug_C.PrintLogType.Event);

                _nextStepClass.OnAction_Func();
            }

            // 이벤트 종료
            else
            {
                Event_Script _clearEvtClass = this.currentEvtClass;
                this.currentEvtClass = null;

                string _eventKey = _clearEvtClass.GetEventKey;
                Debug_C.Log_Func($"스텝 종료. 이벤트키 : {_eventKey}", Debug_C.PrintLogType.Event);

                if(_clearEvtClass.IsRepeat == false)
                {
                    this.OnRemoveEvent_Func(_clearEvtClass);
                }

                this.currentStepID = 0;
                this.clearEvtClassDic.Add_Func(_eventKey, _clearEvtClass);

                this.OnEventDone_Func(_eventKey);

                if (this.onBeginReserveQueue.TryDequeue(out System.Action _del) == true)
                {
                    _del();
                }
            }
        }
        protected virtual void OnEventDone_Func(string _eventKey)
        {
            this.evtClearObs.Notify_Func(_eventKey);
        }
        public void OnRemoveClearEvt_Func(bool _isRestore)
        {
            if(_isRestore == true)
            {
                foreach (var item in this.clearEvtClassDic)
                    this.OnAddEvent_Func(item.Value, false);
            }

            this.clearEvtClassDic.Clear();
        }
        public virtual bool IsClearedEvent_Func(string _eventKey)
        {
            return this.clearEvtClassDic.ContainsKey(_eventKey);
        }

        public void OnAddEvent_Func(EventGroupSobj _eventGroupSobj, bool _isCheckAddable = true)
        {
            foreach (Event_Script _evtClass in _eventGroupSobj.evtClassList)
            {
                OnAddEvent_Func(_evtClass, _isCheckAddable);
            }
        }
        public void OnAddEvent_Func(Event_Script _eventClass, bool _isCheckAddable = true)
        {
            bool _isAddable = true;

            string _eventKey = _eventClass.GetEventKey;
            if (_isCheckAddable == true)
            {
                if (_eventClass.IsRepeat == false && this.IsClearedEvent_Func(_eventKey) == true)
                {
                    _isAddable = false;
                }
            }

            if (_isAddable == true)
            {
                foreach (Event_Begin _beginClass in _eventClass.GetBeginClassArr)
                {
                    BeginType _beginType = _beginClass.GetBeginType;

                    List<Event_Script> _list = this.beginTypeByEvtClassListDic.GetValue_Func(_beginType, () => new List<Event_Script>());
                    _list.Add(_eventClass);
                }

                this.activateEvtClassList.AddNewItem_Func(_eventClass, true);
            }
        }
        public void OnRemoveEvent_Func(EventGroupSobj _eventGroupSobj)
        {
            foreach (Event_Script _evtClass in _eventGroupSobj.evtClassList)
                this.OnRemoveEvent_Func(_evtClass);
        }
        public void OnRemoveEvent_Func(Event_Script _evtClass)
        {
            string _eventKey = _evtClass.GetEventKey;

            Event_Begin[] _beginClassArr = _evtClass.GetBeginClassArr;
            foreach (Event_Begin _beginClass in _beginClassArr)
            {
                BeginType _beginType = _beginClass.GetBeginType;

                if (this.beginTypeByEvtClassListDic.TryGetValue(_beginType, out List<Event_Script> _list) == true)
                {
                    _list.Remove_Func(_evtClass);
                }
                else
                {
                    Debug_C.Error_Func("?");
                }
            }

            this.activateEvtClassList.Remove_Func(_evtClass);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        }

        public void Subscrbie_EvtClear_Func(System.Action<string> _del)
        {
            this.evtClearObs.Subscribe_Func(_del);
        }
        public void Unsubcribe_EvtClear_Func(System.Action<string> _del)
        {
            this.evtClearObs.Unsubscribe_Func(_del);
        }

        public enum ResultType
        {
            None = 0,

            /// <summary>
            /// 시작 시점에 해당하는 이벤트가 없는 경우
            /// </summary>
            Empty,

            /// <summary>
            /// 발동 조건이 미충족하는 경우
            /// </summary>
            ConditionFail, 

            /// <summary>
            /// 이벤트 액션이 가능한 경우
            /// </summary>
            Actionable,
        }

        public enum LayerType
        {
            None = 0,

            /// <summary>
            /// 발동 조건이 참인 이벤트들 중에서 우선 순위가 가장 높은 이벤트의 액션을 실행
            /// </summary>
            CallActionByHighestCondition = 10,

            /// <summary>
            /// 시작 시점이 참인 이벤트들 중에서 우선 순위가 가장 높은 이벤트만의 발동 조건을 비교
            /// </summary>
            CallConditionByHighestBegin = 20,
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }
#endif
    }
}