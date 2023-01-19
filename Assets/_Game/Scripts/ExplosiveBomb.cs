using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.EventSystems;

public class ExplosiveBomb : Bomb
{
    private int damage { get => GetDamage(); }
    protected override int price
    {
        get
        {
            return OutWaveButtonsManager.GetTNTPrice() / 4;
        }
    }
    private int GetDamage()
    {
        return WaveController.NormalizedDay * 75;
    }

    protected override bool Explode()
    {
        if (Exploded) return false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(damage);
        }
        ObjectPooler.SpawnFromPool("Explosive Bomb Effect", transform.position, Quaternion.identity);
        mesh.SetActive(false);
        explodedMesh.SetActive(true);
        Exploded = true;
        HideRangeIndicator();
        return true;
    }
}
