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
        }
    }
}
