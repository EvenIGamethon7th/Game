using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.Loading
{
    public abstract class UI_Loading_Trobber_Script : UI_Loading_Script
    {
        [SerializeField, LabelText("미응답 시 강제 종료 시간")] private float exitTime = 15f;
        private Coroutine activeCor;

        public override Cargold.Loading.LoadingSystem_Manager.LoadingType GetLoadingType => Cargold.Loading.LoadingSystem_Manager.LoadingType.Trobber;

        public override void Activate_Func(Action _activateDoneDel = null)
        {
            if(base.anim != null)
                base.anim.Play_Func();

            base.Activate_Func(_activateDoneDel);

            this.activeCor = Coroutine_C.Invoke_Func(() =>
            {
                Debug_C.Error_Func("?");
                this.Deactivate_Func();
            }, this.exitTime);

            this.ActivateDone_Func();
        }
        public override void Deactivate_Func(Action _deactivateDoneDel = null)
        {
            base.Deactivate_Func(_deactivateDoneDel);

            if(this.activeCor != null)
                Coroutine_C.StopCoroutine_Func(this.activeCor);

            this.activeCor = null;

            this.DeactivateDone_Func();
        }
    } 
}