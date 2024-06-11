using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Effect
{
    public class Effect_SetPos_Rtrf_Script : Effect_SetPos_Script
    {
        [SerializeField] private RectTransform targetRtrf;

        public override PosType GetPosType => PosType.Rtrf;

        public void SetTrf_Func(RectTransform _rTrf)
        {
            this.targetRtrf = _rTrf;
        }

        public override void SetPos_Func(Vector2 _pos, Vector2 _layerPos)
        {
            this.targetRtrf.anchoredPosition = _pos;
        }

        private void Reset()
        {
            this.targetRtrf = this.transform as RectTransform;
        }
    }
}