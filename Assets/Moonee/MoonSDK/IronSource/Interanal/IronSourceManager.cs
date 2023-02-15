using UnityEngine;
using Firebase.Analytics;
using System;
using com.adjust.sdk;
using Moonee.MoonSDK.Internal.Analytics;

namespace Moonee.MoonSDK.Internal.Advertisement
{
    public class IronSourceManager : AdvertisementManager
    {
        private void Start()
        {
            if (settings.IronSource == false)
            {
                Destroy(this);
                return;
            }
#if UNITY_ANDROID
            string appKey = settings.ironSourceAndroidAppKey;
            if (string.IsNullOrEmpty(appKey)) Destroy(this);
#elif UNITY_IPHONE
        string appKey = settings.ironSourceIOSAppKey;
#else
        string appKey = "unexpected_platform";
#endif
           
            IronSource.Agent.setUserId(GenerateUserID());
            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(appKey);

            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

            LoadInterstitial();

            OnShowRewardedVideoEventHandler += ShowRewardedAd;
            OnShowInterstitialEventHandler += ShowInterstitial;
            OnShowBannerEventHandler += ShowBanner;
            OnHideBannerEventHandler += HideBanner;
        }
        private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData == null) return;

            if (settings.Firebase)
            {
                FirebaseAnalytics.LogEvent(
                    FirebaseAnalytics.EventAdImpression,
                    new(FirebaseAnalytics.ParameterAdPlatform, "ironSource"),
                    new(FirebaseAnalytics.ParameterAdSource, impressionData.adNetwork),
                    new(FirebaseAnalytics.ParameterAdFormat, impressionData.adUnit),
                    new(FirebaseAnalytics.ParameterAdUnitName, impressionData.instanceName),
                    new(FirebaseAnalytics.ParameterCurrency, "USD"),
                    new(FirebaseAnalytics.ParameterValue, (double)impressionData.revenue));
            }
            if (settings.Adjust)
            {
                AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
                adjustAdRevenue.setRevenue((double)impressionData.revenue, "USD");
                adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
                adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
                adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
                Adjust.trackAdRevenue(adjustAdRevenue);
            } 
        }
        #region Interstitial Ad Methods

        private void LoadInterstitial()
        {
            IronSource.Agent.loadInterstitial();
        }
        new public void ShowInterstitial(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null)
        {
            if (IronSource.Agent.isInterstitialReady() && IsCanShowInterstital == true)
            {
                OnStartAdEvent?.Invoke();

                if (OnFinishAdEvent != null) OnFinishAdEventHandler += OnFinishAdEvent;

                IronSource.Agent.showInterstitial();
            }
            else OnFailedAdEvent?.Invoke();

            Debug.Log("Is Interstitial ready - " + IronSource.Agent.isInterstitialReady());
            Debug.Log("is CanShow Interstital - " + IsCanShowInterstital);
        }
        private void OnInterstitialDismissedEvent()
        {
            Debug.Log("Interstitial dismissed");

            OnFinishAdEventHandler?.Invoke();
            OnFinishAdEventHandler = null;
            SetInterstitialTimer(RemoteConfigValues.cooldown_between_INTs);
            LoadInterstitial();
        }


        #endregion

        #region Rewarded Ad Methods

        new public void ShowRewardedAd(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null, Action OnFinishRewardedVideoWithSuccessEvent = null)
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                OnStartAdEvent?.Invoke();

                if (OnFinishAdEvent != null) OnFinishAdEventHandler += OnFinishAdEvent;

                OnFinishRewardedVideoWithSuccessEventHandler = null;
                if (OnFinishRewardedVideoWithSuccessEvent != null) OnFinishRewardedVideoWithSuccessEventHandler += OnFinishRewardedVideoWithSuccessEvent;

                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                OnFailedAdEvent?.Invoke();
            }
        }
  
        private void OnRewardedAdDismissedEvent()
        {
            Debug.Log("Rewarded ad dismissed");

            OnFinishAdEventHandler?.Invoke();
            OnFinishAdEventHandler = null;
            SetInterstitialTimer(RemoteConfigValues.cooldown_after_RVs);
        }
        #endregion

        #region Banner Ad Methods

        new public void ShowBanner()
        {
            if (IsBannerActive == false)
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
                SetIsBannerActive(true);
            }
        }
        new public void HideBanner()
        {
            if (IsBannerActive == true)
            {
                IronSource.Agent.destroyBanner();
                SetIsBannerActive(false);
            }
        }
        #endregion

        void OnEnable()
        {
            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;

            //Add Rewarded Video DemandOnly Events
            IronSourceEvents.onRewardedVideoAdOpenedDemandOnlyEvent += RewardedVideoAdOpenedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdClosedDemandOnlyEvent += RewardedVideoAdClosedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdLoadedDemandOnlyEvent += RewardedVideoAdLoadedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdRewardedDemandOnlyEvent += RewardedVideoAdRewardedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedDemandOnlyEvent += RewardedVideoAdShowFailedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdClickedDemandOnlyEvent += RewardedVideoAdClickedDemandOnlyEvent;
            IronSourceEvents.onRewardedVideoAdLoadFailedDemandOnlyEvent += RewardedVideoAdLoadFailedDemandOnlyEvent;

            // Add Interstitial Events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

            // Add Interstitial DemandOnly Events
            IronSourceEvents.onInterstitialAdReadyDemandOnlyEvent += InterstitialAdReadyDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedDemandOnlyEvent += InterstitialAdLoadFailedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdShowFailedDemandOnlyEvent += InterstitialAdShowFailedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdClickedDemandOnlyEvent += InterstitialAdClickedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdOpenedDemandOnlyEvent += InterstitialAdOpenedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdClosedDemandOnlyEvent += InterstitialAdClosedDemandOnlyEvent;

            // Add Banner Events
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
        }

        #region RewardedAd callback handlers

        void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
        {
            Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
        }

        void RewardedVideoAdOpenedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
        {
            Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
            OnFinishRewardedVideoWithSuccessEventHandler?.Invoke();
            OnFinishRewardedVideoWithSuccessEventHandler = null;
        }

        void RewardedVideoAdClosedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
            OnRewardedAdDismissedEvent();
        }

        void RewardedVideoAdStartedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
        }

        void RewardedVideoAdEndedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
        }

        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
        }

        /************* RewardedVideo DemandOnly Delegates *************/

        void RewardedVideoAdLoadedDemandOnlyEvent(string instanceId)
        {

            Debug.Log("unity-script: I got RewardedVideoAdLoadedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {

            Debug.Log("unity-script: I got RewardedVideoAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void RewardedVideoAdOpenedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdOpenedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdClosedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClosedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            Debug.Log("unity-script: I got RewardedVideoAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void RewardedVideoAdClickedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClickedDemandOnlyEvent for instance: " + instanceId);
        }


        #endregion

        #region Interstitial callback handlers

        void InterstitialAdReadyEvent()
        {
            Debug.Log("unity-script: I got InterstitialAdReadyEvent");
            interstitialRetryAttempt = 0;
        }

        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
            Invoke("LoadInterstitial", (float)retryDelay);
        }

        void InterstitialAdShowSucceededEvent()
        {
            Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
        }

        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void InterstitialAdClickedEvent()
        {
            Debug.Log("unity-script: I got InterstitialAdClickedEvent");
        }

        void InterstitialAdOpenedEvent()
        {
            Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
        }

        void InterstitialAdClosedEvent()
        {
            Debug.Log("unity-script: I got InterstitialAdClosedEvent");
            OnInterstitialDismissedEvent();
        }

        /************* Interstitial DemandOnly Delegates *************/

        void InterstitialAdReadyDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got InterstitialAdReadyDemandOnlyEvent for instance: " + instanceId);
        }

        void InterstitialAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            Debug.Log("unity-script: I got InterstitialAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", error code: " + error.getCode() + ",error description : " + error.getDescription());
        }

        void InterstitialAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            Debug.Log("unity-script: I got InterstitialAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", error code :  " + error.getCode() + ",error description : " + error.getDescription());
        }

        void InterstitialAdClickedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got InterstitialAdClickedDemandOnlyEvent for instance: " + instanceId);
        }

        void InterstitialAdOpenedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got InterstitialAdOpenedDemandOnlyEvent for instance: " + instanceId);
        }

        void InterstitialAdClosedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got InterstitialAdClosedDemandOnlyEvent for instance: " + instanceId);
        }




        #endregion

        #region Banner callback handlers

        void BannerAdLoadedEvent()
        {
            Debug.Log("unity-script: I got BannerAdLoadedEvent");
        }

        void BannerAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        }

        void BannerAdClickedEvent()
        {
            Debug.Log("unity-script: I got BannerAdClickedEvent");
        }

        void BannerAdScreenPresentedEvent()
        {
            Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
        }

        void BannerAdScreenDismissedEvent()
        {
            Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
        }

        void BannerAdLeftApplicationEvent()
        {
            Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
        }

        #endregion



        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}





