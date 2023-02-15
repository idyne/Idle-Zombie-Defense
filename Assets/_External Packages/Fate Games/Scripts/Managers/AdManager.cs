using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

namespace FateGames
{
    public static class AdManager
    {
        public static IEnumerator ShowInterstitial()
        {
            if (WaveLevel > RemoteConfigValues.int_grace_level)
            {
                if (AdvertisementManager.IsCanShowInterstital && AdvertisementManager.IsInterstitialdAdReady())
                {
                    bool isAdDone = false;
                    void PauseGame() => Time.timeScale = 0;
                    void ContinueAfterAd() { isAdDone = true; Time.timeScale = 1; }
                    AdvertisementManager.ShowInterstitial(OnStartAdEvent: PauseGame, OnFinishAdEvent: ContinueAfterAd, OnFailedAdEvent: ContinueAfterAd);
                    yield return new WaitUntil(() => isAdDone);
                }
            }
            else
            {
                Debug.Log("Not Grace");
            }
        }
        public static IEnumerator ShowRewardedAd(System.Action OnSuccess, System.Action OnFail)
        {
            if (WaveLevel > RemoteConfigValues.int_grace_level)
            {
                if (AdvertisementManager.IsRewardedAdReady())
                {
                    bool isAdDone = false;
                    void PauseGame() { Time.timeScale = 0; Debug.Log("PauseGame"); }
                    void ContinueAfterAd() { isAdDone = true; Time.timeScale = 1; Debug.Log("ContinueAfterAd"); }
                    bool isSuccess = false;
                    AdvertisementManager.ShowRewardedAd(OnStartAdEvent: PauseGame, OnFinishAdEvent: ContinueAfterAd, OnFailedAdEvent: ContinueAfterAd, OnFinishRewardedVideoWithSuccessEvent: () => { isSuccess = true; OnSuccess(); });
                    yield return new WaitUntil(() => isAdDone);
                    if (!isSuccess) OnFail();
                }
                else
                {
                    Debug.Log("Not Ready");
                    OnFail();
                }
            }
            else
            {
                Debug.Log("Grace");
                OnFail();
            }
        }
    }
}
