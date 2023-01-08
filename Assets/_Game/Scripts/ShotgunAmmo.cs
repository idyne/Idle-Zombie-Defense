using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotgunAmmo : Projectile
{
    protected override void OnReached(Zombie target)
    {
        throw new System.NotImplementedException();
    }

    public void Go(Vector3 direction)
    {
        float distance = 40;
        float t = distance / velocity;
        float previousValue = 0;
        Transform.DOMove(Transform.position + direction.normalized * distance, t).OnComplete(Deactivate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            zombie.GetHit(this);
            Deactivate();
        }
    }

}
