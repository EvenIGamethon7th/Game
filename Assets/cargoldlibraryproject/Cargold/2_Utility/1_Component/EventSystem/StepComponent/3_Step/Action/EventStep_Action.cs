using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.EventSystem
{
    public abstract class EventStep_Action : MonoBehaviour
    {
        public abstract ActionType GetActionType { get; }
        
        public virtual void OnAction_Func(bool _isCalledLastAction)
        {
            this.OnLog_Func();

            this.OnActionOverride_Func();

            if (this.IsAutoNextStepWithAction_Func(_isCalledLastAction) == true)
            {
                EventSystem_Manager.Instance.OnStep_Func();
            }
        }
        protected abstract void OnActionOverride_Func();
        public virtual bool IsAutoNextStepWithAction_Func(bool _isCalledLastAction)
        {
            return _isCalledLastAction == true;
        }

        protected void OnLog_Func(string _str = null)
        {
            if (_str.IsNullOrWhiteSpace_Func() == true)
                _str = this.OnLogParam_Func();

#if UNITY_EDITOR
            Debug_C.Log_Func($"{this.GetActionType.ToString()} / {_str}", Debug_C.PrintLogType.Event_Action);
#endif
        }
        protected virtual string OnLogParam_Func() => null;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), PropertyOrder(-1), LabelText("마지막일 때 액션과 함께 자동으로 다음 스텝?")]
        private bool CallEdit_IsNextStepLast => this.IsAutoNextStepWithAction_Func(true);
        [ShowInInspector, ReadOnly, FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), PropertyOrder(-1), LabelText("마지막이 아닐 때 액션과 함께 자동으로 다음 스텝?")]
        private bool CallEdit_IsNextStep => this.IsAutoNextStepWithAction_Func(false);

        public virtual string CallEdit_GetSerialize_Func(int _minPathCnt) => string.Empty;
        public abstract bool CallEdit_IsUnitTestDone_Func();
#endif
    }
}