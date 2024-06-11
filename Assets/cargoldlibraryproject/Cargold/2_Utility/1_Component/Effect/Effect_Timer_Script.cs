using Cargold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Effect
{
    public class Effect_Timer_Script : Effect_Script
    {
        private float baseTime;
        private Coroutine timeCor;

        public override EffectActivateType GetEffectActivateType => EffectActivateType.Timer;

        public override void Init_Func()
        {
            base.Init_Func();

            this.timeCor = null;
        }

        public void SetData_Func(float _time)
        {
            this.baseTime = _time;
        }

        protected override void ActivateDone_Func()
        {
            base.ActivateDone_Func();

            if (timeCor == null)
                timeCor = StartCoroutine(WaitTime_Cor(this.baseTime));
            else
                Debug_C.Error_Func("?");
        }
        private IEnumerator WaitTime_Cor(float _time)
        {
            yield return Coroutine_C.GetWaitForSeconds_Cor(_time);

            base.Deactivate_Func();
        }
        public override void Deactivate_Func(bool _isInit = false, bool _isOutList = true)
        {
            base.Deactivate_Func(_isInit, _isOutList);

            if (timeCor != null)
                StopCoroutine(timeCor);

            timeCor = null;
        }
    } 
}