using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;

public class ExplosiveAmmo : Projectile
{
    [SerializeField] private float range = 4;
    [SerializeField] private LayerMask affectedLayers;
    [SerializeField] private string soundFXTag;
    protected override void OnReached(Zombie target)
    {
        ObjectPooler.SpawnFromPool("Explosion Effect", Transform.position, Transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(Transform.position, range, affectedLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(this);
        }
        //target.GetHit(this);

        DOVirtual.DelayedCall(1, Deactivate, false);
        SoundFX.PlaySound(soundFXTag, Transform.position);
    }
}
