using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Settings;

public class UIUpgradesPanelButtonManager : MonoBehaviour
{
    [SerializeField] private UIBetweenPhase UIBetweenPhase;
    [SerializeField] private UIButton baseDefenseButton, trapCapacityButton, turretCapacityButton, soldierMergeLevelButton;
    [SerializeField] private TextMeshProUGUI baseDefenseLevelText, trapCapacityLevelText, turretCapacityLevelText, soldierMergeLevelText;

    public void UpdateBaseDefenseButton(int cost, bool maxedOut)
    {
        if (!maxedOut)
            baseDefenseButton.SetText(UIMoney.FormatMoney(cost));
        else baseDefenseButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) baseDefenseButton.Deactivate();
        else if (!baseDefenseButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            baseDefenseButton.Activate();
        }
        baseDefenseLevelText.text = "Level " + PlayerProgression.PlayerData.BaseDefenseLevel.ToString();
    }
    public void UpdateTrapCapacityButton(int cost, bool maxedOut)
    {
        int unlockDay = Mathf.Min(FrostUnlockDay, TNTUnlockDay);
        bool locked = WaveController.Day < unlockDay;
        if (locked)
        {
            int remainingDays = unlockDay - WaveController.Day;
            trapCapacityButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
        }
        else if (!maxedOut)
            trapCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else trapCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) trapCapacityButton.Deactivate();
        else if (!trapCapacityButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            trapCapacityButton.Activate();
        }
        trapCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TrapCapacity + 1);
    }
    public void UpdateTurretCapacityButton(int cost, bool maxedOut)
    {
        int unlockDay = TurretUnlockDay;
        bool locked = WaveController.Day < unlockDay;
        if (locked)
        {
            int remainingDays = unlockDay - WaveController.Day;
            turretCapacityButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
        }
        else
        if (!maxedOut)
            turretCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else turretCapacityButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) turretCapacityButton.Deactivate();
        else if (!turretCapacityButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            turretCapacityButton.Activate();
        }
        turretCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TurretCapacity + 1);
    }
    public void UpdateSoldierMergeLevelButton(int cost, bool maxedOut)
    {
        if (!maxedOut)
            soldierMergeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else soldierMergeLevelButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) soldierMergeLevelButton.Deactivate();
        else if (!soldierMergeLevelButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            soldierMergeLevelButton.Activate();
        }
        soldierMergeLevelText.text = "Level " + PlayerProgression.PlayerData.SoldierMergeLevel.ToString();
    }
}
