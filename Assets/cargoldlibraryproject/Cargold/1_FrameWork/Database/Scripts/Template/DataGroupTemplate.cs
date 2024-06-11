using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public abstract class DataGroupTemplate : SerializedScriptableObject
    {
        public abstract string GetTypeNameStr { get; }

        public virtual void Init_Func()
        {

        }

#if UNITY_EDITOR
        [Button("캐싱 ㄱㄱ ~")]
        public virtual void CallEdit_OnDataImport_Func()
        {

        }
#endif
    }
}