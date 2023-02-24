using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTargetHitbox : MonoBehaviour
{
    public bool Active { get; private set; }
    private SphereCollider sphereCollider;
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
    private GameObject _gameObject = null;
    public GameObject gameObject
    {
        get
        {
            if (_gameObject == null)
                _gameObject = base.gameObject;
            return _gameObject;
        }
    }
    private void Awake()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 3;
        sphereCollider.isTrigger = true;
        gameObject.SetActive(false);
    }

    public Zombie Zombie { get; private set; }
    public void SetZombie(Zombie zombie)
    {
        Zombie = zombie;
        zombie.OnSpawn.AddListener(() =>
        {
            gameObject.SetActive(true);
            Active = true;
            zombie.OnDeath.AddListener(() => { gameObject.SetActive(false); Active = false; });
        });

    }

    public void SetPosition()
    {
        if (!Zombie) return;
        transform.position = Zombie.ShotPoint.position;
    }
}
