using Moonee.MoonSDK.Internal;
using Moonee.MoonSDK.Internal.Analytics;
using System;
using UnityEngine;

public class AdvertisementManager : MonoBehaviour
{
    protected static Action<Action, Action, Action> OnShowInterstitialEventHandler;
    protected static Action<Action, Action, Action, Action> OnShowRewardedVideoEventHandler;
    protected static Action OnShowBannerEventHandler;
    protected static Action OnHideBannerEventHandler;

    protected Action OnFinishAdEventHandler;
    protected Action OnFinishRewardedVideoWithSuccessEventHandler;

    public static double InterstitialTimer { get; private set; }
    public static bool IsBannerActive { get; private set; }
    public static bool IsCanShowInterstital { get; private set; }

    protected int interstitialRetryAttempt;
    protected int rewardedRetryAttempt;

    protected static MoonSDKSettings settings;

    void Awake()
    {
        settings = MoonSDKSettings.Load();
        FirebaseManager.OnRemoteConfigValuesReceived += () => { SetInterstitialTimer(RemoteConfigValues.int_grace_time); };
    }

    void Update()
    {
        CalculateInterstitialTimer();
    }
    protected void SetInterstitialTimer(double time)
    {
        InterstitialTimer = time;
    }
    public void SetIsBannerActive(bool isActive)
    {
        IsBannerActive = isActive;
    }
    public static void ShowInterstitial(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null)
    {
        OnShowInterstitialEventHandler.Invoke(OnStartAdEvent, OnFinishAdEvent, OnFailedAdEvent);
    }
    public static void ShowRewardedAd(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null, Action OnFinishRewardedVideoWithSuccessEvent = null)
    {
        OnShowRewardedVideoEventHandler?.Invoke(OnStartAdEvent, OnFinishAdEvent, OnFailedAdEvent, OnFinishRewardedVideoWithSuccessEvent);
    }
    public static void ShowBanner()
    {
        OnShowBannerEventHandler?.Invoke();
    }
    public static void HideBanner()
    {
        OnHideBannerEventHandler?.Invoke();
    }
    private void CalculateInterstitialTimer()
    {
        if (InterstitialTimer <= 0)
        {
            IsCanShowInterstital = true;
        }
        else
        {
            InterstitialTimer -= Time.deltaTime;
            IsCanShowInterstital = false;
        }
    }
    protected string GenerateUserID()
    {
        int[] userIDArray = new int[64];

        string userID = "";

        for (int i = 0; i < userIDArray.Length; i++)
        {
            userIDArray[i] = UnityEngine.Random.Range(0, 10);
            userID += userIDArray[i].ToString();
        }

        return userID;
    }
    public static bool IsInterstitialdAdReady()
    {
        if (settings.IronSource)
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                return true;
            }
            else return false;
        }
        else if (settings.Applovin)
        {
#if UNITY_ANDROID
            if (MaxSdk.IsInterstitialReady(settings.applovinInterstitialAndroidAdUnit))
            {
                return true;
            }
            else return false;
#elif UNITY_IPHONE
            if (MaxSdk.IsInterstitialReady(settings.applovinInterstitialIOSAdUnit))
            {
                return true;
            }
            else return false;
#else
        Debug.Log("unexpected_platform");
#endif
        }
        return false;
    }
    public static bool IsRewardedAdReady()
    {
        if (settings.IronSource)
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                return true;
            }
            else return false;
        }
        else if (settings.Applovin)
        {
#if UNITY_ANDROID
            if (MaxSdk.IsRewardedAdReady(settings.applovinRewardedVideoAndroidAdUnit))
            {
                return true;
            }
            else return false;
#elif UNITY_IPHONE
            if (MaxSdk.IsRewardedAdReady(settings.applovinRewardedVideoIOSAdUnit))
            {
                return true;
            }
            else return false;
#else
        Debug.Log("unexpected_platform");
#endif
        }
        return false;
    }
}