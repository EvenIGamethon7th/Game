using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.Loading
{
    public abstract class LoadingSystem_Manager : SerializedMonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public static LoadingSystem_Manager Instance;

        [SerializeField] protected Dictionary<LoadingType, UI_Loading_Script> loadingClassDic;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                foreach (var item in this.loadingClassDic)
                    item.Value.Init_Func();

                this.Deactivate_Func(true);
            }
        }

        public void Activate_Func()
        {

        }

        public void OnLoading_Func(LoadingType _loadingType, Action _activateDoneDel = null)
        {
            UI_Loading_Script _loadingClass = this.loadingClassDic.GetValue_Func(_loadingType);
            if (_loadingClass.IsActivate == false)
                _loadingClass.Activate_Func(_activateDoneDel);
        }
        public void OffLoading_Func(LoadingType _loadingType, Action _deactivateDoneDel = null)
        {
            UI_Loading_Script _loadingClass = this.loadingClassDic.GetValue_Func(_loadingType);
            if (_loadingClass.IsActivate == true)
                _loadingClass.Deactivate_Func(_deactivateDoneDel);
        }

        public void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                foreach (var item in this.loadingClassDic)
                    item.Value.DeactivateDone_Func();
            }
        }

        public enum LoadingType
        {
            None = 0,

            Transition = 1,
            Trobber = 2,
        }
    } 
}