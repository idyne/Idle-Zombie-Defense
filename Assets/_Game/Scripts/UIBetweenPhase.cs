using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetweenPhase : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;
    [SerializeField] private GameObject button = null;
    [SerializeField] private GameObject startWaveButton = null;

    public void OpenPanel()
    {
        panel.SetActive(true);
        button.SetActive(false);
        startWaveButton.SetActive(false);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        button.SetActive(true);
        startWaveButton.SetActive(true);
    }
}
