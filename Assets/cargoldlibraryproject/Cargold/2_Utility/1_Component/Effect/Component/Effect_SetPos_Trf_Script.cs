using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Effect
{
    public class Effect_SetPos_Trf_Script : Effect_SetPos_Script
    {
        [SerializeField] private Transform targetTrf = null;
        [SerializeField] private UnityEngine.Space space = Space.World;

        public override PosType GetPosType => PosType.Rtrf;

        public void SetTrf_Func(Transform _trf)
        {
            this.targetTrf = _trf;
        }

        public override void SetPos_Func(Vector2 _pos, Vector2 _layerPos)
        {
            if(this.space == Space.World)
                this.transform.position = _pos;
            else
                this.transform.localPosition = _pos;
        }

        private void Reset()
        {
            this.targetTrf = this.transform;
        }
    } 
}