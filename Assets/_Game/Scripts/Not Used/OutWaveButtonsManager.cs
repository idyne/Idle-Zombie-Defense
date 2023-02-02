using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using System.Linq;
using static Settings;

public class OutWaveButtonsManager : MonoBehaviour
{
    /*private static OutWaveButtonsManager instance = null;
    public static OutWaveButtonsManager Instance { get { if (instance == null) instance = FindObjectOfType<OutWaveButtonsManager>(); return instance; } }
    [SerializeField] private GameObject container;
    [SerializeField] private UIOutWaveButton frostButton, turretButton, tntButton;
    [SerializeField] private PlacementController turretPlacementController, trapPlacementController;*/

    private void Awake()
    {
        //print(Instance);
        /*PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateFrostButton());
        PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateTurretButton());
        PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateTNTButton());*/
    }
    private void Start()
    {/*
        UpdateFrostButton();
        UpdateTurretButton();
        UpdateTNTButton();
        */
    }/*
    public void Hide()
    {
        container.SetActive(false);
    }

    public void Show()
    {
        container.SetActive(true);
    }*/
    /*
    public static int GetFrostPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.FrostBombCost;
            case 2:
                return Settings.Zone2.FrostBombCost;
            case 3:
                return Settings.Zone3.FrostBombCost;
            case 4:
                return Settings.Zone4.FrostBombCost;
        }
        return 1;
    }

    public static int GetTurretPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCost;
            case 2:
                return Settings.Zone2.TurretCost;
            case 3:
                return Settings.Zone3.TurretCost;
            case 4:
                return Settings.Zone4.TurretCost;
        }
        return 1;
    }

    public static int GetTNTPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.ExplosiveBombCost;
            case 2:
                return Settings.Zone2.ExplosiveBombCost;
            case 3:
                return Settings.Zone3.ExplosiveBombCost;
            case 4:
                return Settings.Zone4.ExplosiveBombCost;
        }
        return 1;
    }

    public void UpdateFrostButton()
    {
        int unlockDay = FrostUnlockDay;
        bool locked = WaveController.Day < unlockDay;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        frostButton.ShowCapacity();
        frostButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
        {
            int remainingDays = unlockDay - WaveController.Day;
            frostButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            frostButton.HideCapacity();
        }
        else if (noCapacity)
            frostButton.SetText("No Capacity");
        else
            frostButton.SetText(UIMoney.FormatMoney(GetFrostPrice()));
        if (!PlayerProgression.CanAfford(GetFrostPrice()) || noCapacity || locked) frostButton.Deactivate();
        else frostButton.Activate();
    }

    public void UpdateTurretButton()
    {
        int unlockDay = TurretUnlockDay;
        bool locked = WaveController.Day < unlockDay;
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count();
        turretButton.ShowCapacity();
        turretButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
        {
            int remainingDays = unlockDay - WaveController.Day;
            turretButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            turretButton.HideCapacity();
        }
        else if (noCapacity)
            turretButton.SetText("No Capacity");
        else
            turretButton.SetText(UIMoney.FormatMoney(GetTurretPrice()));
        if (!PlayerProgression.CanAfford(GetTurretPrice()) || noCapacity || locked) turretButton.Deactivate();
        else turretButton.Activate();
    }

    public void UpdateTNTButton()
    {
        int unlockDay = TNTUnlockDay;
        bool locked = WaveController.Day < unlockDay;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        tntButton.ShowCapacity();
        tntButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
        {
            int remainingDays = unlockDay - WaveController.Day;
            tntButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            tntButton.HideCapacity();
        }
        else if (noCapacity)
            tntButton.SetText("No Capacity");
        else
            tntButton.SetText(UIMoney.FormatMoney(GetTNTPrice()));
        if (!PlayerProgression.CanAfford(GetTNTPrice()) || noCapacity || locked) tntButton.Deactivate();
        else tntButton.Activate();
    }

    public void SelectFrostBomb()
    {
        if (!PlayerProgression.CanAfford(GetFrostPrice()) || WaveController.Day < Settings.FrostUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        if (capacity <= 0) return;
        FrostBomb bomb = ObjectPooler.SpawnFromPool("Frost Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<FrostBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectExplosiveBomb()
    {
        if (!PlayerProgression.CanAfford(GetTNTPrice()) || WaveController.Day < Settings.TNTUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        if (capacity <= 0) return;
        ExplosiveBomb bomb = ObjectPooler.SpawnFromPool("Explosive Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<ExplosiveBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectTurret()
    {
        if (!PlayerProgression.CanAfford(GetTurretPrice()) || WaveController.Day < Settings.TurretUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count;
        if (capacity <= 0) return;
        Turret turret = ObjectPooler.SpawnFromPool("Turret", Vector3.up * 100, Quaternion.identity).GetComponent<Turret>();
        turretPlacementController.Select(turret);
    }*/
}
