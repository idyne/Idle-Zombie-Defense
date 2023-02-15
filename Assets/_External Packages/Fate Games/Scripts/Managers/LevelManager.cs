using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using static LevelManager;

namespace FateGames
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        public UnityEvent OnStart { get; private set; } = new();
        public UnityEvent OnSuccess { get; private set; } = new();
        public UnityEvent OnFail { get; private set; } = new();

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;

        }
        public void StartLevel()
        {
            OnStart.Invoke();
        }
        public void FinishLevel(bool success)
        {
            if (GameManager.Instance.State == GameState.FINISHED) return;
            PlayerPrefs.SetInt("AnchorWaveLevel", WaveLevel);
            if (!success)
            {
                MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Fail, Day);
                PlayerProgression.PlayerData.WaveLevel = Day * 4 - 3;
            }
            UIButtonManager.Instance.HideInWaveButtons();
            Turret.Stopped = true;
            Tower.Instance.Explode();
            WaveController.Instance.StopWave();
            Barracks.Instance.DeactivateSoldiers();
            GameManager.Instance.UpdateGameState(GameState.FINISHED);
            CameraController.Instance.ZoomOut();
            DOVirtual.DelayedCall(3, () =>
            {
                if (success) OnSuccess.Invoke();
                else OnFail.Invoke();
                SceneManager.FinishLevel(success);
            });


        }
    }
}