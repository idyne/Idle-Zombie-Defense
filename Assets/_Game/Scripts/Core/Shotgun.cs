using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public override void Shoot(Zombie target)
    {
        for (int i = -2; i < 3; i++)
        {
            ShotgunAmmo projectile = ObjectPooler.SpawnFromPool(ammoTag, barrel.position, barrel.rotation).GetComponent<ShotgunAmmo>();
            Vector3 direction = Quaternion.Euler(0, i * 5, 0) * target.ShotPoint.position - projectile.Transform.position;
            projectile.Go(direction);
        }
    }
}
