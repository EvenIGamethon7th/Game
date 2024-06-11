using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold.SDK.Ads
{
#if Ads_Unity_C
    using UnityEngine.Advertisements;

    public abstract class AdsSystem_UnityAds : Cargold.SDK.Ads.AdsSystem_Manager, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [ReadOnly, ShowInInspector, LabelText("Game ID")] private string currentGameID;
        [ReadOnly, ShowInInspector, LabelText("Ad Unit ID - Rewarded")] private string currentAdUnitID_Reward;
        [ReadOnly, ShowInInspector, LabelText("Ad Unit ID - Interstitial")] private string currentAdUnitID_Interstitial;

        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if (_layer == 0)
            {
#if UNITY_IOS
                _CallIOS_Func();
#elif UNITY_ANDROID
                _CallAOS_Func();
#endif

                Advertisement.Initialize(this.currentGameID, base.isOfficialTestMode, this);

                void _CallIOS_Func()
                {
                    LibraryRemocon.AssetLibraryData.AdvertiseData.UnityAdsData _unityAdsData =
                        LibraryRemocon.Instance.assetLibraryData.advertiseData.unityAdsData;

                    this.currentGameID = _unityAdsData.iosGameID;
                    this.currentAdUnitID_Reward = _unityAdsData.iosUnitID_Reward;
                    this.currentAdUnitID_Interstitial = _unityAdsData.iosUnitID_Interstitial;
                }

                void _CallAOS_Func()
                {
                    LibraryRemocon.AssetLibraryData.AdvertiseData.UnityAdsData _unityAdsData =
                        LibraryRemocon.Instance.assetLibraryData.advertiseData.unityAdsData;

                    this.currentGameID = _unityAdsData.aosGameID;
                    this.currentAdUnitID_Reward = _unityAdsData.aosUnitID_Reward;
                    this.currentAdUnitID_Interstitial = _unityAdsData.aosUnitID_Interstitial;
                }
            }
        }

        protected override void OnAdvertisementLoad_Func()
        {
            Advertisement.Load(this.currentAdUnitID_Interstitial, this);
            Advertisement.Load(this.currentAdUnitID_Reward, this);
        }

        protected override void OnAdsProcess_Official_Func(AdType _adType, bool _isTestMode)
        {
            string _key = null;
            if (_adType == AdType.Rewarded)
                _key = this.currentAdUnitID_Reward;
            else if (_adType == AdType.Interstitial)
                _key = this.currentAdUnitID_Interstitial;
            else
                Debug_C.Error_Func("_adType : " + _adType);

            Advertisement.Show(_key, this);
        }

        void IUnityAdsInitializationListener.OnInitializationComplete()
        {
            Advertisement.Load(this.currentAdUnitID_Interstitial, this);
            Advertisement.Load(this.currentAdUnitID_Reward, this);

            base.OnInitializeDone_Func(true);
        }
        void IUnityAdsInitializationListener.OnInitializationFailed(UnityAdsInitializationError _error, string _msg)
        {
            _msg = StringBuilder_C.Append_Func(" / error : ", _error.ToString_Func(), " / msg : ", _msg);
            base.OnInitializeDone_Func(false, _msg);
        }

        void IUnityAdsLoadListener.OnUnityAdsAdLoaded(string _placementId)
        {
            // If the ad successfully loads, add a listener to the button and enable it:

            string _msg = StringBuilder_C.Append_Func("placementId : ", _placementId);
            base.OnLoadDone_Func(true, _msg);
        }
        void IUnityAdsLoadListener.OnUnityAdsFailedToLoad(string _placementId, UnityAdsLoadError _error, string _msg)
        {
            // Implement Load and Show Listener error callbacks:

            _msg = StringBuilder_C.Append_Func("placementId : ", _placementId, " / error : ", _error.ToString_Func(), " / msg : ", _msg);
            base.OnLoadDone_Func(false, _msg);

            // Use the error details to determine whether to try to load another ad.
        }

        void IUnityAdsShowListener.OnUnityAdsShowClick(string _placementId)
        {
            Debug.Log(StringBuilder_C.Append_Func(AdsSystem_Manager.LogStr, " OnUnityAdsShowClick) ID : ", _placementId));
        }
        void IUnityAdsShowListener.OnUnityAdsShowComplete(string _placementId, UnityAdsShowCompletionState _showCompletionState)
        {
            // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:   

            string _msg = StringBuilder_C.Append_Func("placementId : ", _placementId, " / State : ", _showCompletionState.ToString_Func());
            base.OnAdsDone_Func(true, _msg);
        }
        void IUnityAdsShowListener.OnUnityAdsShowFailure(string _placementId, UnityAdsShowError _error, string _msg)
        {
            // Use the error details to determine whether to try to load another ad.

            _msg = StringBuilder_C.Append_Func("placementId : ", _placementId, " / _error : ", _error.ToString_Func(), " / Msg : ", _msg);
            base.OnAdsDone_Func(false, _msg);
        }
        void IUnityAdsShowListener.OnUnityAdsShowStart(string _placementId)
        {
            Debug.Log(StringBuilder_C.Append_Func(AdsSystem_Manager.LogStr, " OnUnityAdsShowStart) ID : ", _placementId));
        }
    }
#endif
}