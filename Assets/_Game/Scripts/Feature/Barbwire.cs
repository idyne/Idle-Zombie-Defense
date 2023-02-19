using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Barbwire : Trap
{
    protected int damagePerSecond = 50;
    [SerializeField] protected float radius;
    [SerializeField] protected LayerMask zombieLayerMask;
    protected List<Zombie> cooldownList = new();
    public override void Initialize(bool exploded, Grid grid, int saveDataIndex)
    {
        this.saveDataIndex = saveDataIndex;
        Attach(grid, init: true);
    }

    protected override bool Explode()
    {
        return false;
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
                zombie.GetHit(Mathf.CeilToInt(damagePerSecond * t));
                zombie.SlowDown(t);
                cooldownList.Add(zombie);
                DOVirtual.DelayedCall(t, () => { cooldownList.Remove(zombie); }, false);
            }
        }
    }
}
