using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.SDK.TrackingLog
{
#if Log_Unity_C
    using Unity.Services.Analytics;
    using Unity.Services.Core;

    public abstract class LogSystem_UnityGamingService : LogSystem_Manager
    {
        private Dictionary<string, object> paramDic;
        [ShowInInspector, ReadOnly, LabelText("UGS UserID")] private string userID = "확인 불가";

        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            this.userID = null;

            if (_layer == 0)
            {
                this.paramDic = new Dictionary<string, object>();
                
                _Init_Func();

                async void _Init_Func()
                {
                    try
                    {
                        await UnityServices.InitializeAsync();
                        List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents(); // 앱솔리트인데 이게 있어야 초기화가 정상적으로 됨 ㅡㅡ

                        string _userID = this.GetUserID_Func();
                        base.OnInitializeDone_Func(true, "_userID : " + _userID);

                        this.userID = _userID;
                    }
                    catch (ConsentCheckException _e)
                    {
                        // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.

                        Debug.Log(LogSystem_Manager.LogStr + "Exception : " + _e.Message + " / Reason : " + _e.Reason.ToString_Func());
                    }
                }
            }
        }

        public override void OnLog_Func(string _eventName, params ParamData[] _paramDataArr)
        {
            // Send custom event
            this.paramDic.Clear();

            foreach (ParamData _paramData in _paramDataArr)
                this.paramDic.Add(_paramData.parameter, _paramData.value);

            // The ‘myEvent’ event will get queued up and sent every minute
            AnalyticsService.Instance.CustomData(_eventName, this.paramDic);

            // Optional - You can call Events.Flush() to send the event immediately
            AnalyticsService.Instance.Flush();

            base.OnLogDone_Func(_eventName);
        }
        protected override string GetLogKey_AD_Func(AdLogType _adLogType)
        {
            return _adLogType.ToString_Func();
        }
        protected override string GetLogKey_InApp_Func(InappLogType _iapLogType)
        {
            return _iapLogType.ToString_Func();
        }
        protected override void SetLog_Func(string _eventName, bool _isContainDefaultParam, params ParamData[] _paramDataArr)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserID_Func()
        {
            if (this.userID.IsNullOrWhiteSpace_Func() == true)
                this.userID = AnalyticsService.Instance.GetAnalyticsUserID();

            return this.userID; 
        }
    } 
#endif
}