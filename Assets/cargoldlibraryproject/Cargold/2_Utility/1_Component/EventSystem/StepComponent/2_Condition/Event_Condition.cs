using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.EventSystem
{
    public abstract class Event_Condition : MonoBehaviour, ILogic_C
    {
        public abstract ConditionType GetConditionType { get; }
        public abstract bool IsConditionDone_Func();

#if UNITY_EDITOR
        public virtual string CallEdit_GetSerialize_Func(int _varNum) => string.Empty;
        protected virtual void CallEdit_Init_Func() { }
        void Reset() => this.CallEdit_Init_Func();
        public abstract bool CallEdit_IsUnitTestDone_Func();
        protected void OnLog_Func(string _str)
        {
#if UNITY_EDITOR
            Debug_C.Log_Func($"{this.GetConditionType.ToString()}) {_str}", Debug_C.PrintLogType.Event_Condition); 
#endif
        }
#endif

        bool ILogic_C.IsLogicallyTrue_Func() => this.IsConditionDone_Func();
    } 
}