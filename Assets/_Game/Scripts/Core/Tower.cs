using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FateGames;
using static LevelManager;
public class Tower : MonoBehaviour
{
    private int currentHealth = 100;
    [SerializeField] private int towerWorldLevel = 1;
    [SerializeField] private int towerZoneLevel = 1;
    [SerializeField] private BaseSpotlight[] spotlights;
    [SerializeField] private Transform pointContainer;
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject seperateTower;
    [SerializeField] private List<Barrier> barriers;
    [SerializeField] private PlacementController turretPlacementController, trapPlacementController;
    [SerializeField] private Transform bombPointContainer;
    [SerializeField] private Transform zombieSpawnPointContainer;
    [SerializeField] private bool overrideZombieSpawnPoints = false;
    [SerializeField] private CameraController cameraController;
    private List<Transform> bombPoints = new();
    private List<Transform> zombieSpawnPoints = new();
    private List<Transform> _points = new();
    private bool pointsInitialized = false;
    private List<Transform> points
    {
        get
        {
            if (!pointsInitialized) InitializePoints();
            return _points;
        }
    }
    private UIHealthBar healthBar;
    public Transform Transform { get; private set; }
    private int MaxHealth { get => GetMaxHealth(); }
    public int NumberOfPoints { get => points.Count; }
    public int TowerWorldLevel { get => towerWorldLevel; }
    public int TowerZoneLevel { get => towerZoneLevel; }
    public PlacementController TurretPlacementController { get => turretPlacementController; }
    public PlacementController TrapPlacementController { get => trapPlacementController; }
    public List<Transform> BombPoints { get => bombPoints; }
    public bool OverrideZombieSpawnPoints { get => overrideZombieSpawnPoints; }
    public CameraController CameraController { get => cameraController; }

    private int damageTaken = 0;

    private void Awake()
    {
        for (int i = 0; i < bombPointContainer.childCount; i++)
            bombPoints.Add(bombPointContainer.GetChild(i));
        bombPoints.Sort((a, b) => Mathf.CeilToInt(a.position.z - b.position.z));
        if (overrideZombieSpawnPoints)
            for (int i = 0; i < zombieSpawnPointContainer.childCount; i++)
                zombieSpawnPoints.Add(zombieSpawnPointContainer.GetChild(i));
        TurnOffLights();
        Transform = transform;
        healthBar = GetComponentInChildren<UIHealthBar>();
        SetHealth(MaxHealth);
        healthBar.Hide();
        WaveController.Instance.OnNewWave.AddListener((wave) =>
        {
            wave.OnWaveClear.AddListener(Repair);
        });
        UpgradeController.OnBaseDefenseUpgrade.AddListener(() =>
        {
            SetHealth(MaxHealth);
        });
    }

    private void Start()
    {
        /*Debug.Log("b " + gameObject.name, this);
        WaveController.Instance.OnNewWave.AddListener((wave) =>
        {
            Debug.Log("a " + gameObject.name, this);
            wave.OnWaveClear.AddListener(Repair);
        });
        UpgradeController.OnBaseDefenseUpgrade.AddListener(() =>
        {
            SetHealth(MaxHealth);
        });*/
    }

    public Transform GetRandomZombieSpawnPoint()
    {
        return zombieSpawnPoints[Random.Range(0, zombieSpawnPoints.Count)];
    }

    public void Repair()
    {
        Debug.Log(gameObject.name, this);
        foreach (Barrier barrier in barriers)
            barrier.Repair();
        SetHealth(MaxHealth, damageTaken > 0);
        damageTaken = 0;
        HapticManager.DoHaptic();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void TurnOnLights()
    {
        return;
        for (int i = 0; i < spotlights.Length; i++)
        {
            spotlights[i].TurnOn();
        }
    }

    public void TurnOffLights()
    {
        for (int i = 0; i < spotlights.Length; i++)
        {
            spotlights[i].TurnOff();
        }
    }

    public void Explode()
    {
        tower.SetActive(false);
        seperateTower.SetActive(true);
        Rigidbody[] rbs = seperateTower.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(5, transform.position, 5, 1, ForceMode.Impulse);
        }
    }

    private int GetMaxHealth()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.TowerMaxHealth;
            case 2:
                return Settings.World2.TowerMaxHealth;
        }
        return 1;
    }

    private void InitializePoints()
    {
        for (int i = 0; i < pointContainer.childCount; i++)
            _points.Add(pointContainer.GetChild(i));
        pointsInitialized = true;
    }


    public Transform GetPoint(int index)
    {
        if (index < 0 || index >= points.Count) return null;
        return points[index];
    }

    public void GetHit(int damage)
    {
        if (currentHealth <= 0) return;
        damageTaken += damage;
        SetHealth(currentHealth - damage);
    }

    private void SetHealth(int health, bool showBar = true)
    {
        currentHealth = health;
        healthBar.SetPercent(currentHealth / (float)MaxHealth);
        if (showBar)
            healthBar.Show(2);
        if (health <= 0) Die();
    }

    private void Die()
    {
        WaveController.State = WaveController.WaveState.LOSE;
        FateGames.LevelManager.Instance.FinishLevel(false);
    }



}
