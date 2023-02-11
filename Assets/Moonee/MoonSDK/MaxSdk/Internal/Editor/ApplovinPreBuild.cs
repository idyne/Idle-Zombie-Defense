using Moonee.MoonSDK.Internal.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Moonee.MoonSDK.Internal.Advertisement.Editor
{
    public class ApplovinPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateApplovinSettings(MoonSDKSettings.Load());
        }
        public static void CheckAndUpdateApplovinSettings(MoonSDKSettings settings)
        {
            if (!settings.Applovin) return;

            AppLovinSettings appLovinSettings = Resources.Load<AppLovinSettings>("AppLovinSettings");
            appLovinSettings.SdkKey = settings.maxSDKKey; 

            if (settings == null || string.IsNullOrEmpty(settings.maxSDKKey.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoMaxSDKKey);

#if UNITY_IOS

            if (settings == null || string.IsNullOrEmpty(settings.applovinInterstitialIOSAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoIOSInterstitialAdUnitSDKKey);

            if (settings == null || string.IsNullOrEmpty(settings.applovinRewardedVideoIOSAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoIOSRewardedAdUnitSDKKey);

            if (settings == null || string.IsNullOrEmpty(settings.applovinBannerIOSAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoIOSBannerAdUnitSDKKey);

             if (settings == null || string.IsNullOrEmpty(settings.adMobAndroidAppKey.Replace(" ", string.Empty)))
            {
                Debug.LogWarning("There is no ad mob iOS app id");
            }
            else
            {
                appLovinSettings.AdMobAndroidAppId = settings.adMobIOSAppKey;
            }
#endif
#if UNITY_ANDROID
            if (settings == null || string.IsNullOrEmpty(settings.applovinInterstitialAndroidAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAndroidInterstitialAdUnitSDKKey);

            if (settings == null || string.IsNullOrEmpty(settings.applovinRewardedVideoAndroidAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAndroidRewardedAdUnitSDKKey);

            if (settings == null || string.IsNullOrEmpty(settings.applovinBannerAndroidAdUnit.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAndroidBannerAdUnitSDKKey);

            if (settings == null || string.IsNullOrEmpty(settings.adMobAndroidAppKey.Replace(" ", string.Empty)))
            {
                Debug.LogWarning("There is no ad mob android app id");
            }
            else
            {
                appLovinSettings.AdMobAndroidAppId = settings.adMobAndroidAppKey;
            }
           
#endif

        }
    }
}