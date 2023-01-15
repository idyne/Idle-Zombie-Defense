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
        if (!maxedOut)
            trapCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else trapCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) trapCapacityButton.Deactivate();
        else trapCapacityButton.Activate();
        trapCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TrapCapacity + 1);
    }
    public void UpdateTurretCapacityButton(int cost, bool maxedOut)
    {
        if (!maxedOut)
            turretCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else turretCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) turretCapacityButton.Deactivate();
        else turretCapacityButton.Activate();
        turretCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TurretCapacity + 1);
    }
    public void UpdateSoldierMergeLevelButton(int cost, bool maxedOut)
    {
        if (!maxedOut)
            soldierMergeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else soldierMergeLevelButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) soldierMergeLevelButton.Deactivate();
        else soldierMergeLevelButton.Activate();
        soldierMergeLevelText.text = "Level " + PlayerProgression.PlayerData.SoldierMergeLevel.ToString();
    }
}
