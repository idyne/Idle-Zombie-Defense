using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using static LevelManager;

public abstract class ThrowableWeapon : MonoBehaviour
{
    private Transform _transform = null;
    [SerializeField] protected LayerMask zombieLayerMask;
    protected virtual int GetDamage()
    {
        return CycleDay * 75;
    }
    protected int damage { get => GetDamage(); }
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }
    public void Throw(Vector3 to)
    {
        to.y = 0f;
        transform.SimulateProjectileMotion(to, 1.5f, () =>
        {
            Explode();
        });
    }

    protected abstract void Explode();

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
