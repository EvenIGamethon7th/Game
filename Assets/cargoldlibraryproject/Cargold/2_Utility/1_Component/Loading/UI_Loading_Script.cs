using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using static Cargold.Loading.LoadingSystem_Manager;

namespace Cargold.Loading
{
    public abstract class UI_Loading_Script : SerializedMonoBehaviour
    {
        [SerializeField] protected GameObject groupObj;
        [SerializeField] protected Animation anim = null;
        [ShowInInspector, ReadOnly] protected Action activateDoneDel;
        [ShowInInspector, ReadOnly] protected Action deactivateDoneDel;
        [ShowInInspector, ReadOnly] protected bool isActivate;
        [SerializeField, LabelText("자동 히어라키 순서 마지막 조정 여분")] private bool isLastSibling = true;

        public abstract LoadingType GetLoadingType { get; }
        public bool IsActivate => this.isActivate;

        public virtual void Init_Func()
        {
            if(this.isLastSibling == true)
                this.transform.SetAsLastSibling();

            this.DeactivateDone_Func(true);
        }

        public virtual void Activate_Func(Action _activateDoneDel = null)
        {
            this.groupObj.SetActive(true);

            this.activateDoneDel = _activateDoneDel;
        }
        protected virtual void ActivateDone_Func()
        {
            this.isActivate = true;

            if (this.activateDoneDel != null)
                this.activateDoneDel.Invoke();

            this.activateDoneDel = null;
        }

        public virtual void Deactivate_Func(Action _deactivateDoneDel = null)
        {
            this.deactivateDoneDel = _deactivateDoneDel;
        }

        public virtual void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                if (this.deactivateDoneDel != null)
                    this.deactivateDoneDel();
            }

            this.isActivate = false;
            this.activateDoneDel = null;
            this.deactivateDoneDel = null;

            this.groupObj.SetActive(false);
        }
    }
}