using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public class FrostBomb : Bomb
{
    protected override int price
    {
        get
        {
            float result = CostManager.GetFrostPrice() / 4;
            switch (ZoneLevel)
            {
                case 2:
                    result = CostManager.GetFrostPrice() / 4 + Mathf.Pow(NormalizedDay, 1.2f) * 11;
                    break;
                default:
                    break;
            }
            return Mathf.CeilToInt(result);
        }
    }
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
        HideRangeIndicator();
        return true;
    }
}
