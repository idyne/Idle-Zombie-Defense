using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Placeable
{
    [SerializeField] private string shootingSoundTag;
    public static bool Stopped = false;
    [SerializeField] private float shootingPeriod = 1;
    [SerializeField] private float range = 20;
    [SerializeField] private Transform head;
    [SerializeField] protected string ammoTag;
    [SerializeField] protected Transform barrel;
    [SerializeField] private Transform rangeIndicatorTransform;
    [SerializeField] private Animator animator;
    private int saveDataIndex = -1;
    private bool bought { get => saveDataIndex >= 0; }

    public Transform Barrel { get => barrel; }
    private Zombie target = null;
    private float lastShootTime = -50;

    private void Awake()
    {
        HideRangeIndicator();
        OnSelect.AddListener(() => { ShowRangeIndicator(); });
        OnDrop.AddListener(() => { HideRangeIndicator(); });
        UIAnimationSequencer.OnNewZone.AddListener(() =>
        {
            if (grid)
                Attach(TowerController.Instance.GetCurrentTower().TurretPlacementController.GetGrid(grid.Id), false, true);
        });
    }

    protected virtual void Start()
    {
        rangeIndicatorTransform.localScale = Vector3.one * range;

        InvokeRepeating(nameof(FindTarget), 0, 2 + Random.Range(-0.2f, 0.2f));
    }
    private void HideRangeIndicator(float duration = -1)
    {
        if (DOTween.Kill(this) > 0) print("Turret hiderangeindicator tween killed");
        rangeIndicatorTransform.gameObject.SetActive(false);
        if (duration > 0)
            DOVirtual.DelayedCall(duration, () => { ShowRangeIndicator(); }, false);
    }
    private void ShowRangeIndicator(float duration = -1)
    {
        if (DOTween.Kill(this) > 0) print("Turret showrangeindicator tween killed");
        rangeIndicatorTransform.gameObject.SetActive(true);
        if (duration > 0)
            DOVirtual.DelayedCall(duration, () => { HideRangeIndicator(); }, false);
    }

    private void Update()
    {
        if (rangeIndicatorTransform.gameObject.activeSelf)
        {
            Vector3 pos = rangeIndicatorTransform.position;
            pos.y = 0.01f;
            rangeIndicatorTransform.position = pos;
        }
        if (!target) return;
        Vector3 direction = target.ShotPoint.position - barrel.position;
        Debug.DrawRay(barrel.position, direction);
        head.rotation = Quaternion.Lerp(head.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 8);
    }

    public void Initialize(Grid grid, int saveDataIndex)
    {
        this.saveDataIndex = saveDataIndex;
        Attach(grid, init: true);
    }
    private void FindTarget()
    {
        if (WaveController.Instance.CurrentWave == null || WaveController.State != WaveController.WaveState.RUNNING || target != null) return;
        SetTarget();
    }

    public void SetTarget()
    {
        if (WaveController.Instance.CurrentWave == null || WaveController.State != WaveController.WaveState.RUNNING) return;
        Zombie closestZombie = WaveController.Instance.CurrentWave.GetClosest(transform.position);
        if (closestZombie == null || Vector3.Distance(closestZombie.Transform.position, transform.position) > range) return;
        target = closestZombie;
        closestZombie.OnDeath.AddListener(OnTargetDeath);
        if (lastShootTime + shootingPeriod > Time.time)
            DOVirtual.DelayedCall(lastShootTime + shootingPeriod - Time.time, StartShooting, false);
        else
            StartShooting();
    }

    private void OnTargetDeath()
    {
        RemoveTarget(); SetTarget();
    }
    private void StartShooting()
    {
        if (!target) return;
        animator.SetFloat("Speed", 0.15f / shootingPeriod);
        CancelInvoke(nameof(Shoot));
        InvokeRepeating(nameof(Shoot), 0, shootingPeriod);

    }
    private void RemoveTarget()
    {
        target = null;
        StopShooting();
    }
    private void Shoot()
    {
        if (Stopped) return;
        lastShootTime = Time.time;
        Projectile projectile = ObjectPooler.SpawnFromPool(ammoTag, barrel.position, barrel.rotation).GetComponent<Projectile>();
        projectile.StartMovement(target);
        SoundFX.PlaySound(shootingSoundTag, barrel.position);
    }
    private void StopShooting()
    {
        animator.SetFloat("Speed", 0.15f);
        CancelInvoke(nameof(Shoot));
    }
    public override void Attach(Grid grid, bool sound = true, bool init = false)
    {
        base.Attach(grid, sound, init);
        if (!bought)
        {
            saveDataIndex = PlayerProgression.PlayerData.Turrets.Count;
            PlayerProgression.MONEY -= CostManager.GetTurretPrice();
            PlayerProgression.PlayerData.Turrets.Add(grid.Id);
            PlayerProgression.MONEY = PlayerProgression.MONEY;
        }
        else
        {
            PlayerProgression.PlayerData.Turrets[saveDataIndex] = grid.Id;
        }
    }
}
