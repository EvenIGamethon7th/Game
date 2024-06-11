using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Cargold.FrameWork
{
#if Firebase_C
    using Firebase.Analytics;

    public abstract class LogSystem_Manager : MonoBehaviour, GameSystem_Manager.IInitialize
    {
        public virtual string GetParamKey => "Value";

        public static LogSystem_Manager Instance;
        private static Parameter[] defaultParamArr = new Parameter[19];

        private List<Parameter> paramList;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                int _length = this.GetDefaultParamLength_Func();
                defaultParamArr = new Parameter[_length];

                this.paramList = new List<Parameter>();
            }
        }

        // AD
        public void SetAD_Impression_Func(string _adTypeStr)
        {
            string _logKey = this.GetAdLogKey_Func(AdLogType.Impression);

            this.SetLog_Func(_logKey, false, _adTypeStr);
        }
        public void SetAD_Try_Func(string _adTypeStr)
        {
            string _logKey = this.GetAdLogKey_Func(AdLogType.Try);

            this.SetLog_Func(_logKey, false, _adTypeStr);
        }
        public void SetAD_Result_Func(bool _isResult, string _adTypeStr)
        {
            AdLogType _logType = _isResult == true ? AdLogType.Watch : AdLogType.Fail;
            string _logKey = this.GetAdLogKey_Func(_logType);

            this.SetLog_Func(_logKey, false, _adTypeStr);
        }

        // InApp
        public void SetIap_Impression_Func(string _iapTypeStr)
        {
            string _logKey = this.GetIapLogKey_Func(IapLogType.Impression);

            this.SetLog_Func(_logKey, false, _iapTypeStr);
        }
        public void SetIap_Try_Func(string _iapTypeStr)
        {
            string _logKey = this.GetIapLogKey_Func(IapLogType.Try);

            this.SetLog_Func(_logKey, false, _iapTypeStr);
        }
        public void SetInApp_Success_Func(float _price, string _goodsDataKey, int _inAppSuccessCnt)
        {
            Parameter[] _defaultParamArr = this.GetDefaultParamArr_Func();
            Parameter _priceParam = this.GetParam(_price);
            Parameter _goodsDataParam = this.GetParam("", _goodsDataKey);

            // 인앱 결제 성공 로그
            //this.SetLog_Func(GDEItemKeys.LogEvent_IAP_Success, _defaultParamArr, _priceParam, _goodsDataParam);

            //// 인앱 상품 별 로그
            //InAppType _inAppType = InAppSystem_Manager.Instance.GetInAppType_Func(_goodsDataKey);
            //string _goodsLogKey = GetLogKey_Func(_inAppType);
            //this.SetLog_Func(_goodsLogKey, _defaultParamArr, _priceParam);
        }
        /*
        public void SetInApp_Fail_Func(UnityEngine.Purchasing.PurchaseFailureReason _failReason)
        {
            if (_failReason == UnityEngine.Purchasing.PurchaseFailureReason.UserCancelled)
            {
                this.SetLog_Func(GDEItemKeys.LogEvent_IAP_Canceled, false);
            }
            else
            {
                this.SetLog_Func(GDEItemKeys.LogEvent_IAP_Failed, false, Parameter_L.GetParam(_failReason.ToString()));
            }
        }
        public void SetInApp_Success_Func(int _inAppSuccessCnt, float _price, string _goodsDataKey)
        {


            Parameter[] _defaultParamArr = Parameter_L.GetDefaultParamArr_Func();
            Parameter _priceParam = Parameter_L.GetParam(_price);

            // 인앱 결제 성공 로그
            this.SetLog_Func(GDEItemKeys.LogEvent_IAP_Success,
                _defaultParamArr,
                _priceParam,
                Parameter_L.GetParam(GDEItemKeys.LogParameter_Param_IAP_Product, _goodsDataKey));
        }
        */

        // Set Log
        protected void SetLog_Func(string _eventName, bool _isContainDefaultParam, string _paramStr)
        {
            Parameter _param = this.GetParam(_paramStr);
            this.SetLog_Func(_eventName, false, _param);
        }
        protected void SetLog_Func(string _eventName, bool _isContainDefaultParam, Parameter _param1)
        {
            this.paramList.Add(_param1);

            this.SetLog_Func(_eventName, _isContainDefaultParam);
        }
        protected void SetLog_Func(string _eventName, bool _isContainDefaultParam, Parameter _param1, Parameter _param2)
        {
            this.paramList.Add(_param1);
            this.paramList.Add(_param2);

            this.SetLog_Func(_eventName, _isContainDefaultParam);
        }
        protected void SetLog_Func(string _eventName, bool _isContainDefaultParam)
        {
            if (_isContainDefaultParam == true)
            {
                Parameter[] _defaultParamArr = this.GetDefaultParamArr_Func();
                paramList.Add_Func(_defaultParamArr);
            }

            this.SetLog_Func(_eventName);
        }

        protected void SetLog_Func(string _eventName, Parameter[] _defaultParamArr, Parameter _param1)
        {
            this.paramList.Add(_param1);

            this.SetLog_Func(_eventName, _defaultParamArr);
        }
        protected void SetLog_Func(string _eventName, Parameter[] _defaultParamArr, Parameter _param1, Parameter _param2)
        {
            this.paramList.Add(_param1);
            this.paramList.Add(_param2);

            this.SetLog_Func(_eventName, _defaultParamArr);
        }
        protected void SetLog_Func(string _eventName, Parameter[] _defaultParamArr)
        {
            this.paramList.Add_Func(_defaultParamArr);

            this.SetLog_Func(_eventName);
        }

        protected void SetLog_Func(string _eventName)
        {
            if (string.IsNullOrEmpty(_eventName) == false)
            {
                Debug_C.Log_Func("(FB)Event Name : " + _eventName + " / Param Cnt : " + this.paramList.Count);

#if UNITY_EDITOR
                return;
#endif

                if (0 < this.paramList.Count)
                {
                    Parameter[] _tempParamArr = this.paramList.ToArray();
                    this.paramList.Clear();

                    FirebaseSystem_Manager.Instance.SetLog_Func(_eventName, _tempParamArr);
                }
                else
                {
                    FirebaseSystem_Manager.Instance.SetLog_Func(_eventName);
                }
            }
            else
            {
                Debug.LogError("(FB) Log Event Key가 비어있습니다.");
            }
        }

        protected abstract int GetDefaultParamLength_Func();
        protected abstract Parameter[] GetDefaultParamArr_Func();
        protected abstract string GetAdLogKey_Func(AdLogType _adLogType);
        protected abstract string GetIapLogKey_Func(IapLogType _iapLogType);

        protected Parameter GetParam(string _value)
        {
            return new Parameter(GetParamKey, _value);
        }
        protected Parameter GetParam(long _value)
        {
            return new Parameter(GetParamKey, _value);
        }
        protected Parameter GetParam(double _value)
        {
            return new Parameter(GetParamKey, _value);
        }

        protected Parameter GetParam(string _key, string _value)
        {
            return new Parameter(_key, _value);
        }
        protected Parameter GetParam(string _key, long _value)
        {
            return new Parameter(_key, _value);
        }
        protected Parameter GetParam(string _key, double _value)
        {
            return new Parameter(_key, _value);
        }

        protected enum AdLogType
        {
            None = 0,

            Impression,
            Try,
            Watch,
            Fail,
        }

        protected enum IapLogType
        {
            None = 0,

            Impression,
            Try,
            Purchase,
            Cancel,
            Fail,
        }
    }
#endif
}