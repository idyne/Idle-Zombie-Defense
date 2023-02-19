using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uzi : Weapon
{
    public override void Shoot(Zombie target)
    {

        StartCoroutine(ShootCoroutine(target, 3, 0.04f));
    }

    private IEnumerator ShootCoroutine(Zombie target, int count, float delay)
    {
        if (count > 0 && target)
        {
            Projectile projectile = ObjectPooler.SpawnFromPool(ammoTag, barrel.position, barrel.rotation).GetComponent<Projectile>();
            projectile.StartMovement(target);
            SoundFX.PlaySound(soundFXTag, transform.position);
            yield return new WaitForSeconds(delay);
            yield return ShootCoroutine(target, count - 1, delay);
        }
    }
}
