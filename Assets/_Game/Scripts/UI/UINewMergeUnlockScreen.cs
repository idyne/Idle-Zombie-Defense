using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using TMPro;

public class UINewMergeUnlockScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private string[] soldierNames = null;
    [SerializeField] private TextMeshProUGUI bannerText = null;
    [SerializeField] private PauseButton pauseButton = null;

    void Awake()
    {
        Barracks.Instance.OnNewMergeUnlocked.AddListener((level) =>
        {
            if (GameManager.Instance.State != GameState.FINISHED)
                Open(level);
        });
        container.SetActive(false);
    }

    private void Open(int level)
    {
        pauseButton.Press();
        container.SetActive(true);
        bannerText.text = soldierNames[level];
        SoldierRenderController.Instance.Render(level);
    }

    // Continue butonunun onclick eventine baðlý
    public void Close()
    {
        container.SetActive(false);
        pauseButton.Press();
    }

}
