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
    [SerializeField] private UIUpgradeButton baseDefenseButton, soldierMergeLevelButton;
    [SerializeField] private UIUpgradeButton tntUpgradeButton, frostUpgradeButton, barbwireUpgradeButton, turretUpgradeButton;
    [SerializeField] private UIPlacementButton frostButton, turretButton, tntButton, barbwireButton;
    [SerializeField] private PlacementController turretPlacementController, trapPlacementController;

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
        PauseButton.OnPause.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING) HideInWaveButtons(); });
        PauseButton.OnResume.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING) ShowInWaveButtons(); });
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            UpdateAllButtons();
        });
    }

    private void Start()
    {
        UpdateAllButtons();
    }

    public void UpdateAllButtons()
    {
        UpdateFireRateButton();
        UpdateBaseDefenseButton();
        /*UpdateTrapCapacityButton();
        UpdateTurretCapacityButton();*/
        UpdateTNTUpgradeButton();
        UpdateFrostUpgradeButton();
        UpdateBarbwireUpgradeButton();
        UpdateTurretUpgradeButton();
        UpdateSoldierMergeLevelButton();
        UpdateFrostButton();
        UpdateIncomeButton();
        UpdateMergeButton();
        UpdateSoldierButton();
        UpdateTNTButton();
        UpdateTurretButton();
        UpdateBarbwireButton();
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
        if (!PlayerProgression.CanAfford(cost) || maxedOut) baseDefenseButton.Deactivate();
        else if (!baseDefenseButton.Active)
        {
            baseButton.ShowNotification();
            baseDefenseButton.Activate();
        }
        baseDefenseButton.SetLevel(PlayerProgression.PlayerData.BaseDefenseLevel);
    }

    public void UpdateTNTUpgradeButton()
    {
        int cost = CostManager.GetTNTUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.TNTLevel >= LimitManager.GetTNTUpgradeLimit();
        if (!maxedOut)
            tntUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else tntUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) tntUpgradeButton.Deactivate();
        else if (!tntUpgradeButton.Active)
        {
            trapButton.ShowNotification();
            tntUpgradeButton.Activate();
        }
        tntUpgradeButton.SetLevel(PlayerProgression.PlayerData.TNTLevel);
    }
    public void UpdateFrostUpgradeButton()
    {
        int cost = CostManager.GetFrostUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.FrostLevel >= LimitManager.GetFrostUpgradeLimit();
        if (!maxedOut)
            frostUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else frostUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) frostUpgradeButton.Deactivate();
        else if (!frostUpgradeButton.Active)
        {
            trapButton.ShowNotification();
            frostUpgradeButton.Activate();
        }
        frostUpgradeButton.SetLevel(PlayerProgression.PlayerData.FrostLevel);
    }
    public void UpdateBarbwireUpgradeButton()
    {
        int cost = CostManager.GetBarbwireUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.BarbwireLevel >= LimitManager.GetBarbwireUpgradeLimit();
        if (!maxedOut)
            barbwireUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else barbwireUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) barbwireUpgradeButton.Deactivate();
        else if (!barbwireUpgradeButton.Active)
        {
            trapButton.ShowNotification();
            barbwireUpgradeButton.Activate();
        }
        barbwireUpgradeButton.SetLevel(PlayerProgression.PlayerData.BarbwireLevel);
    }
    public void UpdateTurretUpgradeButton()
    {
        int cost = CostManager.GetTurretUpgradePrice();
        bool maxedOut = PlayerProgression.PlayerData.TurretLevel >= LimitManager.GetTurretUpgradeLimit();
        if (!maxedOut)
            turretUpgradeButton.SetText(UIMoney.FormatMoney(cost));
        else turretUpgradeButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) turretUpgradeButton.Deactivate();
        else if (!turretUpgradeButton.Active)
        {
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
    /*
    public void UpdateTrapCapacityButton()
    {
        int cost = CostManager.GetTrapCapacityPrice();
        bool maxedOut = PlayerProgression.PlayerData.TrapCapacity >= LimitManager.GetTrapLimit();
        int unlockDay = Mathf.Min(FrostUnlockDay, TNTUnlockDay);
        bool locked = Day < unlockDay;
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            trapCapacityButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
        }
        else if (!maxedOut)
            trapCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else trapCapacityButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) trapCapacityButton.Deactivate();
        else if (!trapCapacityButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            trapCapacityButton.Activate();
        }
        trapCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TrapCapacity + 1);
    }*/
    /*
    public void UpdateTurretCapacityButton()
    {
        int cost = CostManager.GetTurretCapacityPrice();
        bool maxedOut = PlayerProgression.PlayerData.TurretCapacity >= LimitManager.GetTurretLimit();
        int unlockDay = TurretUnlockDay;
        bool locked = Day < unlockDay;
        if (locked)
        {
            int remainingDays = unlockDay - Day;
            turretCapacityButton.SetText("Unlocks " + (remainingDays > 1 ? "after " + remainingDays + " days" : "the next day"));
        }
        else
        if (!maxedOut)
            turretCapacityButton.SetText(UIMoney.FormatMoney(cost));
        else turretCapacityButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut || locked) turretCapacityButton.Deactivate();
        else if (!turretCapacityButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            turretCapacityButton.Activate();
        }
        turretCapacityLevelText.text = "Level " + (PlayerProgression.PlayerData.TurretCapacity + 1);
    }*/
    public void UpdateSoldierMergeLevelButton()
    {
        int cost = CostManager.GetSoldierMergeLevelPrice();
        bool maxedOut = PlayerProgression.PlayerData.SoldierMergeLevel >= LimitManager.GetSoldierMergeLimit();
        if (!maxedOut)
            soldierMergeLevelButton.SetText(UIMoney.FormatMoney(cost));
        else soldierMergeLevelButton.SetText("Max");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) soldierMergeLevelButton.Deactivate();
        else if (!soldierMergeLevelButton.Active)
        {
            baseButton.ShowNotification();
            soldierMergeLevelButton.Activate();
        }
        soldierMergeLevelButton.SetLevel(PlayerProgression.PlayerData.SoldierMergeLevel);
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
        // TODO
        return;
        turretButton.Show();
        frostButton.Show();
        tntButton.Show();
        barbwireButton.Show();
    }



}
