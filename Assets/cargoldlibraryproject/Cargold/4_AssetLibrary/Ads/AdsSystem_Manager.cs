using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.SDK.Ads
{
    public abstract class AdsSystem_Manager : MonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public const string LogStr = "Cargold Ads) ";

        public static AdsSystem_Manager Instance;

        protected Action<bool> callbackDel;
        [SerializeField, LabelText("공식 테스트 모드"), InfoBox(
            "<True - 추천!>" +
            "\nEditor : SDK가 지원하는 공식 테스트 모드로 광고 호출." +
            "\nBuild : SDK 정식 광고 호출" +
            "\n" +
            "<False - 비추!>" +
            "\n Editor : 1초 딜레이 후 강제 광고 성공 콜백" +
            "\n Build : SDK 정식 광고 호출")]
        protected bool isOfficialTestMode = true;
        protected bool isInitialize;

        private Cargold.Observer.Observer_Action<AdType, bool> showResultObs;
        private AdType adType;

        public bool IsInitialize => this.isInitialize;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

#if !UNITY_EDITOR
                _CallOfficial_Func();
#endif

                this.Deactivate_Func(true);
            }

            void _CallOfficial_Func()
            {
                this.isOfficialTestMode = false;
            }
        }

        protected void OnInitializeDone_Func(bool _isSuccess, string _msg = null)
        {
            string _str = StringBuilder_C.Append_Func(LogStr, "Init Result : ", _isSuccess.ToString());

            if(_msg.IsNullOrWhiteSpace_Func() == false)
                _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

            this.isInitialize = _isSuccess;

            Debug.Log(_str);
        }
        protected void OnLoadDone_Func(bool _isSuccess, string _msg)
        {
            string _str = StringBuilder_C.Append_Func(LogStr, "Load Result : ", _isSuccess.ToString());

            if (_msg.IsNullOrWhiteSpace_Func() == false)
                _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

            Debug.Log(_str);
        }

        public virtual void Activate_Func()
        {

        }
        protected abstract void OnAdvertisementLoad_Func();

        public void OnAds_Func(AdType _adType = AdType.Rewarded)
        {
            this.OnAds_Func(null, _adType);
        }
        public void OnAds_Func(Action<bool> _callbackDel, AdType _adType = AdType.Rewarded, bool _isForced = false, bool _isRemoveToast = true, Action _removeToastDoneDel = null)
        {
            this.OnAds_Func(_callbackDel, out _, _adType, _isForced, _isRemoveToast, _removeToastDoneDel);
        }
        public void OnAds_Func(Action<bool> _callbackDel, out bool _isRemovedAds, AdType _adType = AdType.Rewarded, bool _isForced = false, bool _isRemoveToast = true, Action _removeToastDoneDel = null)
        {
            if (this.IsAdsRemove_Func() == false || _isForced == true)
            {
                _isRemovedAds = false;

                if(Loading.LoadingSystem_Manager.Instance != null)
                    Loading.LoadingSystem_Manager.Instance.OnLoading_Func(Loading.LoadingSystem_Manager.LoadingType.Trobber);

                this.callbackDel = _callbackDel;
                this.adType = _adType;

                this.OnAdsProcess_Func(_adType);
            }
            else
            {
                _isRemovedAds = true;

                _callbackDel?.Invoke(true);

                if(_isRemoveToast == true && Cargold.UI.UI_Toast_Manager.Instance != null)
                {
                    Cargold.UI.UI_Toast_Manager.Instance.Activate_LibraryToast_Func(Cargold.UI.ToastKey.AdsRemove, _removeToastDoneDel);
                }
                else
                {
                    if (_removeToastDoneDel != null)
                        _removeToastDoneDel();
                }

                this.showResultObs.Notify_Func(_adType, true);
            }
        }
        protected void OnAdsProcess_Func(AdType _adType = AdType.Rewarded)
        {
#if UNITY_EDITOR
            _CallEditor_Func();
#else
            _CallOfficial_Func();
#endif

            void _CallEditor_Func()
            {
                if (this.isOfficialTestMode == false)
                {
                    Coroutine_C.Invoke_Func(() =>
                    {
                        this.OnAdsDone_Func(true);
                    }, 1f);
                }
                else
                {
                    _CallOfficial_Func();
                }
            }
            void _CallOfficial_Func()
            {
                this.OnAdsProcess_Official_Func(_adType, this.isOfficialTestMode);
            }
        }
        protected abstract void OnAdsProcess_Official_Func(AdType _adType, bool _isTestMode);
        protected virtual void OnAdsDone_Func(bool _isSuccess, string _msg = null, bool _isTestCall = false)
        {
            if(_isTestCall == false && _msg.IsNullOrWhiteSpace_Func() == false)
            {
                string _str = StringBuilder_C.Append_Func(LogStr, "Watch Result : ", _isSuccess.ToString());

                if (_msg.IsNullOrWhiteSpace_Func() == false)
                    _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

                if(_isSuccess == false && Cargold.UI.UI_Toast_Manager.Instance != null)
                    Cargold.UI.UI_Toast_Manager.Instance.Activate_LibraryToast_Func(Cargold.UI.ToastKey.AdsFail);

                Debug.Log(_str);
            }

            if(Loading.LoadingSystem_Manager.Instance != null)
                Loading.LoadingSystem_Manager.Instance.OffLoading_Func(Loading.LoadingSystem_Manager.LoadingType.Trobber);

            if (this.callbackDel != null)
                this.callbackDel(_isSuccess);
            else
                Debug_C.Warning_Func("광고 성공 콜백이 없는데?");

            if(this.showResultObs != null)
                this.showResultObs.Notify_Func(this.adType, _isSuccess);
            this.adType = AdType.None;

            this.OnAdvertisementLoad_Func();
        }

        public abstract bool IsAdsRemove_Func();

        public void Subscribe_OnShowDone_Func(Action<AdType, bool> _del)
        {
            if(this.showResultObs == null)
                this.showResultObs = new Observer.Observer_Action<AdType, bool>();

            this.showResultObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_OnShowDone_Func(Action<AdType, bool> _del)
        {
            this.showResultObs.Unsubscribe_Func(_del);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        }

#if UNITY_EDITOR
        [Button("테스트 호출")]
        private void CallEdit_Test_Func(AdType _adType = AdType.Rewarded)
        {
            this.OnAds_Func(null, _adType);
        }

        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }
#endif
    }

    public enum AdType
    {
        None = 0,

        Rewarded,
        Interstitial,
    }
}