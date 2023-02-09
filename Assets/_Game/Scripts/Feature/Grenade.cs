using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ThrowableWeapon
{
    [SerializeField] protected float range;
    protected override void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(damage);
        }
        ObjectPooler.SpawnFromPool("Grenade Explosion Effect", transform.position, Quaternion.identity);
        Deactivate();
    }
}
