using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.EventSystem
{
    public class Event_Step : MonoBehaviour
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), LabelText(" ")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false
                        , OnBeginListElementGUI = "CallEdit_BeginDrawListElement_Action_Func"
            , CustomRemoveElementFunction = "CallEdit_RemoveElem_Action_Func"
            , OnEndListElementGUI = "CallEdit_EndDrawListElement_Func")]
        protected EventStep_Action[] actionClassArr;

        public EventStep_Action[] GetActionClassArr => this.actionClassArr;

        public void OnAction_Func()
        {
            for (int i = 0; i < actionClassArr.Length; i++)
            {
                EventStep_Action _actionClass = this.actionClassArr[i];

                bool _isLast = i == (this.actionClassArr.Length - 1);
                _actionClass.OnAction_Func(_isLast);
            }
        }

#if UNITY_EDITOR
        [ShowInInspector, LabelText("스텝 내 마지막 액션이면 액션과 함께 자동으로 다음 스텝으로 넘어감?")]
        private bool CallEdit_isNextStepAuto
        {
            get
            {
                if(this.actionClassArr.IsHave_Func() == true)
                {
                    EventStep_Action _lastAction = this.actionClassArr.GetLastItem_Func();

                    if(_lastAction != null)
                    {
                        return _lastAction.IsAutoNextStepWithAction_Func(true);
                    }
                    else
                    {
                        Debug_C.Error_Func(this.transform.GetPath_Func() + " 마지막 액션 컴포가 Null...");
                    }
                }

                return false;
            }
        }

        public virtual string CallEdit_GetSerialize_Func(int _componentNum, int _varNum)
        {
            string _returnStr = string.Empty;

            Event_Script.CallEdit_GetSerialize_Func(ref _returnStr, string.Empty, _componentNum, _varNum, this.actionClassArr, (_actionClass) =>
            {
                ActionType _actionType = _actionClass.GetActionType;
                _returnStr = StringBuilder_C.Append_Func(_returnStr, LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_actionType).typeStr
                    , Event_Script.CallEdit_GetParseStr_Func(2), _actionClass.CallEdit_GetSerialize_Func(_varNum));
            }, _addParseCharID: 1);

            return _returnStr;
        }

        public void CallEdit_RemoveAll_Func()
        {
            for (int i = actionClassArr.Length - 1; i >= 0; i--)
            {
                EventStep_Action _actionClass = this.actionClassArr[i];
                ActionType _actionType = _actionClass.GetActionType;
                var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_actionType);
                Debug_C.Log_Func("(제거) 이벤트 액션 : " + _data.typeStr);

                GameObject.DestroyImmediate(_actionClass, true);
            }
        }
        public void CallEdit_SetActionClassArr_Func(EventStep_Action[] _actionClassArr)
        {
            this.actionClassArr = _actionClassArr;
        }

        #region 액션
        private void CallEdit_BeginDrawListElement_Action_Func(int index)
        {
            EventStep_Action _actionClass = this.actionClassArr[index];
            ActionType _actionType = _actionClass.GetActionType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_actionType);
            if(_data != null)
            {
                string _typeStr = _data.typeStr;
                Sirenix.Utilities.Editor.SirenixEditorGUI.BeginBox(_typeStr);
            }
        }
        private void CallEdit_RemoveElem_Action_Func(EventStep_Action _actionClass)
        {
            var _actionType = _actionClass.GetActionType;
            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_actionType);
            Debug_C.Log_Func("(제거) 이벤트 액션 : " + _data.typeStr);

            this.actionClassArr = this.actionClassArr.GetRemove_Func(_actionClass);

            Component.DestroyImmediate(_actionClass, true);

            this.CallEdit_Save_Func();
        }

        [PropertySpace(20f), Button("추가하기")]
        public void CallEdit_AddAction_Func(ActionType _actionType)
        {
            if (_actionType == ActionType.None)
            {
                Debug_C.Warning_Func("안 골랐는뎁쇼?");

                return;
            }

            var _data = LibraryRemocon.Instance.utilityClassData.eventSystemData.eventToolControl.GetActionData_Func(_actionType);
            System.Type _type = _data.type;
            EventStep_Action _conClass = this.transform.gameObject.AddComponent(_type) as EventStep_Action;
            this.actionClassArr = this.actionClassArr.GetAdd_Func(_conClass);

            Debug_C.Log_Func("(추가) 이벤트 액션 : " + _data.typeStr);
            
            this.CallEdit_Save_Func();
        }
        public void CallEdit_SetAction_Func(EventStep_Action _actionClass)
        {
            this.actionClassArr = new EventStep_Action[] { _actionClass };
        }

        private void CallEdit_Save_Func()
        {
            string _path = Editor_C.GetPath_Func(this.gameObject, true);
            Object _baseObj = Editor_C.GetLoadAssetAtPath_Func(_path);
            Editor_C.SetSaveAsset_Func(_baseObj);
            Editor_C.SetSelection_Func(_baseObj, true);
        }

        private void CallEdit_EndDrawListElement_Func(int index)
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.EndBox();
        }
        #endregion

        public virtual void CallEdit_UnitTest_Func()
        {
            foreach (EventStep_Action _actionClass in this.actionClassArr)
            {
                if (_actionClass.CallEdit_IsUnitTestDone_Func() == false)
                    Event_Script.CallEdit_OnUnitTestLog_Func(_actionClass, _actionClass.GetActionType);
            }
        }
#endif
    }
}