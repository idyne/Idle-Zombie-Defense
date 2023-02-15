using com.adjust.sdk;
using Firebase.Analytics;
using System;
using UnityEngine;

public class ApplovinManager : AdvertisementManager
{
    private string maxSdkKey;
    private string interstitialAdUnitId;
    private string rewardedAdUnitId;
    private string bannerAdUnitId;

    void Start()
    {
        if (settings.Applovin == false)
        {
            Destroy(this);
            return;
        }
        maxSdkKey = settings.maxSDKKey;
#if UNITY_ANDROID
        interstitialAdUnitId = settings.applovinInterstitialAndroidAdUnit;
        rewardedAdUnitId = settings.applovinRewardedVideoAndroidAdUnit;
        bannerAdUnitId = settings.applovinBannerAndroidAdUnit;

        MaxSdkAndroid.SetHasUserConsent(true);
        MaxSdkAndroid.SetIsAgeRestrictedUser(false);
        MaxSdkAndroid.SetDoNotSell(true);

#elif UNITY_IPHONE
        
        interstitialAdUnitId = settings.applovinInterstitialIOSAdUnit;
        rewardedAdUnitId = settings.applovinRewardedVideoIOSAdUnit;
        bannerAdUnitId = settings.applovinBannerIOSAdUnit;
#else
        string appKey = "unexpected_platform";
#endif

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");

                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeBannerAds();
                //MaxSdk.ShowMediationDebugger();
            };
        MaxSdk.SetUserId(GenerateUserID());
        MaxSdk.SetSdkKey(maxSdkKey);
        MaxSdk.InitializeSdk();

        OnShowRewardedVideoEventHandler += ShowRewardedAd;
        OnShowInterstitialEventHandler += ShowInterstitial;
        OnShowBannerEventHandler += ShowBanner;
        OnHideBannerEventHandler += HideBanner;
    }

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    void LoadInterstitial()
    {
        Debug.Log("Loading Interstitial Ad not ready");
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }
    new public void ShowInterstitial(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null)
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId) && IsCanShowInterstital == true)
        {
            OnStartAdEvent?.Invoke();

            if (OnFinishAdEvent != null) OnFinishAdEventHandler += OnFinishAdEvent;

            MaxSdk.ShowInterstitial(interstitialAdUnitId);
            Debug.Log("Showing Interstitial Ad not ready");
        }
        else OnFailedAdEvent?.Invoke();

        Debug.Log("Is Interstitial ready - " + IronSource.Agent.isInterstitialReady());
        Debug.Log("is CanShow Interstital - " + IsCanShowInterstital);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        Debug.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        Debug.Log("Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...");
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed");

        OnFinishAdEventHandler?.Invoke();
        OnFinishAdEventHandler = null;
        SetInterstitialTimer(RemoteConfigValues.cooldown_between_INTs);
        LoadInterstitial();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");
        TrackAdRevenue(adInfo);
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        Debug.Log("Loading rewarded ad");
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }
    new public void ShowRewardedAd(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null, Action OnFinishRewardedVideoWithSuccessEvent = null)
    {
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
        {
            OnStartAdEvent?.Invoke();

            if (OnFinishAdEvent != null) OnFinishAdEventHandler += OnFinishAdEvent;

            OnFinishRewardedVideoWithSuccessEventHandler = null;
            if (OnFinishRewardedVideoWithSuccessEvent != null) OnFinishRewardedVideoWithSuccessEventHandler += OnFinishRewardedVideoWithSuccessEvent;

            Debug.Log("Showing rewarded ad");
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
        else
        {
            OnFailedAdEvent?.Invoke();
            Debug.Log("Rewarded Ad not ready");
        }
    }
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

        Debug.Log("Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...");
        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded ad dismissed");
        LoadRewardedAd();

        OnFinishAdEventHandler?.Invoke();
        OnFinishAdEventHandler = null;
        SetInterstitialTimer(RemoteConfigValues.cooldown_after_RVs);
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        Debug.Log("Rewarded ad received reward");
        OnFinishRewardedVideoWithSuccessEventHandler?.Invoke();
        OnFinishRewardedVideoWithSuccessEventHandler = null;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Rewarded ad revenue paid");
        TrackAdRevenue(adInfo);
    }

    #endregion



    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

        if (string.IsNullOrEmpty(bannerAdUnitId)) return;

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdkUtils.IsTablet();
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
    }
    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Banner ad revenue paid");
        TrackAdRevenue(adInfo);
    }
    new public void ShowBanner()
    {
        if (IsBannerActive == false)
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
            SetIsBannerActive(true);
        }
    }
    new public void HideBanner()
    {
        if (IsBannerActive == true)
        {
            MaxSdk.HideBanner(bannerAdUnitId);
            SetIsBannerActive(false);
        }
    }

    #endregion



    private void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
    {
        if (settings.Firebase)
        {
            FirebaseAnalytics.LogEvent(
               FirebaseAnalytics.EventAdImpression,
               new(FirebaseAnalytics.ParameterAdPlatform, "Applovin"),
               new(FirebaseAnalytics.ParameterAdUnitName, adInfo.AdUnitIdentifier),
               new(FirebaseAnalytics.ParameterAdFormat, adInfo.AdFormat),
               new(FirebaseAnalytics.ParameterAdSource, adInfo.NetworkName),
               new(FirebaseAnalytics.ParameterCurrency, "USD"),
               new(FirebaseAnalytics.ParameterValue, adInfo.Revenue));
        }
        if (settings.Firebase)
        {
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);

            adjustAdRevenue.setRevenue(adInfo.Revenue, "USD");
            adjustAdRevenue.setAdRevenueNetwork(adInfo.NetworkName);
            adjustAdRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
            adjustAdRevenue.setAdRevenuePlacement(adInfo.Placement);

            Adjust.trackAdRevenue(adjustAdRevenue);
        }
    }
}