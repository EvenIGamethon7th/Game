using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.SDK.TrackingLog
{
    public abstract class LogSystem_Manager : MonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public const string LogStr = "Cargold TrackingLog) ";

        public static LogSystem_Manager Instance;

        protected List<ParamData> paramDataList;
        protected bool isInitialize;

        public bool IsInitialize => this.isInitialize;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                this.paramDataList = new List<ParamData>();
            }
        }

        // AD
        public void OnLog_AD_Impression_Func(string _adTypeStr)
        {
            string _logKey = this.GetLogKey_AD_Func(AdLogType.Impression);

            this.SetLog_Func(_logKey, false, new ParamData("AdType", _adTypeStr));
        }
        public void OnLog_AD_Try_Func(string _adTypeStr)
        {
            string _logKey = this.GetLogKey_AD_Func(AdLogType.Try);

            this.SetLog_Func(_logKey, false, new ParamData("AdType", _adTypeStr));
        }
        public void OnLog_AD_Result_Func(bool _isResult, string _adTypeStr)
        {
            AdLogType _logType = _isResult == true ? AdLogType.WatchDone : AdLogType.Fail;
            string _logKey = this.GetLogKey_AD_Func(_logType);

            this.SetLog_Func(_logKey, false, new ParamData("AdType", _adTypeStr));
        }

        // InApp
        public void OnLog_InApp_Impression_Func(string _iapTypeStr)
        {
            string _logKey = this.GetLogKey_InApp_Func(InappLogType.Impression);

            this.SetLog_Func(_logKey, false, new ParamData("InappType", _iapTypeStr));
        }
        public void OnLog_InApp_Try_Func(string _iapTypeStr)
        {
            string _logKey = this.GetLogKey_InApp_Func(InappLogType.Try);

            this.SetLog_Func(_logKey, false, new ParamData("InappType", _iapTypeStr));
        }
        public void OnLog_InApp_Success_Func(float _price, string _goodsDataKey, int _inAppSuccessCnt)
        {
            string _logKey = this.GetLogKey_InApp_Func(InappLogType.Purchase);
            this.SetLog_Func(_logKey, false,
                new ParamData("GoodsDataKey", _goodsDataKey),
                new ParamData("InappSuccessCnt", _inAppSuccessCnt.ToString())
                );
        }

        public void OnLog_Func(string _eventName, string _key, object _value)
        {
            this.OnLog_Func(_eventName, new ParamData(_key, _value));
        }
        public abstract void OnLog_Func(string _eventName, params ParamData[] _paramDataArr);

        protected abstract string GetLogKey_AD_Func(AdLogType _adLogType);
        protected abstract string GetLogKey_InApp_Func(InappLogType _iapLogType);

        protected abstract void SetLog_Func(string _eventName, bool _isContainDefaultParam, params ParamData[] _paramDataArr);

        protected void OnInitializeDone_Func(bool _isSuccess, string _msg = null)
        {
            this.isInitialize = _isSuccess;

            string _str = StringBuilder_C.Append_Func(LogStr, "Init Result : ", _isSuccess.ToString());

            if (_msg.IsNullOrWhiteSpace_Func() == false)
                _str = StringBuilder_C.Append_Func(_str, " / (Msg) ", _msg);

            Debug.Log(_str);
        }
        protected void OnLogDone_Func(string _eventName)
        {
            string _str = StringBuilder_C.Append_Func(LogStr, "Event Name : ", _eventName);
            Debug.Log(_str);
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        } 
#endif

        protected enum AdLogType
        {
            None = 0,

            Impression,
            Try,
            WatchDone,
            Fail,
        }

        protected enum InappLogType
        {
            None = 0,

            Impression,
            Try,
            Purchase,
            Cancel,
            Fail,
        }

        public struct ParamData
        {
            public string parameter;
            public object value;

            public ParamData(string _parameter, object _value)
            {
                this.parameter = _parameter;
                this.value = _value;
            }
        }
    }
}