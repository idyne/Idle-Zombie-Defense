using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.EventSystems;
using static LevelManager;
public class ExplosiveBomb : Bomb
{
    private int damage { get => GetDamage(); }
    protected override int price
    {
        get
        {
            float result = CostManager.GetTNTPrice() / 4;
            switch (ZoneLevel)
            {
                case 2:
                    result = CostManager.GetTNTPrice() / 4 + Mathf.Pow(NormalizedDay, 1.2f) * 11;
                    break;
                default:
                    break;
            }
            return Mathf.CeilToInt(result);
        }
    }
    private int GetDamage()
    {
        return NormalizedDay * 75;
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
        SoundFX.PlaySound("TNT Explosion Sound", transform.position);
        mesh.SetActive(false);
        explodedMesh.SetActive(true);
        Exploded = true;
        HideRangeIndicator();
        return true;
    }
}
