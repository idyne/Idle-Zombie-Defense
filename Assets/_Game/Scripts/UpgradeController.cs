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
    [SerializeField] private UIUpgradesPanelButtonManager buttonManager;
    public UnityEvent<int> OnSoldierMergeLevelUpgrade = new();
    public UnityEvent OnBaseDefenseUpgrade = new();
    private void Awake()
    {
        if (Instance) { gameObject.SetActive(false); }
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {

            buttonManager.UpdateBaseDefenseButton(GetBaseDefenseLevelPrice(), PlayerProgression.PlayerData.BaseDefenseLevel >= GetBaseDefenseLimit());
            buttonManager.UpdateTrapCapacityButton(GetTrapCapacityPrice(), PlayerProgression.PlayerData.TrapCapacity >= GetTrapLimit());
            buttonManager.UpdateTurretCapacityButton(GetTurretCapacityPrice(), PlayerProgression.PlayerData.TurretCapacity >= GetTurretLimit());
            buttonManager.UpdateSoldierMergeLevelButton(GetSoldierMergeLevelPrice(), PlayerProgression.PlayerData.SoldierMergeLevel >= GetSoldierMergeLimit());
        });
    }

    private int GetTrapLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TrapCapacityLimit;
            case 2:
                return Settings.Zone2.TrapCapacityLimit;
            case 3:
                return Settings.Zone3.TrapCapacityLimit;
            case 4:
                return Settings.Zone4.TrapCapacityLimit;
        }
        return 1;
    }

    private int GetTurretLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCapacityLimit;
            case 2:
                return Settings.Zone2.TurretCapacityLimit;
            case 3:
                return Settings.Zone3.TurretCapacityLimit;
            case 4:
                return Settings.Zone4.TurretCapacityLimit;
        }
        return 1;
    }

    private int GetBaseDefenseLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.BaseDefenseLimit;
            case 2:
                return Settings.Zone2.BaseDefenseLimit;
            case 3:
                return Settings.Zone3.BaseDefenseLimit;
            case 4:
                return Settings.Zone4.BaseDefenseLimit;
        }
        return 1;
    }

    private int GetSoldierMergeLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.SoldierMergeLevelLimit;
            case 2:
                return Settings.Zone2.SoldierMergeLevelLimit;
            case 3:
                return Settings.Zone3.SoldierMergeLevelLimit;
            case 4:
                return Settings.Zone4.SoldierMergeLevelLimit;
        }
        return 1;
    }

    private void Start()
    {
        buttonManager.UpdateBaseDefenseButton(GetBaseDefenseLevelPrice(), PlayerProgression.PlayerData.BaseDefenseLevel >= GetBaseDefenseLimit());
        buttonManager.UpdateTrapCapacityButton(GetTrapCapacityPrice(), PlayerProgression.PlayerData.TrapCapacity >= GetTrapLimit());
        buttonManager.UpdateTurretCapacityButton(GetTurretCapacityPrice(), PlayerProgression.PlayerData.TurretCapacity >= GetTurretLimit());
        buttonManager.UpdateSoldierMergeLevelButton(GetSoldierMergeLevelPrice(), PlayerProgression.PlayerData.SoldierMergeLevel >= GetSoldierMergeLimit());
    }

    private bool CanAfford(int price) => price <= PlayerProgression.MONEY;
    private int GetBaseDefenseLevelPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.BaseDefenseLevelCost;
            case 2:
                return Settings.Zone2.BaseDefenseLevelCost;
            case 3:
                return Settings.Zone3.BaseDefenseLevelCost;
            case 4:
                return Settings.Zone4.BaseDefenseLevelCost;
        }
        return 1;
    }
    private int GetTrapCapacityPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TrapCapacityCost;
            case 2:
                return Settings.Zone2.TrapCapacityCost;
            case 3:
                return Settings.Zone3.TrapCapacityCost;
            case 4:
                return Settings.Zone4.TrapCapacityCost;
        }
        return 1;
    }
    private int GetTurretCapacityPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCapacityCost;
            case 2:
                return Settings.Zone2.TurretCapacityCost;
            case 3:
                return Settings.Zone3.TurretCapacityCost;
            case 4:
                return Settings.Zone4.TurretCapacityCost;
        }
        return 1;
    }
    private int GetSoldierMergeLevelPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.SoldierMergeLevelCost;
            case 2:
                return Settings.Zone2.SoldierMergeLevelCost;
            case 3:
                return Settings.Zone3.SoldierMergeLevelCost;
            case 4:
                return Settings.Zone4.SoldierMergeLevelCost;
        }
        return 1;
    }
    public void UpgradeBaseDefense()
    {
        int price = GetBaseDefenseLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.BaseDefenseLevel++;
            PlayerProgression.MONEY -= price;
            OnBaseDefenseUpgrade.Invoke();
        }
    }
    public void UpgradeTrapCapacity()
    {
        int price = GetTrapCapacityPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TrapCapacity++;
            PlayerProgression.MONEY -= price;
        }
    }

    public void UpgradeTurretCapacity()
    {
        int price = GetTurretCapacityPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.TurretCapacity++;
            PlayerProgression.MONEY -= price;
        }
    }

    public void UpgradeSoldierMergeLevel()
    {
        int price = GetSoldierMergeLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.SoldierMergeLevel++;
            PlayerProgression.MONEY -= price;
            OnSoldierMergeLevelUpgrade.Invoke(PlayerProgression.PlayerData.SoldierMergeLevel);
        }
    }
}
