using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FateGames;
using static Settings;
using System.Linq;

public class UIButtonManager : MonoBehaviour
{
    public static UIButtonManager Instance { get; private set; }
    [SerializeField] private UIButton soldierButton, fireRateButton, incomeButton, mergeButton, startButton, upgradesButton;
    [SerializeField] private GameObject inWaveButtons, outWaveButtons;
    [SerializeField] private UIBetweenPhase UIBetweenPhase;
    [SerializeField] private UIButton baseDefenseButton, trapCapacityButton, turretCapacityButton, soldierMergeLevelButton;
    [SerializeField] private TextMeshProUGUI baseDefenseLevelText, trapCapacityLevelText, turretCapacityLevelText, soldierMergeLevelText;
    [SerializeField] private UIPlacementButton frostButton, turretButton, tntButton;
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
        WaveController.Instance.OnWaveStart.AddListener(UpdateUpgradesButton);
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
        UpdateTrapCapacityButton();
        UpdateTurretCapacityButton();
        UpdateSoldierMergeLevelButton();
        UpdateFrostButton();
        UpdateIncomeButton();
        UpdateMergeButton();
        UpdateSoldierButton();
        UpdateTNTButton();
        UpdateTurretButton();
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
        upgradesButton.Hide();
    }

    public void ShowStartAndUpgradeButtons()
    {
        startButton.Show();
        if (WaveController.Day > 1)
            upgradesButton.Show();
    }

    public void UpdateFireRateButton()
    {
        int cost = CostManager.GetFireRateCost();
        bool isMaxedOut = PlayerProgression.PlayerData.FireRateLevel >= LimitManager.GetMaxFireRateLevel();
        if (!isMaxedOut)
            fireRateButton.SetText(UIMoney.FormatMoney(cost));
        else
            fireRateButton.SetText("Maxed Out");
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

    public void UpdateUpgradesButton()
    {
        if (WaveController.State == WaveController.WaveState.RUNNING) upgradesButton.Hide();
        else if (WaveController.State == WaveController.WaveState.WAITING && WaveController.Day > 1) upgradesButton.Show();
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
        else baseDefenseButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || maxedOut) baseDefenseButton.Deactivate();
        else if (!baseDefenseButton.Active)
        {
            UIBetweenPhase.ShowNotification();
            baseDefenseButton.Activate();
        }
        baseDefenseLevelText.text = "Level " + PlayerProgression.PlayerData.BaseDefenseLevel.ToString();
    }
    public void UpdateTrapCapacityButton()
    {
        int cost = CostManager.GetTrapCapacityPrice();
        bool maxedOut = PlayerProgression.PlayerData.TrapCapacity >= LimitManager.GetTrapLimit();
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
    public void UpdateTurretCapacityButton()
    {
        int cost = CostManager.GetTurretCapacityPrice();
        bool maxedOut = PlayerProgression.PlayerData.TurretCapacity >= LimitManager.GetTurretLimit();
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
    public void UpdateSoldierMergeLevelButton()
    {
        int cost = CostManager.GetSoldierMergeLevelPrice();
        bool maxedOut = PlayerProgression.PlayerData.SoldierMergeLevel >= LimitManager.GetSoldierMergeLimit();
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
            frostButton.SetText(UIMoney.FormatMoney(CostManager.GetFrostPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetFrostPrice()) || noCapacity || locked) frostButton.Deactivate();
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
            turretButton.SetText(UIMoney.FormatMoney(CostManager.GetTurretPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetTurretPrice()) || noCapacity || locked) turretButton.Deactivate();
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
            tntButton.SetText(UIMoney.FormatMoney(CostManager.GetTNTPrice()));
        if (!PlayerProgression.CanAfford(CostManager.GetTNTPrice()) || noCapacity || locked) tntButton.Deactivate();
        else tntButton.Activate();
    }

    public void SelectFrostBomb()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetFrostPrice()) || WaveController.Day < Settings.FrostUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        if (capacity <= 0) return;
        FrostBomb bomb = ObjectPooler.SpawnFromPool("Frost Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<FrostBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectExplosiveBomb()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetTNTPrice()) || WaveController.Day < Settings.TNTUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        if (capacity <= 0) return;
        ExplosiveBomb bomb = ObjectPooler.SpawnFromPool("Explosive Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<ExplosiveBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectTurret()
    {
        if (!PlayerProgression.CanAfford(CostManager.GetTurretPrice()) || WaveController.Day < Settings.TurretUnlockDay) return;
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count;
        if (capacity <= 0) return;
        Turret turret = ObjectPooler.SpawnFromPool("Turret", Vector3.up * 100, Quaternion.identity).GetComponent<Turret>();
        turretPlacementController.Select(turret);
    }

    public void HidePlacementButtons()
    {
        // TODO
        turretButton.Hide();
        frostButton.Hide();
        tntButton.Hide();
    }

    public void ShowPlacementButtons()
    {
        // TODO
        turretButton.Show();
        frostButton.Show();
        tntButton.Show();
    }

}
