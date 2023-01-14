using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostBomb : Bomb
{
    protected override bool Explode()
    {
        if (Exploded) return false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.SlowDown();
        }
        ObjectPooler.SpawnFromPool("Frost Bomb Effect", transform.position, Quaternion.identity);
        mesh.SetActive(false);
        explodedMesh.SetActive(true);
        Exploded = true;
        return true;
    }
}
