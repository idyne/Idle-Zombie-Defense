using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected string ammoTag;
    [SerializeField] protected Transform barrel;

    public Transform Barrel { get => barrel; }

    public virtual void Shoot(Zombie target)
    {
        Projectile projectile = ObjectPooler.SpawnFromPool(ammoTag, barrel.position, barrel.rotation).GetComponent<Projectile>();
        projectile.StartMovement(target);
    }
}
