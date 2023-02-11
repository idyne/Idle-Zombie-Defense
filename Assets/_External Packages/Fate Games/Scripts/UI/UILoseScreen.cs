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

        private void Awake()
        {
            levelText.text = "DAY " + NormalizedDay;
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
    }
}