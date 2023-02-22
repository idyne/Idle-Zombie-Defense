using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class DetonationTutorial : TutorialIndicator
{
    private static bool active = false;

    private void Awake()
    {
        Hide();
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            if (!active && !PlayerProgression.HasEverDetonated && transform.parent.gameObject.activeSelf)
            {
                Show();
                active = true;
            }
        });
        WaveController.Instance.OnWaveEnd.AddListener(() =>
        {
            Hide();
            if (active) active = false;
        });
        PlayerProgression.OnHasEverDetonatedChanged.AddListener((hasEverDetonated) => { if (hasEverDetonated) Hide(); });
    }

}
