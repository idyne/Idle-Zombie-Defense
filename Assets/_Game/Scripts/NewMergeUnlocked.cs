using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using TMPro;

public class NewMergeUnlocked : MonoBehaviour
{
    [SerializeField] private string[] soldierNames = null;
    [SerializeField] private TextMeshProUGUI bannerText = null;
    [SerializeField] private PauseButton pauseButton = null;

    void Awake()
    {
        Barracks.Instance.OnNewMergeUnlocked.AddListener((level) =>
        {
            Open(level);
        });
        gameObject.SetActive(false);
    }

    /*private int i = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            *//*if (gameObject.activeInHierarchy)
                Close();*//*

            Open(i);
            i++;
        }
    }*/

    private void Open(int level)
    {
        pauseButton.Press();
        gameObject.SetActive(true);
        bannerText.text = soldierNames[level];
        SoldierRenderController.Instance.Render(level);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        pauseButton.Press();
    }

}
