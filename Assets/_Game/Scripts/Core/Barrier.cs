using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using FateGames;
using static LevelManager;
public class Barrier : MonoBehaviour
{
    [SerializeField] private List<Transform> parts;
    private int currentHealth = 100;
    private UIHealthBar healthBar;
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
        healthBar = GetComponentInChildren<UIHealthBar>();
        SetHealth(MaxHealth, false);
        healthBar.Hide();
        UpgradeController.Instance.OnBaseDefenseUpgrade.AddListener(() =>
        {
            SetHealth(MaxHealth);
        });
    }

    private static int GetMaxHealth()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.BarrierMaxHealth;
            case 2:
                return Settings.World2.BarrierMaxHealth;
        }
        return 1;
    }

    private void SetHealth(int health, bool showBar = true)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(health, 0, MaxHealth);
        healthBar.SetPercent(currentHealth / (float)MaxHealth);
        if (showBar)
            healthBar.Show(2);
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
        if (percent < 0.66f && breakLevel < 1)
        {
            breakLevel = 1;
            parts[0].DOKill();
            parts[0].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", Transform.position + Vector3.up, Transform.rotation);
        }
        if (percent < 0.33f && breakLevel < 2)
        {
            breakLevel = 2;
            parts[1].DOKill();
            parts[1].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", Transform.position + Vector3.up, Transform.rotation);
        }
        if (percent <= 0.0f && breakLevel < 3)
        {
            breakLevel = 3;
            parts[2].DOKill();
            parts[2].DOScale(Vector3.zero, 0.2f);
            ObjectPooler.SpawnFromPool("Wood Effect", Transform.position + Vector3.up, Transform.rotation);
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
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].DOScale(Vector3.one, 0.2f);
        }
        breakLevel = 0;
        SetHealth(MaxHealth, damageTaken > 0);
        damageTaken = 0;
    }
}
