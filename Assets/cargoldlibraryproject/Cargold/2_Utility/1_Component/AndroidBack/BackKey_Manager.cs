using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using System.Linq;
using Cargold.FrameWork;

namespace Cargold.BackKeySystem
{
    public abstract class BackKey_Manager : MonoBehaviour, GameSystem_Manager.IInitializer
    {
        public static BackKey_Manager Instance;

        [ShowInInspector, ReadOnly] private Dictionary<string, IBackKey> iBackKeySubscriberDic;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                this.iBackKeySubscriberDic = new Dictionary<string, IBackKey>();
            }
        }

        public bool TryGetLastSubscriber_Func(out string _backKey, out IBackKey _iBackKey)
        {
            if(this.iBackKeySubscriberDic.IsHave_Func() == true)
            {
                IEnumerable<KeyValuePair<string, IBackKey>> _enumerable = this.iBackKeySubscriberDic.TakeLast(1);

                foreach (KeyValuePair<string, IBackKey> item in _enumerable)
                {
                    _backKey = item.Key;
                    _iBackKey = item.Value;

                    return true;
                }
            }

            _backKey = null;
            _iBackKey = null;

            return false;
        }

#if UNITY_ANDROID
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) == true)
            {
                this.OnBack_Func();
            }
        }

        public virtual void OnBack_Func()
        {
            if (this.iBackKeySubscriberDic.IsHave_Func() == true)
            {
                if (this.TryGetLastSubscriber_Func(out string _backKey, out BackKey_Manager.IBackKey _iAndroidBack) == true)
                {
                    _iAndroidBack.OnBackByBackSystem_Func(_backKey);
                }
                else
                {
                    Debug_C.Error_Func("?");
                }
            }
            else
            {
                this.OnExitApp_Func();
            }
        }
#endif

        protected abstract void OnExitApp_Func();
        public void OnClearSubscriber_Func()
        {
            this.iBackKeySubscriberDic.Clear();
        }

        public void Subscribe_Func(IBackKey _iBackKey, string _key)
        {
            this.iBackKeySubscriberDic.Add_Func(_key, _iBackKey);
        }
        public void Unsubscribe_Func(IBackKey _iBackKey, string _key)
        {
            if(this.iBackKeySubscriberDic.TryTake_Func(_key, out var _originI) == true)
            {
#if UNITY_EDITOR
                if (_originI != _iBackKey)
                    Debug_C.Error_Func("_key : " + _key); 
#endif
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;

            if (this.transform.parent.TryGetComponent(out Cargold.FrameWork.GameSystem_Manager _gsm) == true)
                _gsm.CallEdit_AddInitializer_Func(this);
        } 
#endif

        public interface IBackKey
        {
            public void ActivateByBackSystem_Func();
            public void OnBackByBackSystem_Func(string _key);
        }
    }
}