using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using static LevelManager;
public class AirstrikeBomb : MonoBehaviour, IPooledObject
{
    [SerializeField] private float range;
    [SerializeField] private LayerMask zombieLayerMask;
    [SerializeField] private Animator animator;
    private int damage { get => GetDamage(); }
    private Transform _transform = null;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }
    private int GetDamage()
    {
        return WorldDay * 25;
    }
    private void Fall()
    {
        animator.SetTrigger("Fall");
    }
    public void Explode()
    {
        ObjectPooler.SpawnFromPool("Airstrike Bomb Effect", transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(damage);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        Fall();
    }
}
