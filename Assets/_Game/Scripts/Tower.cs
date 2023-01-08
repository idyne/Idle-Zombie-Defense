using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Tower : MonoBehaviour
{
    private static Tower instance;
    public static Tower Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Tower>();
            return instance;
        }
    }
    private int baseHealth = 1000;
    private int currentHealth = 100;
    [SerializeField] private Transform pointContainer;
    [SerializeField] private Transform smallPointContainer;
    [SerializeField] private GameObject smallTower, bigTower;
    private List<Transform> points = new();
    private List<Transform> lessPoints = new();
    private HealthBar healthBar;
    public Transform Transform { get; private set; }
    private int MaxHealth { get => GetMaxHealth(); }
    public int NumberOfPoints { get => WaveController.ZoneLevel == 1 ? lessPoints.Count : points.Count; }
    private int damageTaken = 0;
    [SerializeField] private GameObject seperateSmallTower, seperateBigTower;

    private void Awake()
    {
        Transform = transform;
        healthBar = GetComponentInChildren<HealthBar>();
        SetHealth(MaxHealth);
        healthBar.Hide();
        InitializePoints();
    }

    private void Start()
    {
        SetTower();
        WaveController.Instance.OnNewWave.AddListener((wave) =>
        {
            wave.OnWaveClear.AddListener(() =>
            {
                SetHealth(MaxHealth, damageTaken > 0);
                damageTaken = 0;
            });
        });
    }

    public void Explode()
    {
        smallTower.SetActive(false);
        bigTower.SetActive(false);
        seperateSmallTower.SetActive(WaveController.ZoneLevel == 1);
        seperateBigTower.SetActive(WaveController.ZoneLevel != 1);
    }

    public void SetTower()
    {
        smallTower.SetActive(WaveController.ZoneLevel == 1);
        bigTower.SetActive(WaveController.ZoneLevel != 1);
    }

    private int GetMaxHealth()
    {
        float result = baseHealth;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = baseHealth * (WaveController.NormalizedDay);
                break;
            case 2:
                result = baseHealth * (WaveController.NormalizedDay);
                break;
            case 3:
                result = baseHealth * (WaveController.NormalizedDay);
                break;
            case 4:
                result = baseHealth * (WaveController.NormalizedDay);
                break;
        }
        if (WaveController.CurrentTimePeriod == WaveController.TimePeriod.Night)
            result *= 3;
        return Mathf.CeilToInt(result);
    }

    private void InitializePoints()
    {
        for (int i = 0; i < pointContainer.childCount; i++)
            points.Add(pointContainer.GetChild(i));
        for (int i = 0; i < smallPointContainer.childCount; i++)
            lessPoints.Add(smallPointContainer.GetChild(i));
    }


    public Transform GetPoint(int index)
    {
        List<Transform> pointList = WaveController.ZoneLevel == 1 ? lessPoints : points;
        if (index < 0 || index >= pointList.Count) return null;
        return pointList[index];
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
            healthBar.Show(4);
        if (health <= 0) Die();
    }

    private void Die()
    {
        LevelManager.Instance.FinishLevel(false);
    }

}
