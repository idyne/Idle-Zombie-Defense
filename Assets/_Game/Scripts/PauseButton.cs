using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Sprite pauseSprite = null;
    [SerializeField] private Sprite continueSprite = null;
    [SerializeField] private Image buttonImage = null;
    private bool isPaused = false;

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
    }

    public void Press()
    {
        print("pause");
        isPaused = !isPaused;

        if (isPaused)
            buttonImage.sprite = continueSprite;
        else
            buttonImage.sprite = pauseSprite;


    }
}
