using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using FSG.MeshAnimator;

public class Soldier : MonoBehaviour, IPooledObject
{
    public Transform Transform { get; private set; }
    [SerializeField] private float baseShootingPeriod = 1;
    private float shootingPeriod { get => baseShootingPeriod - (Barracks.FireRateLevel - 1) * baseShootingPeriod / (float)LimitManager.GetMaxFireRateLevel() * 0.6f; }
    [SerializeField] private float range = 20;
    private Weapon weapon;
    private Zombie target = null;
    private MeshAnimatorBase meshAnimator;
    private float lastShootTime = -50;
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private Animator ragdollAnimator;
    private void Awake()
    {
        Transform = transform;
        weapon = GetComponentInChildren<Weapon>();
        meshAnimator = GetComponentInChildren<MeshAnimatorBase>();
    }

    private void Update()
    {
        if (!target) return;
        Vector3 direction = target.Transform.position - Transform.position;
        direction.y = 0;
        Vector3 newDir = Vector3.MoveTowards(transform.forward, direction, Time.deltaTime * 8);
        Transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void FindTarget()
    {
        if (WaveController.Instance.CurrentWave == null || WaveController.State != WaveController.WaveState.RUNNING || target != null) return;
        SetTarget();
    }

    public void SetTarget()
    {
        if (WaveController.Instance.CurrentWave == null || WaveController.State != WaveController.WaveState.RUNNING) return;
        Zombie closestZombie = WaveController.Instance.CurrentWave.GetClosest(Transform.position);
        if (closestZombie == null || Vector3.Distance(closestZombie.Transform.position, Transform.position) > range) return;
        target = closestZombie;
        target.targetedSoldiers.Add(this);
        closestZombie.OnDeath.AddListener(OnTargetDeath);
        //LookAtTarget(StartShooting);
        if (lastShootTime + shootingPeriod > Time.time)
            DOVirtual.DelayedCall(lastShootTime + shootingPeriod - Time.time, StartShooting, false);
        else
            StartShooting();
    }

    public void ActivateRagdoll()
    {
        if (ragdoll)
        {
            ragdollAnimator.enabled = false;
            ragdoll.transform.SetParent(null);
            ragdoll.SetActive(true);
            Rigidbody[] rigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(5, Vector3.zero, 10, 1, ForceMode.Impulse);
            }
        }
    }

    public void Rewind()
    {
        Rigidbody[] rigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
            rb.isKinematic = true;
        ragdollAnimator.enabled = true;
        ragdollAnimator.SetTrigger("Rewind");
        ragdoll.transform.SimulateProjectileMotion(transform.position, 2);
        ragdoll.transform.DORotateQuaternion(transform.rotation, 2);
        DOVirtual.DelayedCall(2, () =>
        {
            Activate();
            ragdoll.transform.SetParent(transform);
            ragdoll.SetActive(false);
        });
    }

    private void LookAtTarget(TweenCallback onComplete)
    {
        Vector3 direction = target.Transform.position - Transform.position;
        direction.y = 0;
        Transform.DORotate(Quaternion.LookRotation(direction).eulerAngles, 0.4f).OnComplete(onComplete);
    }

    public void OnTargetDeath()
    {
        RemoveTarget(); SetTarget();
    }

    private void StartShooting()
    {
        if (!target) return;
        CancelInvoke(nameof(Shoot));
        InvokeRepeating(nameof(Shoot), 0, shootingPeriod);

    }
    private void StopShooting()
    {
        CancelInvoke(nameof(Shoot));
        //meshAnimator.Play(0);
    }

    private void Shoot()
    {
        lastShootTime = Time.time;
        weapon.Shoot(target);
        meshAnimator.Play(1);
        meshAnimator.RestartAnim();
    }

    private void RemoveTarget()
    {
        if (target)
        {
            target.OnDeath.RemoveListener(OnTargetDeath);
            target.targetedSoldiers.Remove(this);
            target = null;
        }
        StopShooting();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        InvokeRepeating(nameof(FindTarget), 0, 2 + Random.Range(-0.2f, 0.2f));
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        RemoveTarget();
        DOTween.Kill(Transform);
        CancelInvoke();
    }

    public void OnObjectSpawn()
    {
        //meshAnimator.Play(0);
        InvokeRepeating(nameof(FindTarget), 0, 2 + Random.Range(-0.2f, 0.2f));
    }

    public void PlayFireRateEffect()
    {
        ObjectPooler.SpawnFromPool("Fire Rate Effect", weapon.Barrel.position, Quaternion.identity);
    }
}
