using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using System.Linq;

public class OutWaveButtonsManager : MonoBehaviour
{
    private static OutWaveButtonsManager instance = null;
    public static OutWaveButtonsManager Instance { get { if (instance == null) instance = FindObjectOfType<OutWaveButtonsManager>(); return instance; } }
    [SerializeField] private GameObject container;
    [SerializeField] private UIOutWaveButton frostButton, turretButton, tntButton;
    [SerializeField] private PlacementController turretPlacementController, trapPlacementController;

    private void Awake()
    {
        PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateFrostButton());
        PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateTurretButton());
        PlayerProgression.OnMoneyChanged.AddListener((money, change) => UpdateTNTButton());
    }
    private void Start()
    {
        UpdateFrostButton();
        UpdateTurretButton();
        UpdateTNTButton();
    }
    public void Hide()
    {
        container.SetActive(false);
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public int GetFrostPrice()
    {
        return 5;
    }

    public int GetTurretPrice()
    {
        return 5;
    }

    public int GetTNTPrice()
    {
        return 5;
    }

    public void UpdateFrostButton()
    {
        bool locked = WaveController.Day <= 7;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        frostButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
            frostButton.SetText("Unlocks after 1st week");
        else if (noCapacity)
            frostButton.SetText("No Capacity");
        else
            frostButton.SetText(UIMoney.FormatMoney(GetFrostPrice()));
        if (!PlayerProgression.CanAfford(GetFrostPrice()) || noCapacity || locked) frostButton.Deactivate();
        else frostButton.Activate();
    }

    public void UpdateTurretButton()
    {
        bool locked = WaveController.Day <= 14;
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count();
        turretButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
            turretButton.SetText("Unlocks after 2nd week");
        else if (noCapacity)
            turretButton.SetText("No Capacity");
        else
            turretButton.SetText(UIMoney.FormatMoney(GetTurretPrice()));
        if (!PlayerProgression.CanAfford(GetTurretPrice()) || noCapacity || locked) turretButton.Deactivate();
        else turretButton.Activate();
    }

    public void UpdateTNTButton()
    {
        bool locked = WaveController.Day <= 7;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        tntButton.SetCapacityText(capacity.ToString());
        bool noCapacity = capacity <= 0;
        if (locked)
            tntButton.SetText("Unlocks after 1st week");
        else if (noCapacity)
            tntButton.SetText("No Capacity");
        else
            tntButton.SetText(UIMoney.FormatMoney(GetTNTPrice()));
        if (!PlayerProgression.CanAfford(GetTNTPrice()) || noCapacity || locked) tntButton.Deactivate();
        else tntButton.Activate();
    }

    public void SelectFrostBomb()
    {
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        if (capacity <= 0) return;
        FrostBomb bomb = ObjectPooler.SpawnFromPool("Frost Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<FrostBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectExplosiveBomb()
    {
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        if (capacity <= 0) return;
        ExplosiveBomb bomb = ObjectPooler.SpawnFromPool("Explosive Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<ExplosiveBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectTurret()
    {
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count;
        if (capacity <= 0) return;
        Turret turret = ObjectPooler.SpawnFromPool("Turret", Vector3.up * 100, Quaternion.identity).GetComponent<Turret>();
        turretPlacementController.Select(turret);
    }
}
