using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.Events;

public class AimTutorial : TutorialIndicator
{
    private void Awake()
    {
        Hide();
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            if (!PlayerProgression.HasEverAimed)
                Show();
            else Hide();
        });
    }
}
