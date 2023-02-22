using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTutorial : TutorialIndicator
{

    private void Awake()
    {
        if (PlayerProgression.HasEverPlaced) Hide();
        else Show();
        PlayerProgression.OnHasEverPlacedChanged.AddListener((hasEverPlaced) => { if (hasEverPlaced) Hide(); });
    }
}
