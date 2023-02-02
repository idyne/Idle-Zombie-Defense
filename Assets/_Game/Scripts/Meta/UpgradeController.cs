using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.Events;

public class UpgradeController : MonoBehaviour
{
    private static UpgradeController instance = null;
    public static UpgradeController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UpgradeController>();
            return instance;
        }
    }
    
    public UnityEvent<int> OnSoldierMergeLevelUpgrade { get; private set; } = new();
    public UnityEvent OnBaseDefenseUpgrade { get; private set; } = new();

    private bool CanAfford(int price) => price <= PlayerProgression.MONEY;

    public void UpgradeBaseDefense()
    {
        int price = CostManager.GetBaseDefenseLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.BaseDefenseLevel++;
            PlayerProgression.MONEY -= price;
            OnBaseDefenseUpgrade.Invoke();
        }
    }
    public void UpgradeTrapCapacity()
    {
        int price = CostManager.GetTrapCapacityPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TrapCapacity++;
            PlayerProgression.MONEY -= price;
        }
    }

    public void UpgradeTurretCapacity()
    {
        int price = CostManager.GetTurretCapacityPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TurretCapacity++;
            PlayerProgression.MONEY -= price;
        }
    }

    public void UpgradeSoldierMergeLevel()
    {
        int price = CostManager.GetSoldierMergeLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.SoldierMergeLevel++;
            PlayerProgression.MONEY -= price;
            OnSoldierMergeLevelUpgrade.Invoke(PlayerProgression.PlayerData.SoldierMergeLevel);
        }
    }
}
