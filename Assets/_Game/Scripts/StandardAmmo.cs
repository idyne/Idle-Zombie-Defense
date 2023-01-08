using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardAmmo : Projectile
{
    protected override void OnReached(Zombie target)
    {
        target.GetHit(this);
        Deactivate();
    }
}
