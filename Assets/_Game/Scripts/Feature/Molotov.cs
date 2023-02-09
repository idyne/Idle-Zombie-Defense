using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Molotov : ThrowableWeapon, IPooledObject
{
    protected int damagePerSecond = 50;
    protected List<Zombie> cooldownList = new();
    [SerializeField] private Collider damageCollider;
    [SerializeField] private GameObject mesh;
    private float duration = 4;

    public void OnObjectSpawn()
    {
        damageCollider.enabled = false;
        ActivateMesh();
    }

    protected override void Explode()
    {
        damageCollider.enabled = true;
        DeactivateMesh();
        ObjectPooler.SpawnFromPool("Molotov Effect", transform.position, Quaternion.identity);
        DOVirtual.DelayedCall(duration, Deactivate);
    }

    private void OnTriggerStay(Collider other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (!cooldownList.Contains(zombie))
        {
            float t = 0.5f;
            zombie.GetHit(Mathf.CeilToInt(damagePerSecond * t));
            cooldownList.Add(zombie);
            DOVirtual.DelayedCall(t, () => { cooldownList.Remove(zombie); });
        }
    }

    private void DeactivateMesh() => mesh.SetActive(false);
    private void ActivateMesh() => mesh.SetActive(true);
}
