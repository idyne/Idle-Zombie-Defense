using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUpgradesPanelButtonManager : MonoBehaviour
{
    [SerializeField] private UIButton baseDefenseButton, trapCapacityButton, turretCapacityButton, soldierMergeLevelButton;
    [SerializeField] private TextMeshProUGUI baseDefenseLevelText, trapCapacityLevelText, turretCapacityLevelText, soldierMergeLevelText;

    public void UpdateBaseDefenseButton(int cost, bool maxedOut)
    {
        if (!maxedOut)
            baseDefenseButton.SetText(UIMoney.FormatMoney(cost));
        else baseDefenseButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) baseDefenseButton.Deactivate();
        else baseDefenseButton.Activate();
        baseDefenseLevelText.text = "Level " + PlayerProgression.PlayerData.BaseDefenseLevel.ToString();
    }
    public void UpdateTrapCapacityButton(int cost, bool maxedOut)
    {
        bool locked = WaveController.Day <= 7;
        if (locked)
            trapCapacityButton.SetText("Unlocks after 1st week");
        else if (!maxedOut)
            trapCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else trapCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) trapCapacityButton.Deactivate();
        else trapCapacityButton.Activate();
        trapCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TrapCapacity + 1);
    }
    public void UpdateTurretCapacityButton(int cost, bool maxedOut)
    {
        bool locked = WaveController.Day <= 14;
        if (locked)
            turretCapacityButton.SetText("Unlocks after 2nd week");
        else
        if (!maxedOut)
            turretCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else turretCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) turretCapacityButton.Deactivate();
        else turretCapacityButton.Activate();
        turretCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TurretCapacity + 1);
    }
    public void UpdateSoldierMergeLevelButton(int cost, bool maxedOut)
    {
        bool locked = WaveController.Day <= 7;
        if (locked)
            soldierMergeLevelButton.SetText("Unlocks after 1st week");
        else
        if (!maxedOut)
            soldierMergeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else soldierMergeLevelButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) soldierMergeLevelButton.Deactivate();
        else soldierMergeLevelButton.Activate();
        soldierMergeLevelText.text = "Level " + PlayerProgression.PlayerData.SoldierMergeLevel.ToString();
    }
}
