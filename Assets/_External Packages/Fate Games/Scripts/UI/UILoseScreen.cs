using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelManager;
namespace FateGames
{
    public class UILoseScreen : MonoBehaviour
    {
        [SerializeField] private Text levelText;
        [SerializeField] private Button continueButton, reviveButton;
        [SerializeField] private GameObject adIcon, loading;

        private void Awake()
        {
            levelText.text = "DAY " + NormalizedDay;
            StartCoroutine(PrepareReviveButton());
            StartCoroutine(ShowContinueButton());
        }

        private IEnumerator ShowContinueButton(float delay = 2)
        {
            yield return new WaitForSeconds(delay);
            continueButton.gameObject.SetActive(true);
        }

        private IEnumerator PrepareReviveButton()
        {
            reviveButton.interactable = false;
            adIcon.SetActive(false);
            loading.SetActive(true);
            yield return new WaitUntil(AdvertisementManager.IsRewardedAdReady);
            reviveButton.interactable = true;
            adIcon.SetActive(true);
            loading.SetActive(false);
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            StartCoroutine(ContinueCoroutine());
        }

        private IEnumerator ContinueCoroutine()
        {
            if (RemoteConfigValues.show_int_if_fail)
                yield return AdManager.ShowInterstitial();
            SceneManager.LoadCurrentLevel();
        }

        public void Revive()
        {
            void Success()
            {
                gameObject.SetActive(false);
                WaveLevel = PlayerPrefs.GetInt("AnchorWaveLevel");
                Tower tower = TowerController.Instance.GetCurrentTower();
                ObjectPooler.SpawnFromPool("Revive Effect", tower.Transform.position, Quaternion.identity);
                tower.Rewind();
                Barracks.Instance.RewindSoldiers();
                tower.CameraController.ZoomIn();
                DOVirtual.DelayedCall(2, () =>
                {
                    WaveBomb.Instance.Explode();
                    Turret.Stopped = false;
                    GameManager.Instance.UpdateGameState(GameState.IN_GAME);
                });
                //SceneManager.LoadCurrentLevel();
            }
            void Fail()
            {
                SceneManager.LoadCurrentLevel();
            }

            StartCoroutine(AdManager.ShowRewardedAd(Success, Fail));
        }
    }
}