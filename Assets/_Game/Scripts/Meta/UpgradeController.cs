using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.Events;

public static class UpgradeController
{

    public static UnityEvent<int> OnSoldierMergeLevelUpgrade { get; private set; } = new();
    public static UnityEvent OnBaseDefenseUpgrade { get; private set; } = new();

    private static bool CanAfford(int price) => PlayerProgression.CanAffordUpgrade(price);

    public static void UpgradeBaseDefense()
    {
        int price = CostManager.GetBaseDefenseLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.BaseDefenseLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
            OnBaseDefenseUpgrade.Invoke();
        }
    }
    public static void UpgradeThrowableWeaponsGuy()
    {
        int price = CostManager.GetThrowableWeaponsGuyLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeAirstrike()
    {
        int price = CostManager.GetAirstrikePrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.AirstrikeLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeTNT()
    {
        int price = CostManager.GetTNTUpgradePrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TNTLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeFrost()
    {
        int price = CostManager.GetFrostUpgradePrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.FrostLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeBarbwire()
    {
        int price = CostManager.GetBarbwireUpgradePrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.BarbwireLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeTurret()
    {
        int price = CostManager.GetTNTUpgradePrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TurretLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
        }
    }
    public static void UpgradeSoldierMergeLevel()
    {
        int price = CostManager.GetSoldierMergeLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.SoldierMergeLevel++;
            PlayerProgression.UPGRADE_POINT -= price;
            OnSoldierMergeLevelUpgrade.Invoke(PlayerProgression.PlayerData.SoldierMergeLevel);
        }
    }
}
