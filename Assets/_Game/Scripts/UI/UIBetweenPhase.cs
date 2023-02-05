using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetweenPhase : MonoBehaviour
{
    [SerializeField] private UIUpgradeScreen upgradeScreen = null;
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject notification;
    private bool[] wereTheyShown;

    private void Awake()
    {
        wereTheyShown = new bool[objectsToHide.Length];
    }


    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);

    public void ShowNotification()
    {
        Debug.Log("show", notification);
        notification.SetActive(true);
    }

    public void HideNotification()
    {
        Debug.Log("hide", notification);
        notification.SetActive(false);
    }


    public void OpenPanel()
    {
        upgradeScreen.Show();
        Hide();
        HideNotification();
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            wereTheyShown[i] = objectsToHide[i].activeSelf;
            objectsToHide[i].SetActive(false);
        }
    }

    public void ClosePanel()
    {
        upgradeScreen.Hide();
        Show();
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (wereTheyShown[i])
                objectsToHide[i].SetActive(true);
        }
    }
}
