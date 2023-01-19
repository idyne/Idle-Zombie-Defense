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
        int result = 1;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = 1;
                break;
            case 2:
                result = 4;
                break;
            case 3:
                result = 5;
                break;
            case 4:
                result = 6;
                break;
            default:
                break;
        }
        return result;
    }

    private int GetTurretLimit()
    {
        int result = 1;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = 1;
                break;
            case 2:
                result = 2;
                break;
            case 3:
                result = 4;
                break;
            case 4:
                result = 6;
                break;
            default:
                break;
        }
        return result;
    }

    private int GetBaseDefenseLimit()
    {
        int result = 1;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = 7;
                break;
            case 2:
                result = 14;
                break;
            case 3:
                result = 30;
                break;
            case 4:
                result = 90;
                break;
            default:
                break;
        }
        return result;
    }

    private int GetSoldierMergeLimit()
    {
        int result = 1;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = 2;
                break;
            case 2:
                result = 3;
                break;
            case 3:
                result = 4;
                break;
            case 4:
                result = 4;
                break;
            default:
                break;
        }
        return result;
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
        float result = PlayerProgression.PlayerData.BaseDefenseLevel * 100;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 3;
                break;
            case 4:
                result *= 3;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }
    private int GetTrapCapacityPrice()
    {
        float result = (PlayerProgression.PlayerData.TrapCapacity + 1) * 100;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 2;
                break;
            case 4:
                result *= 2;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }
    private int GetTurretCapacityPrice()
    {
        float result = (PlayerProgression.PlayerData.TurretCapacity + 1) * 250;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 1.5f;
                break;
            case 4:
                result *= 1.5f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }
    private int GetSoldierMergeLevelPrice()
    {
        float result = PlayerProgression.PlayerData.SoldierMergeLevel * 1500;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 1.5f;
                break;
            case 4:
                result *= 1.5f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
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
