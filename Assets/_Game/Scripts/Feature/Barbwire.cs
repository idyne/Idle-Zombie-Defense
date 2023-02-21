using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static LevelManager;

public class Barbwire : Trap
{
    protected int damagePerSecond { get => GetDPS(); }
    [SerializeField] protected float radius;
    [SerializeField] protected LayerMask zombieLayerMask;
    protected List<Zombie> cooldownList = new();
    private Dictionary<Zombie, int> damageTable = new();
    public override void Initialize(bool exploded, Grid grid, int saveDataIndex)
    {
        this.saveDataIndex = saveDataIndex;
        Attach(grid, init: true);
    }

    protected override void Awake()
    {
        base.Awake();
        WaveController.Instance.OnWaveStart.AddListener(() => { damageTable = new(); });
    }

    protected override bool Explode()
    {
        return false;
    }

    private int GetDPS()
    {
        return Settings.BarbwireDPS;
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            if (!cooldownList.Contains(zombie))
            {
                float t = 0.5f;
                int damage = Mathf.CeilToInt(damagePerSecond * t);

                if (damageTable.ContainsKey(zombie))
                {
                    if (damageTable[zombie] < Settings.BarbwireMaxDamage)
                    {
                        damage = Mathf.Clamp(damage, 0, Settings.BarbwireMaxDamage - damageTable[zombie]);
                        zombie.GetHit(damage);
                        zombie.SlowDown(t);
                        cooldownList.Add(zombie);
                        damageTable[zombie] += damage;
                        DOVirtual.DelayedCall(t, () => { cooldownList.Remove(zombie); }, false);
                    }
                }
                else
                {
                    zombie.GetHit(damage);
                    zombie.SlowDown(t);
                    cooldownList.Add(zombie);
                    damageTable.Add(zombie, damage);
                    DOVirtual.DelayedCall(t, () => { cooldownList.Remove(zombie); }, false);
                }
            }
        }
    }
}
