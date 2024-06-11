using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    /// <summary>
    /// 출시 시 고려해야 하는 기능들이 모여있는 전역 클래스입니다.
    /// </summary>
    public static class Release_C
    {
        private const string StoreUrlAndroid = "https://play.google.com/store/apps/details?id={0}";
        private const string StoreUrlIos = "https://apps.apple.com/app/{0}";

        public static void OpenURL_Store_Func(string _identifierStr = null, PlatformType _platformType = PlatformType.None)
        {
            string _urlStr = null;

            if(_platformType == PlatformType.None)
            {
    #if UNITY_ANDROID
                _CallAos_Func();
    #elif UNITY_IOS
                _CallIos_Func();
    #endif
            }

            if (_platformType == PlatformType.Android)
            {
                if (_identifierStr.IsNullOrWhiteSpace_Func() == true)
                    _identifierStr = Application.identifier;

                _urlStr = string.Format(StoreUrlAndroid, _identifierStr);
            }
            else if(_platformType == PlatformType.IOS)
            {
                // 모름!

                //UnityEngine.iOS.Device.RequestStoreReview();

                _urlStr = string.Format(StoreUrlIos, _identifierStr);
            }
            else
            {
                Debug_C.Error_Func("_platformType : " + _platformType);
            }

            if(_urlStr.IsNullOrWhiteSpace_Func() == false)
                Application.OpenURL(_urlStr);

            void _CallAos_Func()
            {
                _platformType = PlatformType.Android;
            }

            void _CallIos_Func()
            {
                _platformType = PlatformType.IOS;
            }
        }
        public static void OpenMail_Func(string _receiveEmailStr)
        {
            OpenMail_Func(_receiveEmailStr, null);
        }
        public static void OpenMail_Func(string _receiveEmailStr, string _subject)
        {
            string _mailto = _receiveEmailStr;

            if(_subject.IsNullOrWhiteSpace_Func() == true)
                _subject = Release_C.EscapeURL($"{Application.productName}, Feedback.");

            string _body = Release_C.EscapeURL(
                "--------------------------------\n\n\n\n\n\n" +
                "--------------------------------\n" +
                $"Device Model : {SystemInfo.deviceModel}\n" +
                $"Device OS : {SystemInfo.operatingSystem}\n" +
                $"Bundle Version : {Application.version}\n" +
                "--------------------------------\n"
                );

            Application.OpenURL($"mailto:{_mailto}?subject={_subject}&body={_body}");
        }

        private static string EscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }
    }
}