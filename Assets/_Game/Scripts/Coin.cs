using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;

public class Coin : MonoBehaviour, IPooledObject
{
    private Rigidbody rb;
    private Transform _transform = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return _transform;
        }
    }


    private void GoToBase()
    {
        rb.isKinematic = true;
        float distance = Transform.position.magnitude;
        float velocity = 20;
        float time = distance / velocity;
        Transform.SimulateProjectileMotion(new Vector3(0, 2, 0), time, () => { gameObject.SetActive(false); });
        //Transform.DOMove(new Vector3(0, 0, 0), time).SetEase(Ease.InQuint).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        rb.isKinematic = false;
        Invoke(nameof(GoToBase), 3);
    }
}
