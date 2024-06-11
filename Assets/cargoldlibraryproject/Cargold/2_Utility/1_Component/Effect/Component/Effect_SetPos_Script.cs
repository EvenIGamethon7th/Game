using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Effect
{
    public abstract class Effect_SetPos_Script : MonoBehaviour
    {
        public abstract PosType GetPosType { get; }

        public abstract void SetPos_Func(Vector2 _pos, Vector2 _layerPos);

        private void Reset()
        {
#if UNITY_EDITOR
            if (this.TryGetComponent(out Effect_Script _effectClass) == true)
                _effectClass.CallEdit_SetPos_Func(this); 
#endif
        }
    }

    public enum PosType
    {
        None = 0,

        Trf = 10,
        Rtrf = 20,
    }
}