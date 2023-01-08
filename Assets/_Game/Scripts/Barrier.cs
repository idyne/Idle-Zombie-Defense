using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using FateGames;

public class Barrier : MonoBehaviour
{
    [SerializeField] private List<Transform> parts;
    private static int baseHealth = 300;
    private int currentHealth = 100;
    private HealthBar healthBar;
    public Transform Transform { get; private set; }
    public bool IsDead { get => currentHealth == 0; }
    public UnityEvent OnDeath { get; private set; } = new();
    public UnityEvent<int> OnHit { get; private set; } = new();
    public static int MaxHealth { get => GetMaxHealth(); }

    private int breakLevel = 0;
    private int damageTaken = 0;
    private void Awake()
    {
        Transform = transform;
        healthBar = GetComponentInChildren<HealthBar>();
        SetHealth(MaxHealth, false);
        healthBar.Hide();
    }

    private static int GetMaxHealth()
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
        if (WaveController.ZoneLevel == 3 && WaveController.NormalizedDay == 1)
            result *= 2;
        if (WaveController.CurrentTimePeriod == WaveController.TimePeriod.Night)
            result *= 3;
        return Mathf.CeilToInt(result);
    }

    private void SetHealth(int health, bool showBar = true)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(health, 0, MaxHealth);
        healthBar.SetPercent(currentHealth / (float)MaxHealth);
        if (showBar)
            healthBar.Show(4);
    }

    public void GetHit(int damage)
    {
        if (currentHealth <= 0) return;
        damageTaken += damage;
        SetHealth(currentHealth - damage);
        BreakOff();
        OnHit.Invoke(damage);
        if (currentHealth == 0) Die();
    }

    private void BreakOff()
    {
        float percent = currentHealth / (float)MaxHealth;
        if (percent < 0.75f && breakLevel < 1)
        {
            breakLevel = 1;
            parts[0].DOKill();
            parts[0].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", parts[0].position, parts[0].rotation);
        }
        if (percent < 0.50f && breakLevel < 2)
        {
            breakLevel = 2;
            parts[1].DOKill();
            parts[1].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", parts[1].position, parts[1].rotation);
        }
        if (percent < 0.25f && breakLevel < 3)
        {
            breakLevel = 3;
            parts[2].DOKill();
            parts[2].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", parts[2].position, parts[2].rotation);
        }
        if (percent <= 0.0f && breakLevel < 4)
        {
            breakLevel = 4;
            parts[3].DOKill();
            parts[3].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", parts[3].position, parts[3].rotation);
        }
    }

    private void Die()
    {
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    public void Repair()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            parts[i].DOScale(Vector3.one, 0.2f);
        }
        breakLevel = 0;
        SetHealth(MaxHealth, damageTaken > 0);
        damageTaken = 0;
    }
}
