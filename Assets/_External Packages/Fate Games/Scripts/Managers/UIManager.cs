using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FateGames
{
    public class UIManager : MonoBehaviour
    {
        private static UIWinScreen winScreen;
        private static UILoseScreen loseScreen;
        private static UILevelText levelText;
        private static UIStartText startText;
        private static UILoadingScreen loadingScreen;
        public static UIWinScreen WinScreen { get => winScreen; }
        public static UILoseScreen LoseScreen { get => loseScreen; }
        public static UILevelText LevelText { get => levelText; }
        public static UIStartText StartText { get => startText; }
        public static UILoadingScreen LoadingScreen { get => loadingScreen; }

        public static void CreateUILevelText()
        {
            levelText = Instantiate(PrefabManager.Prefabs["UILevelText"]).GetComponent<UILevelText>();
        }
        public static void CreateUIWinScreen()
        {
            winScreen = Instantiate(PrefabManager.Prefabs["UIWinScreen"]).GetComponent<UIWinScreen>();
        }
        public static void CreateUILoseScreen()
        {
            loseScreen = Instantiate(PrefabManager.Prefabs["UILoseScreen"]).GetComponent<UILoseScreen>();
        }
        public static void CreateUIStartText()
        {
            startText = Instantiate(PrefabManager.Prefabs["UIStartText"]).GetComponent<UIStartText>();
        }
        public static void CreateUILoadingScreen()
        {
            loadingScreen = Instantiate(PrefabManager.Prefabs["UILoadingScreen"]).GetComponent<UILoadingScreen>();
        }
    }
}
