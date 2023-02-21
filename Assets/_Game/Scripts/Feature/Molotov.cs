using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using static LevelManager;

public class Molotov : ThrowableWeapon, IPooledObject
{
    protected int damagePerSecond { get => GetDPS(); }
    protected List<Zombie> cooldownList = new();
    [SerializeField] private Collider damageCollider;
    [SerializeField] private GameObject mesh;
    private float duration = 6;

    private int GetDPS()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.MolotovDPS;
            case 2:
                return Settings.World2.MolotovDPS;
        }
        return 75;
    }

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
        SoundFX.PlaySound("Molotov Impact Sound", transform.position);
        SoundFXWorker worker = SoundFX.PlaySound("Fire Sound", transform.position);
        DOVirtual.DelayedCall(duration, () => { worker.Stop(); Deactivate(); }, false);
    }

    private void OnTriggerStay(Collider other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (!cooldownList.Contains(zombie))
        {
            float t = 0.5f;
            zombie.GetHit(Mathf.CeilToInt(damagePerSecond * t));
            cooldownList.Add(zombie);
            DOVirtual.DelayedCall(t, () => { cooldownList.Remove(zombie); }, false);
        }
    }

    private void DeactivateMesh() => mesh.SetActive(false);
    private void ActivateMesh() => mesh.SetActive(true);
}
