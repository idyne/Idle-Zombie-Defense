using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FateGames
{
    public static class SceneManager
    {
        private static int levelCount { get => UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1; }

        private static int CurrentSceneIndex
        {
            get
            {
                int result = PlayerProgression.CurrentLevel % levelCount;
                if (result == 0)
                    result = levelCount;
                return result + 1;
            }
        }
        public static void LoadCurrentLevel()
        {
            LoadScene(CurrentSceneIndex);
        }
        public static void LoadScene(int sceneIndex)
        {
            if (sceneIndex < 0 && sceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                throw new System.ArgumentOutOfRangeException();
            Debug.Log(DOTween.KillAll() + " tweens have killed.");
            SoundFX.StopWorkers();
            GameManager.Instance.StartCoroutine(LoadSceneAsynchronously(sceneIndex));
        }

        private static IEnumerator LoadSceneAsynchronously(int sceneIndex)
        {
            AdvertisementManager.HideBanner();
            GameManager.Instance.UpdateGameState(GameState.LOADING_SCREEN);
            UIManager.CreateUILoadingScreen();
            GlobalEventDispatcher.Clear();
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                yield return null;
            }
            if (operation.isDone)
            {
                //ObjectPooler.CreatePools();
                if (GameManager.Instance.ShowLevelText)
                    UIManager.CreateUILevelText();
                UIManager.CreateUIStartText();
                GameManager.Instance.UpdateGameState(GameState.START_SCREEN);
            }
        }

        public static void StartLevel()
        {
            AnalyticsManager.ReportStartProgress();
            GameManager.Instance.UpdateGameState(GameState.IN_GAME);
            if (UIManager.StartText)
                UIManager.StartText.Hide();
            GameManager.Instance.LevelManager.StartLevel();
        }
        public static void FinishLevel(bool success)
        {
            AnalyticsManager.ReportFinishProgress(success);
            GameManager.Instance.UpdateGameState(GameState.COMPLETE_SCREEN);
            if (success)
                UIManager.CreateUIWinScreen();
            else
                UIManager.CreateUILoseScreen();
            if (success) PlayerProgression.CurrentLevel += 1;
        }
    }

}
