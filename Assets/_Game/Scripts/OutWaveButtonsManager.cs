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
        print(Instance);
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

    public static int GetFrostPrice()
    {
        float result = PlayerProgression.PlayerData.Traps.Where(trapData => trapData.Item1 == 1).Count() * 100 + 5;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 2f;
                break;
            case 4:
                result *= 2f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }

    public static int GetTurretPrice()
    {
        float result = (PlayerProgression.PlayerData.Turrets.Count) * 800 + 200;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 2f;
                break;
            case 4:
                result *= 2f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }

    public static int GetTNTPrice()
    {
        float result = PlayerProgression.PlayerData.Traps.Where(trapData => trapData.Item1 == 0).Count() * 100 + 5;
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 2f;
                break;
            case 4:
                result *= 2f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
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
        if (!PlayerProgression.CanAfford(GetFrostPrice()) || WaveController.Day < 8) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 1).Count();
        if (capacity <= 0) return;
        FrostBomb bomb = ObjectPooler.SpawnFromPool("Frost Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<FrostBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectExplosiveBomb()
    {
        if (!PlayerProgression.CanAfford(GetTNTPrice()) || WaveController.Day < 8) return;
        int capacity = PlayerProgression.PlayerData.TrapCapacity - PlayerProgression.PlayerData.Traps.Where(data => data.Item1 == 0).Count();
        if (capacity <= 0) return;
        ExplosiveBomb bomb = ObjectPooler.SpawnFromPool("Explosive Bomb", Vector3.up * 100, Quaternion.identity).GetComponent<ExplosiveBomb>();
        trapPlacementController.Select(bomb);
    }
    public void SelectTurret()
    {
        if (!PlayerProgression.CanAfford(GetTurretPrice()) || WaveController.Day < 15) return;
        int capacity = PlayerProgression.PlayerData.TurretCapacity - PlayerProgression.PlayerData.Turrets.Count;
        if (capacity <= 0) return;
        Turret turret = ObjectPooler.SpawnFromPool("Turret", Vector3.up * 100, Quaternion.identity).GetComponent<Turret>();
        turretPlacementController.Select(turret);
    }
}
