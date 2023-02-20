using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementModeController : MonoBehaviour
{
    private static PlacementModeController instance = null;
    public static PlacementModeController Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlacementModeController>(true);
            print(instance);
            return instance;
        }
    }
    [SerializeField] private GameObject placementButtonContainer, placementModeButton, exitPlacementModeButton, notification;
    public static bool InPlacementMode { get; private set; } = false;
    public static UnityEvent OnEnterPlacementMode { get; private set; } = new();
    public static UnityEvent OnExitPlacementMode { get; private set; } = new();
    public void OpenPlacementMode()
    {
        InPlacementMode = true;
        HidePlacementModeButton();
        ShowPlacementButtons();
        ShowExitPlacementModeButton();
        UIButtonManager.Instance.HideStartAndUpgradeButtons();
        UIDayBar.Instance.Hide();
        UILevelBar.Instance.Hide();
        OnEnterPlacementMode.Invoke();
    }

    public void ExitPlacementMode()
    {
        InPlacementMode = false;
        HidePlacementButtons();
        HideExitPlacementModeButton();
        ShowPlacementModeButton();
        UIButtonManager.Instance.ShowStartAndUpgradeButtons();
        UIDayBar.Instance.Show();
        UILevelBar.Instance.Show();
        OnExitPlacementMode.Invoke();
    }

    public void HidePlacementButtons()
    {
        placementButtonContainer.SetActive(false);
    }

    public void ShowPlacementButtons()
    {
        placementButtonContainer.SetActive(true);
    }

    private void ShowPlacementModeButton()
    {
        placementModeButton.SetActive(true);
    }

    private void HidePlacementModeButton()
    {
        placementModeButton.SetActive(false);
    }

    public void ShowExitPlacementModeButton()
    {
        exitPlacementModeButton.SetActive(true);
    }

    public void HideExitPlacementModeButton()
    {
        exitPlacementModeButton.SetActive(false);
    }

    public void ShowNotification() => notification.SetActive(true);
    public void HideNotification() => notification.SetActive(false);
}
