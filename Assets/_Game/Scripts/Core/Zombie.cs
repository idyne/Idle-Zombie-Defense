using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FateGames;
using UnityEngine.Events;
using DG.Tweening;
using static LevelManager;
using FSG.MeshAnimator;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IPooledObject
{
    public Transform Transform { get; private set; }
    private NavMeshAgent agent;
    [SerializeField] private float speed = 1;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private int level = 1;
    private int maxHealth { get => GetMaxHealth(); }
    [SerializeField] private int matIndex = 0;
    [SerializeField] private float hitCooldownDuration = 1f;
    private int damage { get => GetDamage(); }
    [SerializeField] private Transform shotPoint, levitatingTextPoint;
    [SerializeField] private Color slowedDownColor;
    private UIHealthBar healthBar;
    private MeshAnimatorBase meshAnimator;
    private int currentHealth = 100;
    private List<Barrier> barriers = new();
    public UnityEvent OnDeath { get; private set; } = new();
    public UnityEvent OnSpawn { get; private set; } = new();
    public Transform ShotPoint { get => shotPoint; }

    private bool canHit = true;
    private MeshRenderer rend;
    private Color originalColor;
    private Tower tower;
    private bool isStopped = false;
    private bool slowedDown = false;
    private Tween slowDownTween = null;
    private bool died = false;
    public List<string> logs = new();
    private float slowDownMultiplier = 1;

    private void Awake()
    {
        Transform = transform;
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        healthBar = GetComponentInChildren<UIHealthBar>();
        meshAnimator = GetComponentInChildren<MeshAnimatorBase>();
        rend = GetComponentInChildren<MeshRenderer>();
        if (!isBoss)
            originalColor = rend.material.color;
        else
            originalColor = rend.materials[1].color;
        healthBar.Hide();
        ZombieTargetHitbox zombieTargetHitbox = Instantiate(PrefabManager.Prefabs["Zombie Target Hitbox"]).GetComponent<ZombieTargetHitbox>();
        zombieTargetHitbox.SetZombie(this);
        ZombieTargetHitboxUpdater.Instance.Register(zombieTargetHitbox);
    }

    public void SetLevel(int level)
    {
        this.level = level;
        SetHealth(maxHealth, false);
    }

    public void Stop(float duration = 4)
    {
        //agent.enabled = false;
        agent.isStopped = false;
        agent.SetDestination(TowerController.Instance.GetCurrentTower().Transform.position);
        agent.speed = 1;
        meshAnimator.Play(0);
        isStopped = true;
        tower = null;
        barriers.Clear();
        DOTween.To(() => agent.speed, x => agent.speed = x, 0, duration).OnComplete(() =>
        {
            agent.isStopped = true;
        });
    }

    public void SlowDown(float t = 15, float multiplier = 0.4f)
    {
        if (slowedDown) return;
        if (slowDownTween != null)
        {
            slowDownTween.Kill();
            slowDownTween = null;
        }
        slowedDown = true;
        slowDownMultiplier = multiplier;
        agent.speed *= slowDownMultiplier;
        meshAnimator.speed *= slowDownMultiplier;
        hitCooldownDuration *= 1f / slowDownMultiplier;
        SetColor(slowedDownColor);
        slowDownTween = DOVirtual.DelayedCall(t, CancelSlowDown, false);
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
        agent.speed *= 1f / slowDownMultiplier;
        meshAnimator.speed *= 1f / slowDownMultiplier;
        hitCooldownDuration *= slowDownMultiplier;
        SetColor(originalColor);
    }


    private int GetDamage()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.ZombieDamage(level, isBoss);
            case 2:
                return Settings.World2.ZombieDamage(level, isBoss);
        }
        return 1;
    }

    private int GetMaxHealth()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.ZombieHealth(level, isBoss);
            case 2:
                return Settings.World2.ZombieHealth(level, isBoss);
        }
        return 1;
    }

    private int GetGain()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.ZombieGain(level, isBoss);
            case 2:
                return Settings.World2.ZombieGain(level, isBoss);
        }
        return 1;
    }

    private void DropMoney()
    {
        int gain = GetGain();

        int numberOfCoins = level;
        if (isBoss) numberOfCoins *= 10;
        CoinBurster.Burst(gain, numberOfCoins, Transform.position, Vector3.up, isBoss ? 10 : 7, isBoss);
        UILevitatingText levitatingText = ObjectPooler.SpawnFromPool("Levitating Text", levitatingTextPoint.position, Quaternion.identity).GetComponent<UILevitatingText>();
        levitatingText.SetText("$" + gain);
    }

    public void GetHit(Projectile projectile, bool ignoreLose = false)
    {
        if (!ignoreLose && WaveController.State == WaveController.WaveState.LOSE) return;
        if (currentHealth <= 0 || !gameObject.activeSelf) return;
        Flash(0.05f);
        if (!isBoss)
            GetPushed(projectile.Damage / (float)maxHealth);
        SetHealth(currentHealth - projectile.Damage);
    }
    public void GetHit(int damage, bool ignoreLose = false)
    {
        if (!ignoreLose && WaveController.State == WaveController.WaveState.LOSE) return;
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
        agent.Move(-Transform.forward.normalized * value);
    }
    private void Die()
    {
        DropMoney();
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
        Deactivate();
        ObjectPooler.SpawnFromPool(isBoss ? "Dying Zombie Boss" : ("Dying Zombie " + level), Transform.position, Transform.rotation);
        SoundFX.PlaySound("Zombie Dying Sound " + (Random.value > 0.5f ? "1" : "2"), ShotPoint.position);
        died = true;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        isStopped = false;
        SetHealth(maxHealth, false);
        agent.speed = speed;
        agent.SetDestination(TowerController.Instance.GetCurrentTower().Transform.position);
        CancelSlowDown();
        SetColor(originalColor);
        OnSpawn.Invoke();
        died = false;
    }
    private void SetHealth(int health, bool showBar = true)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        bool first = healthBar.Percent <= currentHealth / (float)maxHealth;
        healthBar.SetPercent(currentHealth / (float)maxHealth);
        if (showBar)
            healthBar.Show(2f, instant: first);
        if (currentHealth == 0) Die();
    }

    private void HitTower(Tower tower)
    {
        if (!canHit) return;
        this.tower = tower;
        tower.GetHit(damage);
        canHit = false;
        DOVirtual.DelayedCall(hitCooldownDuration, () => { canHit = true; }, false);
    }
    private void HitBarrier(Barrier barrier)
    {
        if (!canHit) return;
        barrier.GetHit(damage);
        canHit = false;
        DOVirtual.DelayedCall(hitCooldownDuration, () => { canHit = true; }, false);
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
