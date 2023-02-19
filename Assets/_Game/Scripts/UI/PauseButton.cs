using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using FateGames;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Sprite pauseSprite = null;
    [SerializeField] private Sprite continueSprite = null;
    [SerializeField] private Image buttonImage = null;
    public static bool Paused = false;
    public static UnityEvent OnPause = new(), OnResume = new();

    private void Start()
    {
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            gameObject.SetActive(true);
        });

        WaveController.Instance.OnWaveEnd.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    public void Pause()
    {
        buttonImage.sprite = continueSprite;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        buttonImage.sprite = pauseSprite;
        Time.timeScale = 1;
    }

    public void Press()
    {
        Paused = !Paused;

        if (Paused) Pause();
        else Resume();
        if (Paused) OnPause.Invoke();
        else OnResume.Invoke();
    }
}
