using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FateGames;
using static Settings;
using System.Linq;
using static LevelManager;

public class UIButtonManager : MonoBehaviour
{
    public static UIButtonManager Instance { get; private set; }
    [SerializeField] private UIButton soldierButton, fireRateButton, incomeButton, mergeButton, startButton, baseUpgradesButton, trapUpgradesButton;
    [SerializeField] private GameObject inWaveButtons, outWaveButtons;
    [SerializeField] private UIBetweenPhase baseButton, trapButton;
    [SerializeField] private UIUpgradeButton baseDefenseButton, soldierMergeLevelButton, throwableWeaponsGuyButton, airstrikeLevelButton;
    [SerializeField] private UIUpgradeButton tntUpgradeButton, frostUpgradeButton, barbwireUpgradeButton, turretUpgradeButton;
    [SerializeField] private UIPlacementButton frostButton, turretButton, tntButton, barbwireButton;
    [SerializeField] private PlacementController turretPlacementController, trapPlacementController;
    private bool canAffordBaseDefenseUpgrade = false, canAffordSoldierMergeUpgrade = false, canAffordThrowableGuyUpgrade = false, canAffordAirstrikeUpgrade = false;
    private bool canAffordTNTUpgrade = false, canAffordFrostUpgrade = false, canAffordBarbwireUpgrade = false, canAffordTurretUpgrade = false;
    private bool notifyBaseUpgrades { get => canAffordBaseDefenseUpgrade || canAffordSoldierMergeUpgrade || canAffordThrowableGuyUpgrade || canAffordAirstrikeUpgrade; }
    private bool notifyTrapUpgrades { get => canAffordTNTUpgrade || canAffordFrostUpgrade || canAffordBarbwireUpgrade || canAffordTurretUpgrade; }

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        WaveController.Instance.OnWaveStart.AddListener(SwitchInWave);
        WaveController.Instance.OnWaveStart.AddListener(UpdateTrapUpgradesButton);
        WaveController.Instance.OnWaveStart.AddListener(UpdateBaseUpgradesButton);
        WaveController.Instance.OnWaveEnd.AddListener(HideInWaveButtons);
        UpgradeController.OnSoldierMergeLevelUpgrade.AddListener((level) => { UpdateSoldierButton(); UpdateMergeButton(); });
        PauseButton.OnPause.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING) HideInWaveButtons(); });
        PauseButton.OnResume.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING) ShowInWaveButtons(); });
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            UpdateAllButtonsExceptUpgrades();
        });
        PlayerProgression.OnUpgradePointChanged.AddListener((money, change) =>
        {
            UpdateUpgradeButtons();
        });
        WaveController.Instance.OnWaveEnd.AddListener(UpdateUpgradeButtons);
    }

    private void Start()
    {
        UpdateAllButtonsExceptUpgrades();
        UpdateUpgradeButtons();
    }

    public void UpdateAllButtonsExceptUpgrades()
    {
        UpdateFireRateButton();
        UpdateFrostButton();
        UpdateIncomeButton();
        UpdateMergeButton();
        UpdateSoldierButton();
        UpdateTNTButton();
        UpdateTurretButton();
        UpdateBarbwireButton();
    }

    public void UpdateUpgradeButtons()
    {
        UpdateBaseDefenseButton();
        UpdateTNTUpgradeButton();
        UpdateFrostUpgradeButton();
        UpdateBarbwireUpgradeButton();
        UpdateTurretUpgradeButton();
        UpdateSoldierMergeLevelButton();
        UpdateThrowableWeaponsGuyUpgradeButton();
        UpdateAirstrikeUpgradeButton();
    }

    public void SwitchInWave()
    {
        HideOutWaveButtons();
        ShowInWaveButtons();
    }
    public void SwitchOutWave()
    {
        HideInWaveButtons();
        ShowOutWaveButtons();
    }

    public void ShowInWaveButtons() => inWaveButtons.SetActive(true);
    public void HideInWaveButtons() => inWaveButtons.SetActive(false);
    public void ShowOutWaveButtons() => outWaveButtons.SetActive(true);
    public void HideOutWaveButtons() => outWaveButtons.SetActive(false);

    public void UpdateSoldierButton()
    {
        int cost = CostManager.GetSoldierCost();
        bool isFull = Barracks.Instance.Full;
        bool isMerging = Barracks.Instance.Merging;
        if (!isFull)
            soldierButton.SetText(UIMoney.FormatMoney(cost));
        else soldierButton.SetText("Full");
        if (!PlayerProgression.CanAfford(cost) || isFull || isMerging) soldierButton.Deactivate();
        else soldierButton.Activate();
    }

    public void HideStartAndUpgradeButtons()
    {
        startButton.Hide();
        baseUpgradesButton.Hide();
        trapUpgradesButton.Hide();
    }

    public void ShowStartAndUpgradeButtons()
    {
        startButton.Show();
        if (Day > 1)
        {
            baseUpgradesButton.Show();
            trapUpgradesButton.Show();
        }
    }

    public void UpdateFireRateButton()
    {
        int cost = CostManager.GetFireRateCost();
        bool isMaxedOut = PlayerProgression.PlayerData.FireRateLevel >= LimitManager.GetMaxFireRateLevel();
        if (!isMaxedOut)
            fireRateButton.SetText(UIMoney.FormatMoney(cost));
        else
            fireRateButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || isMaxedOut) fireRateButton.Deactivate();
        else fireRateButton.Activate();
    }

    public void UpdateIncomeButton()
    {
        int cost = CostManager.GetIncomeCost();
        incomeButton.SetText(UIMoney.FormatMoney(cost));
        if (!PlayerProgression.CanAfford(cost)) incomeButton.Deactivate();
        else incomeButton.Activate();
    }

    public void UpdateStartButton()
    {
        if (WaveController.State == WaveController.WaveState.RUNNING) startButton.Hide();
        else if (WaveController.State == WaveController.WaveState.WAITING) startButton.Show();
    }

    public void UpdateBaseUpgradesButton()
    {
        if (WaveController.State == WaveController.WaveState.RUNNING)
            baseUpgradesButton.Hide();
        else if (WaveController.State == WaveController.WaveState.WAITING)
            baseUpgradesButton.Show();
    }

    public void UpdateTrapUpgradesButton()
    {
        if (WaveController.State == WaveController.WaveState.RUNNING)
            trapUpgradesButton.Hide();
        else if (WaveController.State == WaveController.WaveState.WAITING && Day > 1)
            trapUpgradesButton.Show();
    }

    public void UpdateMergeButton()
    {
        int cost = CostManager.GetMergeCost();
        bool canMerge = Barracks.Instance.CanMerge(out _);
        mergeButton.SetText(UIMoney.FormatMoney(cost));
        if (!canMerge) mergeButton.Hide();
        else mergeButton.Show();
        if (!PlayerProgression.CanAfford(cost)) mergeButton.Deactivate();
        else mergeButton.Activate();
    }

    public void UpdateBaseDefenseButton()
    {
        int cost = CostManager.GetBaseDefenseLevelPrice();
        bool maxedOut = PlayerProgression.PlayerData.BaseDefenseLevel >= LimitManager.GetBaseDefenseLimit();
        if (!maxedOut)
            baseDefenseButton.SetText(UIMoney.FormatMoney(cost));
        else baseDefenseButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            baseDefenseButton.Deactivate();
            canAffordBaseDefenseUpgrade = false;
            if (!notifyBaseUpgrades)
                baseButton.HideNotification();
        }
        else if (!baseDefenseButton.Active)
        {
            canAffordBaseDefenseUpgrade = true;
            baseButton.ShowNotification();
            baseDefenseButton.Activate();
        }
        baseDefenseButton.SetLevel(PlayerProgression.PlayerData.BaseDefenseLevel);
    }

    public void UpdateSoldierMergeLevelButton()
    {
        int cost = CostManager.GetSoldierMergeLevelPrice();
        bool maxedOut = PlayerProgression.PlayerData.SoldierMergeLevel >= LimitManager.GetSoldierMergeLimit();
        if (!maxedOut)
            soldierMergeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else soldierMergeLevelButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            soldierMergeLevelButton.Deactivate();
            canAffordSoldierMergeUpgrade = false;
            if (!notifyBaseUpgrades)
                baseButton.HideNotification();
        }
        else if (!soldierMergeLevelButton.Active)
        {
            canAffordSoldierMergeUpgrade = true;
            baseButton.ShowNotification();
            soldierMergeLevelButton.Activate();
        }
        soldierMergeLevelButton.SetLevel(PlayerProgression.PlayerData.SoldierMergeLevel);
    }

    public void UpdateThrowableWeaponsGuyUpgradeButton()
    {
        int cost = CostManager.GetThrowableWeaponsGuyLevelPrice();
        bool maxedOut = PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel >= LimitManager.GetThrowableWeaponsGuyLevelLimit();
        bool isLocked = Settings.ThrowableWeaponsUnlockDay > Day;
        if (isLocked)
        {
            throwableWeaponsGuyButton.Lock(Settings.ThrowableWeaponsUnlockDay);
            return;
        }
        if (!maxedOut)
            throwableWeaponsGuyButton.SetText(UIMoney.FormatMoney(cost));
        else throwableWeaponsGuyButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            throwableWeaponsGuyButton.Deactivate();
            canAffordThrowableGuyUpgrade = false;
            if (!notifyBaseUpgrades)
                baseButton.HideNotification();
        }
        else if (!throwableWeaponsGuyButton.Active)
        {
            canAffordThrowableGuyUpgrade = true;
            baseButton.ShowNotification();
            throwableWeaponsGuyButton.Activate();
        }
        throwableWeaponsGuyButton.SetLevel(PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel);
    }
    public void UpdateAirstrikeUpgradeButton()
    {
        int cost = CostManager.GetAirstrikePrice();
        bool maxedOut = PlayerProgression.PlayerData.AirstrikeLevel >= LimitManager.GetAirstrikeLevelLimit();
        bool isLocked = Settings.AirstrikeUnlockDay > Day;
        if (isLocked)
        {
            airstrikeLevelButton.Lock(Settings.AirstrikeUnlockDay);
            return;
        }
        if (!maxedOut)
            airstrikeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else airstrikeLevelButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            airstrikeLevelButton.Deactivate();
            canAffordAirstrikeUpgrade = false;
            if (!notifyBaseUpgrades)
                baseButton.HideNotification();
        }
        else if (!airstrikeLevelButton.Active)
        {
            canAffordAirstrikeUpgrade = true;
            baseButton.ShowNotification();
            airstrikeLevelButton.Activate();
        }
        airstrikeLevelButton.SetLevel(PlayerProgression.PlayerData.AirstrikeLevel);
    }

    public void UpdateTNTUpgradeButton()
    {
        int cost = CostManager.GetTNTUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.TNTLevel >= LimitManager.GetTNTUpgradeLimit();
        bool isLocked = Settings.TNTUnlockDay > Day;
        if (isLocked)
        {
            tntUpgradeButton.Lock(Settings.TNTUnlockDay);
            return;
        }
        if (!maxedOut)
            tntUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else tntUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            tntUpgradeButton.Deactivate();
            canAffordTNTUpgrade = false;
            if (!notifyTrapUpgrades)
                trapButton.HideNotification();
        }
        else if (!tntUpgradeButton.Active)
        {
            canAffordTNTUpgrade = true;
            trapButton.ShowNotification();
            tntUpgradeButton.Activate();
        }
        tntUpgradeButton.SetLevel(PlayerProgression.PlayerData.TNTLevel);
    }
    public void UpdateFrostUpgradeButton()
    {
        int cost = CostManager.GetFrostUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.FrostLevel >= LimitManager.GetFrostUpgradeLimit();
        bool isLocked = Settings.FrostUnlockDay > Day;
        if (isLocked)
        {
            frostUpgradeButton.Lock(Settings.FrostUnlockDay);
            return;
        }
        if (!maxedOut)
            frostUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else frostUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            canAffordFrostUpgrade = false;
            frostUpgradeButton.Deactivate();
            if (!notifyTrapUpgrades)
                trapButton.HideNotification();
        }
        else if (!frostUpgradeButton.Active)
        {
            canAffordFrostUpgrade = true;
            trapButton.ShowNotification();
            frostUpgradeButton.Activate();
        }
        frostUpgradeButton.SetLevel(PlayerProgression.PlayerData.FrostLevel);
    }
    public void UpdateBarbwireUpgradeButton()
    {
        int cost = CostManager.GetBarbwireUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.BarbwireLevel >= LimitManager.GetBarbwireUpgradeLimit();
        bool isLocked = Settings.BarbwireUnlockDay > Day;
        if (isLocked)
        {
            barbwireUpgradeButton.Lock(Settings.BarbwireUnlockDay);
            return;
        }
        if (!maxedOut)
            barbwireUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else barbwireUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            canAffordBarbwireUpgrade = false;
            barbwireUpgradeButton.Deactivate();
            if (!notifyTrapUpgrades)
                trapButton.HideNotification();
        }
        else if (!barbwireUpgradeButton.Active)
        {
            canAffordBarbwireUpgrade = true;
            trapButton.ShowNotification();
            barbwireUpgradeButton.Activate();
        }
        barbwireUpgradeButton.SetLevel(PlayerProgression.PlayerData.BarbwireLevel);
    }
    public void UpdateTurretUpgradeButton()
    {
        int cost = CostManager.GetTurretUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.TurretLevel >= LimitManager.GetTurretUpgradeLimit();
        bool isLocked = Settings.TurretUnlockDay > Day;
        if (isLocked)
        {
            turretUpgradeButton.Lock(Settings.TurretUnlockDay);
            return;
        }
        if (!maxedOut)
            turretUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else turretUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAffordUpgrade(cost) || maxedOut)
        {
            canAffordTurretUpgrade = false;
            turretUpgradeButton.Deactivate();
            if (!notifyTrapUpgrades)
                trapButton.HideNotification();
        }
        else if (!turretUpgradeButton.Active)
        {
            canAffordTurretUpgrade = true;
            trapButton.ShowNotification();
            turretUpgradeButton.Activate();
        }
        turretUpgradeButton.SetLevel(PlayerProgression.PlayerData.TurretLevel);
    }

    public void HandleBaseDefenseUpgradeButton()
    {
        UpgradeController.UpgradeBaseDefense();
    }
    public void HandleSoldierLevelUpgradeButton()
    {
        UpgradeController.UpgradeSoldierMergeLevel();
    }
    public void HandleThrowableWeaponsGuyUpgradeButton()
    {
        UpgradeController.UpgradeThrowableWeaponsGuy();
    }
    public void HandleAirstrikeUpgradeButton()
    {
        UpgradeController.UpgradeAirstrike();
    }
    public void HandleTNTUpgradeButton()
    {
        UpgradeController.UpgradeTNT();
    }
    public void HandleFrostUpgradeButton()
    {
        UpgradeController.UpgradeFrost();
    }
    public void HandleBarbwireUpgradeButton()
    {
        UpgradeController.UpgradeBarbwire();
    }
    public void HandleTurretUpgradeButton()
    {
        UpgradeController.UpgradeTurret();
    }


    public void UpdateFrostButton()
    {
        int unlockDay = FrostUnlockDay;
        bool locked = Day < unlockDay;
        if (locked)
        {
            frostButton.Hide();
            return;
        }
        else frostButton.Show();
        int limit = LimitManager.GetFrostLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 1).Count();
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            frostButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            frostButton.HideCapacity();
        }
        else if (noCapacity)
            frostButton.SetText("No Capacity");
        else
            frostButton.SetText(UIMoney.FormatMoney(CostManager.GetFrostPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetFrostPrice()) || noCapacity || locked) frostButton.Deactivate();
        else frostButton.Activate();
    }

    public void UpdateTurretButton()
    {
        int unlockDay = TurretUnlockDay;
        bool locked = Day < unlockDay;
        if (locked)
        {
            turretButton.Hide();
            return;
        }
        else turretButton.Show();
        int limit = LimitManager.GetTurretLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Turrets.Count;
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            turretButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            turretButton.HideCapacity();
        }
        else if (noCapacity)
            turretButton.SetText("No Capacity");
        else
            turretButton.SetText(UIMoney.FormatMoney(CostManager.GetTurretPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetTurretPrice()) || noCapacity || locked) turretButton.Deactivate();
        else turretButton.Activate();
    }

    public void UpdateTNTButton()
    {
        int unlockDay = TNTUnlockDay;
        bool locked = Day < unlockDay;
        if (locked)
        {
            tntButton.Hide();
            return;
        }
        else tntButton.Show();
        int limit = LimitManager.GetTNTLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 0).Count();
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            tntButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            tntButton.HideCapacity();
        }
        else if (noCapacity)
            tntButton.SetText("No Capacity");
        else
            tntButton.SetText(UIMoney.FormatMoney(CostManager.GetTNTPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetTNTPrice()) || noCapacity || locked) tntButton.Deactivate();
        else tntButton.Activate();
    }

    public void UpdateBarbwireButton()
    {
        int unlockDay = BarbwireUnlockDay;
        bool locked = Day < unlockDay;
        if (locked)
        {
            barbwireButton.Hide();
            return;
        }
        else barbwireButton.Show();
        int limit = LimitManager.GetBarbwireLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 2).Count();
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            barbwireButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
            barbwireButton.HideCapacity();
        }
        else if (noCapacity)
            barbwireButton.SetText("No Capacity");
        else
            barbwireButton.SetText(UIMoney.FormatMoney(CostManager.GetTNTPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetBarbwirePrice()) || noCapacity || noCapacity || locked) barbwireButton.Deactivate();
        else barbwireButton.Activate();
    }

    public void SelectFrostBomb()
    {

        if (!PlayerProgression.CanAfford(CostManager.GetFrostPrice()) || Day < Settings.FrostUnlockDay) return;
        int limit = LimitManager.GetFrostLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 1).Count();
        if (noCapacity) return;
        FrostBomb bomb = ObjectPooler.SpawnFromPool("Frost Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<FrostBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectBarbwire()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetBarbwirePrice()) || Day < Settings.BarbwireUnlockDay) return;
        int limit = LimitManager.GetBarbwireLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 2).Count();
        if (noCapacity) return;
        Barbwire barbwire = ObjectPooler.SpawnFromPool("Barbwire", Vector3.up * 100, Quaternion.identity).GetComponent<Barbwire>();
        trapPlacementController.Select(barbwire);
    }
    public void SelectExplosiveBomb()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetTNTPrice()) || Day < Settings.TNTUnlockDay) return;
        int limit = LimitManager.GetTNTLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Traps.Where(trap => trap.Item1 == 0).Count();
        if (noCapacity) return;
        ExplosiveBomb bomb = ObjectPooler.SpawnFromPool("Explosive Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<ExplosiveBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectTurret()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetTurretPrice()) || Day < Settings.TurretUnlockDay) return;
        int limit = LimitManager.GetTurretLimit();
        bool noCapacity = limit <= PlayerProgression.PlayerData.Turrets.Count;
        if (noCapacity) return;
        Turret turret = ObjectPooler.SpawnFromPool("Turret", Vector3.up * 100, Quaternion.identity).GetComponent<Turret>();
        turretPlacementController.Select(turret);
    }

    public void HidePlacementButtons()
    {
        // TODO
        turretButton.Hide();
        frostButton.Hide();
        tntButton.Hide();
        barbwireButton.Hide();
    }

    public void ShowPlacementButtons()
    {
        UpdateTurretButton();
        UpdateTNTButton();
        UpdateBarbwireButton();
        UpdateFrostButton();
    }



}
