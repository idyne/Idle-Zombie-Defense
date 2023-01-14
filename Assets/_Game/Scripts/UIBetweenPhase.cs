using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetweenPhase : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;
    [SerializeField] private GameObject startWaveButton = null;

    private void Start()
    {
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        WaveController.Instance.OnWaveEnd.AddListener(() =>
        {
            gameObject.SetActive(true);
        });
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
        gameObject.SetActive(false);
        startWaveButton.SetActive(false);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        gameObject.SetActive(true);
        startWaveButton.SetActive(true);
    }
}
