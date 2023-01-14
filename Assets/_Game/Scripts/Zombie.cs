using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FateGames;
using UnityEngine.Events;
using DG.Tweening;
using FSG.MeshAnimator;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IPooledObject
{
    public Transform Transform { get; private set; }
    private NavMeshAgent agent;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private int level = 1;
    private static int baseHealth = 25;
    private int maxHealth { get => GetMaxHealth(); }
    [SerializeField] private int matIndex = 0;
    [SerializeField] private float hitCooldownDuration = 1f;
    private static int baseDamage = 25;
    private int damage { get => GetDamage(); }
    [SerializeField] private Transform shotPoint, levitatingTextPoint;
    [SerializeField] private Color slowedDownColor;
    private HealthBar healthBar;
    private MeshAnimatorBase meshAnimator;
    private int currentHealth = 100;
    private List<Barrier> barriers = new();
    public UnityEvent OnDeath { get; private set; } = new();
    public Transform ShotPoint { get => shotPoint; }

    private bool canHit = true;
    private MeshRenderer rend;
    private Color originalColor;
    private Tower tower;
    private bool isStopped = false;
    private bool slowedDown = false;
    private Tween slowDownTween = null;

    private void Awake()
    {
        Transform = transform;
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<HealthBar>();
        meshAnimator = GetComponentInChildren<MeshAnimatorBase>();
        rend = GetComponentInChildren<MeshRenderer>();
        if (!isBoss)
            originalColor = rend.material.color;
        else
            originalColor = rend.materials[1].color;
        healthBar.Hide();
    }

    public void SetLevel(int level)
    {
        this.level = level;
        SetHealth(maxHealth, false);
    }

    public void Stop()
    {
        //agent.enabled = false;
        agent.isStopped = false;
        agent.SetDestination(Vector3.zero);
        meshAnimator.Play(0);
        agent.speed = 1f;
        isStopped = true;
        tower = null;
        barriers.Clear();
    }

    public void SlowDown()
    {
        if (slowedDown) return;
        if (slowDownTween != null)
        {
            slowDownTween.Kill();
            slowDownTween = null;
        }
        slowedDown = true;
        agent.speed *= 0.2f;
        meshAnimator.speed *= 0.2f;
        hitCooldownDuration *= 5;
        SetColor(slowedDownColor);
        slowDownTween = DOVirtual.DelayedCall(30, CancelSlowDown);
    }

    public void CancelSlowDown()
    {
        if (!slowedDown) return;
        if (slowDownTween != null)
        {
            slowDownTween.Kill();
            slowDownTween = null;
        }
        slowedDown = false;
        agent.speed *= 5f;
        meshAnimator.speed *= 5f;
        hitCooldownDuration *= 0.2f;
        SetColor(originalColor);
    }


    private int GetDamage()
    {
        int bossMultiplier = 4;
        float normalizedDayMultiplier = (WaveController.NormalizedDay - 1) * 0.8f;
        if (WaveController.NormalizedDay >= 3)
            normalizedDayMultiplier *= 0.5f;
        float result = baseDamage * (level + normalizedDayMultiplier);
        switch (WaveController.ZoneLevel)
        {
            case 2:
                result *= 1.0f;
                break;
            case 3:
                result *= 1.0f;
                break;
            case 4:
                result *= 1.0f;
                break;
        }

        if (isBoss)
        {
            result *= bossMultiplier * 0.8f;
            if (WaveController.ZoneLevel == 2)
            {
                result *= 2;
                if (WaveController.NormalizedDay == 14)
                {
                    result *= 0.7f;
                }
            }
            else if (WaveController.ZoneLevel == 3)
            {
                result *= 2;
                if (WaveController.NormalizedDay == 20)
                {
                    result *= 0.7f;
                }
            }
            else if (WaveController.ZoneLevel == 4)
            {
                result *= 2;
                if (WaveController.NormalizedDay == 30)
                {
                    result *= 0.7f;
                }
            }

        }
        return Mathf.CeilToInt(result);
    }

    private int GetMaxHealth()
    {
        int bossMultiplier = 8;
        float normalizedDayMultiplier = (WaveController.NormalizedDay - 1) * 0.8f;
        if (WaveController.NormalizedDay >= 3)
            normalizedDayMultiplier *= 0.5f;
        float result = baseHealth * (level + normalizedDayMultiplier);
        switch (WaveController.ZoneLevel)
        {
            case 2:
                result *= 1.0f;
                break;
            case 3:
                result *= 1.2f;
                break;
            case 4:
                result *= 1.2f;
                break;
        }
        if (isBoss)
        {
            result *= bossMultiplier * 1.5f;
            if (WaveController.ZoneLevel == 2)
                result *= 2;
            else if (WaveController.ZoneLevel == 3)
                result *= 1.3f;
            else if (WaveController.ZoneLevel == 4)
                result *= 1.2f;
        }
        if (WaveController.ZoneLevel == 2 && WaveController.NormalizedDay > 5)
        {
            result *= 1.3f + (WaveController.NormalizedDay - 5) / 9f * 2f;
        }
        else if (WaveController.ZoneLevel == 2 && WaveController.NormalizedDay <= 5)
        {
            result *= 1.3f + (WaveController.NormalizedDay - 5) / 9f * 2f;
            result *= 1.3f;
        }
        else if (WaveController.ZoneLevel == 3 && WaveController.NormalizedDay > 5)
        {
            result *= 1.3f + (WaveController.NormalizedDay - 5) / 9f * 2f;
        }
        else if (WaveController.ZoneLevel == 4 && WaveController.NormalizedDay > 5)
        {
            result *= 1.3f + (WaveController.NormalizedDay - 5) / 9f * 2f;
        }
        if (WaveController.NormalizedDay > 1)
            result *= 1.35f;
        return Mathf.CeilToInt(result);
    }

    private int GetGain()
    {
        float gain = (((PlayerProgression.PlayerData.IncomeLevel - 1) / 3f) + (WaveController.NormalizedDay) * 2) * level;
        switch (WaveController.ZoneLevel)
        {
            case 2:
                gain *= 2.5f;
                break;
            case 3:
                gain *= 3.5f;
                break;
            case 4:
                gain *= 3.5f;
                break;
        }
        if (isBoss)
            gain *= 2f;
        return Mathf.CeilToInt(gain);
    }

    private void DropMoney()
    {
        int gain = GetGain();

        int numberOfCoins = level;
        if (isBoss) numberOfCoins *= 10;
        CoinBurster.Burst(gain, numberOfCoins, Transform.position, Vector3.up, isBoss ? 10 : 7, isBoss);
        LevitatingText levitatingText = ObjectPooler.SpawnFromPool("Levitating Text", levitatingTextPoint.position, Quaternion.identity).GetComponent<LevitatingText>();
        levitatingText.SetText("$" + gain);
    }

    public void GetHit(Projectile projectile)
    {
        if (currentHealth <= 0 || !gameObject.activeSelf) return;
        Flash(0.05f);
        if (!isBoss)
            GetPushed(projectile.Damage / (float)maxHealth);
        SetHealth(currentHealth - projectile.Damage);
    }
    public void GetHit(int damage)
    {
        if (currentHealth <= 0 || !gameObject.activeSelf) return;
        Flash(0.05f);
        if (!isBoss)
            GetPushed(damage / (float)maxHealth);
        SetHealth(currentHealth - damage);
    }

    private void GetPushed(float value)
    {
        if (!agent.enabled) return;
        value = Mathf.Clamp(value, 0, 1);
        agent.Move(Transform.position.normalized * value);
    }
    private void Die()
    {
        DropMoney();
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
        Deactivate();
        ObjectPooler.SpawnFromPool(isBoss ? "Dying Zombie Boss" : ("Dying Zombie " + level), Transform.position, Transform.rotation);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        SetHealth(maxHealth, false);
        agent.SetDestination(Vector3.zero);
        CancelSlowDown();
        SetColor(originalColor);
    }
    private void SetHealth(int health, bool showBar = true)
    {

        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        healthBar.SetPercent(currentHealth / (float)maxHealth);
        if (showBar)
            healthBar.Show(2f);
        if (currentHealth == 0) Die();
    }

    private void HitTower(Tower tower)
    {
        if (!canHit) return;
        this.tower = tower;
        tower.GetHit(damage);
        canHit = false;
        DOVirtual.DelayedCall(hitCooldownDuration, () => { canHit = true; });
    }
    private void HitBarrier(Barrier barrier)
    {
        if (!canHit) return;
        barrier.GetHit(damage);
        canHit = false;
        DOVirtual.DelayedCall(hitCooldownDuration, () => { canHit = true; });
    }

    private void OnTriggerStay(Collider other)
    {
        if (isStopped) return;
        if (other.CompareTag("Tower"))
        {
            Tower tower = other.GetComponent<Tower>();
            agent.isStopped = true;
            Transform.LookAt(tower.Transform);
            meshAnimator.Play(1);
            HitTower(tower);
        }
        else if (other.CompareTag("Barrier"))
        {
            Barrier barrier = other.GetComponent<Barrier>();
            if (barrier.IsDead) return;
            if (!barriers.Contains(barrier))
            {
                barriers.Add(barrier);
                barrier.OnDeath.AddListener(() =>
                {
                    if (barriers.Remove(barrier))
                    {
                        if (gameObject.activeSelf)
                        {
                            agent.isStopped = false;
                            meshAnimator.Play(0);
                        }
                    }
                });
            }
            agent.isStopped = true;
            Transform.LookAt(barrier.Transform);
            meshAnimator.Play(1);
            HitBarrier(barrier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isStopped) return;
        if (other.CompareTag("Tower"))
        {
            Tower tower = other.GetComponent<Tower>();
            if (tower == this.tower)
            {
                agent.isStopped = false;
                meshAnimator.Play(0);
            }
        }
        else if (other.CompareTag("Barrier"))
        {
            Barrier barrier = other.GetComponent<Barrier>();
            if (barriers.Contains(barrier))
            {
                barriers.Remove(barrier);
                agent.isStopped = false;
                meshAnimator.Play(0);
            }
        }
    }

    private void Flash(float duration)
    {
        StartCoroutine(FlashCoroutine(duration));
    }
    private IEnumerator FlashCoroutine(float duration)
    {
        SetColor(Color.white);
        yield return new WaitForSeconds(duration);
        SetColor(slowedDown ? slowedDownColor : originalColor);
    }

    private void SetColor(Color color)
    {
        Material material;
        material = rend.materials[matIndex];
        material.color = color;
        rend.materials[matIndex] = material;

    }


}
