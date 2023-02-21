using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

public class Grenade : ThrowableWeapon
{
    [SerializeField] protected float range;
    protected override int GetDamage()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.GrenadeDamage;
            case 2:
                return Settings.World2.GrenadeDamage;
        }
        return 75;
    }
    protected override void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(damage);
        }
        ObjectPooler.SpawnFromPool("Grenade Explosion Effect", transform.position, Quaternion.identity);
        SoundFX.PlaySound("Grenade Explosion Sound", transform.position);
        Deactivate();
    }
}
