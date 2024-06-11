using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.Loading
{
    public abstract class UI_Loading_Transition_Script : UI_Loading_Script
    {
        [SerializeField, LabelText("연출")] private IDirectionComponent[] iDirectionComponentArr = null;
        [SerializeField, LabelText("비활성화 딜레이")] protected float deactivateDelay = 1f;
        [SerializeField, LabelText("시작 애니메이션")] private AniData_C enterAniData = null;
        [SerializeField, LabelText("종료 애니메이션")] private AniData_C exitAniData = null;

        public override Cargold.Loading.LoadingSystem_Manager.LoadingType GetLoadingType => Cargold.Loading.LoadingSystem_Manager.LoadingType.Transition;

        public override void Init_Func()
        {
            foreach (IDirectionComponent _iDirectionComponent in this.iDirectionComponentArr)
                _iDirectionComponent.Init_Func();

            base.Init_Func();
        }

        public override void Activate_Func(Action _activateDoneDel = null)
        {
            base.Activate_Func(_activateDoneDel);

            foreach (IDirectionComponent _iDirectionComponent in this.iDirectionComponentArr)
                _iDirectionComponent.Activate_Func(this.ActivateDone_Func);

            this.anim.Play_Func(this.enterAniData);
        }
        protected override void ActivateDone_Func()
        {
            foreach (IDirectionComponent _iDirectionComponent in this.iDirectionComponentArr)
            {
                if (_iDirectionComponent.IsActivate_Func() == true)
                    return;
            }

            if (this.enterAniData.IsHave == true)
            {
                bool _isPlayEnterAni = this.anim.IsPlaying(this.enterAniData.GetClip.name);
                if (_isPlayEnterAni == true)
                {
                    Debug_C.Log_Func("_isPlayEnterAni");
                    return;
                }
            }

            base.ActivateDone_Func();
        }

        public override void Deactivate_Func(Action _deactivateDoneDel = null)
        {
            if (this.deactivateDelay <= 0f)
            {
                _OnDeactivate_Func();
            }
            else
            {
                Coroutine_C.Invoke_Func(_OnDeactivate_Func, this.deactivateDelay);
            }

            void _OnDeactivate_Func()
            {
                base.Deactivate_Func(_deactivateDoneDel);

                this.anim.Play_Func(this.exitAniData);

                foreach (IDirectionComponent _iDirectionComponent in this.iDirectionComponentArr)
                    _iDirectionComponent.Deactivate_Func(this.CheckDeactivate_Func);
            }
        }
        private void CheckDeactivate_Func()
        {
            foreach (IDirectionComponent _iDirectionComponent in this.iDirectionComponentArr)
            {
                if (_iDirectionComponent.IsActivate_Func() == true)
                    return;
            }

            if (this.exitAniData.IsHave == true)
            {
                bool _isPlayExitAni = this.anim.IsPlaying(this.exitAniData.GetClip.name);
                if (_isPlayExitAni == true)
                {
                    Debug_C.Log_Func("_isPlayExitAni");

                    return;
                }
            }

            this.DeactivateDone_Func();
        }
        public override void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            base.DeactivateDone_Func(_isInit);
        }

        protected void CallAni_EnterDone_Func()
        {
            if (base.isActivate == true)
                return;

            this.anim.Stop(this.enterAniData.GetClip.name);

            this.ActivateDone_Func();
        }
        protected void CallAni_ExitDone_Func()
        {
            if (base.isActivate == false)
                return;

            this.anim.Stop(this.exitAniData.GetClip.name);

            this.CheckDeactivate_Func();
        }

        public interface IDirectionComponent
        {
            void Init_Func();
            void Activate_Func(Action _doneDel);
            bool IsActivate_Func();
            void Deactivate_Func(Action _doneDel);
        }
    } 
}