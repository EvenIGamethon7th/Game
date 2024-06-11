using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.EventSystem
{
    public abstract class Event_Begin : MonoBehaviour
    {
        public abstract BeginType GetBeginType { get; }

#if UNITY_EDITOR
        public virtual string CallEdit_GetSerialize_Func(int _varNum) => string.Empty;
        public abstract bool CallEdit_IsUnitTestDone_Func();
#endif
    }
}