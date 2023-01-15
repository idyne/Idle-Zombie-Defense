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
    private void Awake()
    {
        if (Instance) { gameObject.SetActive(false); }
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            buttonManager.UpdateBaseDefenseButton(GetBaseDefenseLevelPrice(), PlayerProgression.PlayerData.BaseDefenseLevel > 20);
            buttonManager.UpdateTrapCapacityButton(GetTrapCapacityPrice(), PlayerProgression.PlayerData.TrapCapacity > 5);
            buttonManager.UpdateTurretCapacityButton(GetTurretCapacityPrice(), PlayerProgression.PlayerData.TurretCapacity > 5);
            buttonManager.UpdateSoldierMergeLevelButton(GetSoldierMergeLevelPrice(), PlayerProgression.PlayerData.SoldierMergeLevel > 3);
        });
    }

    private void Start()
    {
        buttonManager.UpdateBaseDefenseButton(GetBaseDefenseLevelPrice(), PlayerProgression.PlayerData.BaseDefenseLevel > 20);
        buttonManager.UpdateTrapCapacityButton(GetTrapCapacityPrice(), PlayerProgression.PlayerData.TrapCapacity > 5);
        buttonManager.UpdateTurretCapacityButton(GetTurretCapacityPrice(), PlayerProgression.PlayerData.TurretCapacity > 5);
        buttonManager.UpdateSoldierMergeLevelButton(GetSoldierMergeLevelPrice(), PlayerProgression.PlayerData.SoldierMergeLevel > 3);
    }

    private bool CanAfford(int price) => price <= PlayerProgression.MONEY;
    private int GetBaseDefenseLevelPrice()
    {
        return PlayerProgression.PlayerData.BaseDefenseLevel * 50;
    }
    private int GetTrapCapacityPrice()
    {
        return (PlayerProgression.PlayerData.TrapCapacity + 1) * 50;
    }
    private int GetTurretCapacityPrice()
    {
        return (PlayerProgression.PlayerData.TurretCapacity + 1) * 250;
    }
    private int GetSoldierMergeLevelPrice()
    {
        return PlayerProgression.PlayerData.SoldierMergeLevel * 500;
    }
    public void UpgradeBaseDefense()
    {
        int price = GetBaseDefenseLevelPrice();
        if (CanAfford(price))
        {
            PlayerProgression.PlayerData.BaseDefenseLevel++;
            PlayerProgression.MONEY -= price;
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
