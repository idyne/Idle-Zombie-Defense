using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon
{
    public override void Shoot(Zombie target)
    {
        StartCoroutine(LaunchRockets(target));
    }

    private IEnumerator LaunchRockets(Zombie target, int rocketsLeft = 4)
    {
        if (rocketsLeft <= 0) yield break;
        base.Shoot(target);
        yield return new WaitForSeconds(0.2f);
        yield return LaunchRockets(target, rocketsLeft - 1);
    }
}
