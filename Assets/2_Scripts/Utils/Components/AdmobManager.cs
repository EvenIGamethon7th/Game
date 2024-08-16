using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class AdmobManager : Singleton<AdmobManager>
    {
        private RewardedAd _rewardedAd;

#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-8904224703245079/6364492829";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private string _adUnitId = "unused";
#endif

        public void Start()
        {
            MobileAds.Initialize(status =>
            {
                LoadRewardedAd();
            });
        }

        private void LoadRewardedAd()
        {
            AdRequest request = new AdRequest();
            RewardedAd.Load(_adUnitId, request, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load: " + error);
                    return;
                }

                _rewardedAd = ad;
                RegisterAdEvents();
                Debug.Log("Rewarded ad loaded.");
            });
        }

        async UniTaskVoid WaitTask(Action callback )
        {
            await UniTask.WaitForSeconds(0.2f);
            callback?.Invoke();
        } 
        
        public void ShowRewardedAd(Action rewardCallback)
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show(reward =>
                {
                    Debug.Log($"User earned reward of {reward.Amount} {reward.Type}");
                    // 여기서 유저에게 보상을 지급하는 로직을 추가하세요.
                     WaitTask(() => rewardCallback?.Invoke()).Forget();
                });
            }
            else
            {
                Debug.Log("Rewarded ad is not ready yet.");
            }
        }

        private void RegisterAdEvents()
        {
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed.");
                WaitTask(LoadRewardedAd).Forget();
            };

            _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to show: " + error);
                WaitTask(LoadRewardedAd).Forget();
            };

            _rewardedAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad opened.");
            };

            _rewardedAd.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad clicked.");
            };

            _rewardedAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad impression recorded.");
            };

            _rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Rewarded ad paid: {adValue.Value} {adValue.CurrencyCode}");
            };
        }
    }
}
