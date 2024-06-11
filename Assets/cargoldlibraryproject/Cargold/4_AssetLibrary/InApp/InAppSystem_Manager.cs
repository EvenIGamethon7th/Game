using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.SDK.Purchase
{
    public abstract class InAppSystem_Manager : MonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public const string LogStr = "Cargold InApp) ";

        public static InAppSystem_Manager Instance;

        protected string inappKey;
        protected Action<PurchaseResult> callbackDel;
        protected bool isInitialize;

        public bool IsInitialize => this.isInitialize;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                this.Deactivate_Func(true);
            }
        }

        protected void OnInitializeDone_Func(bool _isSuccess, string _msg = null)
        {
            this.isInitialize = _isSuccess;

            string _str = StringBuilder_C.Append_Func(LogStr, "Init Result : ", _isSuccess.ToString());

            if (_msg.IsNullOrWhiteSpace_Func() == false)
                _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

            Debug.Log(_str);
        }

        public virtual void Activate_Func()
        {

        }

        public abstract void OnRestore_Func(Action<bool> _callbackDel);
        protected void OnRestoreDone_Func(bool _isResult, string _msg = null)
        {
            _msg = StringBuilder_C.Append_Func(LogStr, "Result : ", _isResult.ToString(), _msg);
            Debug.Log(_msg);
        }
        public void OnPurchase_Func(string _dataKey, Action<PurchaseResult> _callbackDel, bool _isTestCall = false)
        {
            if (Cargold.FrameWork.DataBase_Manager.Instance.GetInapp_C.TryGetData_Func(_dataKey, out IInappData _iInappData) == true)
            {
                this.OnPurchase_Func(_iInappData, _callbackDel, _isTestCall);
            }
            else
            {
                Debug_C.Log_Func("다음 key의 인앱 데이터는 없음 : " + _dataKey);
            }
        }
        public virtual void OnPurchase_Func(IInappData _iInappData, Action<PurchaseResult> _callbackDel, bool _isTestCall = false)
        {
            if(Loading.LoadingSystem_Manager.Instance != null)
                Loading.LoadingSystem_Manager.Instance.OnLoading_Func(Loading.LoadingSystem_Manager.LoadingType.Trobber);

            this.OnPurchaseProcess_Func(_iInappData, _callbackDel, _isTestCall);
        }
        protected void OnPurchaseProcess_Func(IInappData _iInappData, Action<PurchaseResult> _callbackDel, bool _isTestCall = false)
        {
            this.inappKey = _iInappData.GetKey;
            this.callbackDel = _callbackDel;

#if UNITY_EDITOR
            _OnSuccess_Func();
#else
            _CallOfficial_Func();
#endif

            void _OnSuccess_Func()
            {
                Coroutine_C.Invoke_Func(() =>
                {
                    if(Loading.LoadingSystem_Manager.Instance != null)
                        Loading.LoadingSystem_Manager.Instance.OffLoading_Func(Loading.LoadingSystem_Manager.LoadingType.Trobber);

                    this.OnPurchaseDone_Func(_iInappData.GetKey, PurchaseResult.Succcess, null, true);
                }, 1f);
            }
            void _CallOfficial_Func()
            {
                if (_isTestCall == false)
                    this.OnPurchaseProcess_Official_Func(_iInappData);
                else
                    _OnSuccess_Func();
            }
        }
        protected abstract void OnPurchaseProcess_Official_Func(IInappData _iInappData);
        protected virtual void OnPurchaseDone_Func(string _inappKey, PurchaseResult _purchaseResult, string _msg = null, bool _isTestCall = false)
        {
            this.inappKey = null;

            if (_isTestCall == false && _msg.IsNullOrWhiteSpace_Func() == false)
            {
                string _str = StringBuilder_C.Append_Func(LogStr, "Purchase Result : ", _purchaseResult.ToString(), " / Test Call : " , _isTestCall.ToString());

                if (_msg.IsNullOrWhiteSpace_Func() == false)
                    _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

                if (_purchaseResult != PurchaseResult.Succcess)
                    Cargold.UI.UI_Toast_Manager.Instance.Activate_LibraryToast_Func(Cargold.UI.ToastKey.PurchaseFail);

                Debug.Log(_str);
            }

            if(Loading.LoadingSystem_Manager.Instance != null)
                Loading.LoadingSystem_Manager.Instance.OffLoading_Func(Loading.LoadingSystem_Manager.LoadingType.Trobber);

            if (this.callbackDel != null)
                this.callbackDel(_purchaseResult);
            else
                Debug_C.Warning_Func("인앱 성공 콜백이 없는데?");
        }

        public string GetLocalizePriceStr_Func(IInappData _iData)
        {
            return this.GetLczPriceStr_Func(_iData.GetKey);
        }
        [Button("가격표(Str)")]
        public string GetLocalizePriceStr_Func(string _dataKey)
        {
#if UNITY_EDITOR
            if (Cargold.FrameWork.DataBase_Manager.Instance.GetInapp_C.TryGetData_Func(_dataKey, out IInappData _iData) == false)
                Debug_C.Error_Func("다음 Key는 인앱 테이블에 존재하지 않습니다. : " + _dataKey); 
#endif

            return this.GetLczPriceStr_Func(_dataKey);
        }
        protected abstract string GetLczPriceStr_Func(string _dataKey);

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        } 
    }

    public enum PurchaseResult
    {
        None = 0,

        Succcess = 10,

        /// <summary>
        /// (구매 불가) 시스템 구매 기능을 사용할 수 없습니다.
        /// </summary>
        Fail_PurchasingUnavailable = 100,

        /// <summary>
        /// (기존 구매 보류) 새 구매를 요청했을 때 이미 구매가 진행 중이었습니다.
        /// </summary>
        Fail_ExistingPurchasePending = 110,

        /// <summary>
        /// (제품 사용 불가) 스토어에서 제품을 구매할 수 없습니다.
        /// </summary>
        Fail_ProductUnavailable = 120,

        /// <summary>
        /// (서명 유효하지 않음) 구매 영수증의 서명 유효성 검사에 실패했습니다.
        /// </summary>
        Fail_SignatureInvalid = 130,

        /// <summary>
        /// (사용자 취소) 사용자가 구매를 진행하지 않고 취소를 선택했습니다.
        /// </summary>
        Fail_UserCancelled = 140,

        /// <summary>
        /// (결제 거절) 결제에 문제가 발생했습니다.
        /// </summary>
        Fail_PaymentDeclined = 150,

        /// <summary>
        /// (중복 결제) 거래가 이미 성공적으로 완료된 경우 중복 거래 오류가 발생합니다.
        /// </summary>
        Fail_DuplicateTransaction = 160,

        /// <summary>
        /// (알 수 없음) 인식할 수 없는 구매 문제에 대한 포괄적인 설명입니다.
        /// </summary>
        Fail_Unknown = 170,
    }
}