using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;

public abstract class Projectile : MonoBehaviour, IPooledObject
{
    [SerializeField] protected float velocity = 5;
    [SerializeField] protected Ease ease;
    public int Damage = 1;
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
    }

    //TODO dotween.kill çalýþýyor mu kesin deðil
    public virtual void StartMovement(Zombie target)
    {
        float distance = Vector3.Distance(Transform.position, target.Transform.position);
        float t = distance / velocity;
        float previousValue = 0;
        DOTween.To((val) =>
        {
            float deltaTime = val - previousValue;
            Vector3 position = Vector3.MoveTowards(Transform.position, target.ShotPoint.position, deltaTime * velocity);
            Quaternion rotation;
            if (position - Transform.position != Vector3.zero)
                rotation = Quaternion.LookRotation(position - Transform.position);
            else rotation = Transform.rotation;
            if (rotation.eulerAngles != Vector3.zero)
                Transform.SetPositionAndRotation(position, rotation);
            else Transform.position = position;
            previousValue = val;
        }, 0, t, t).SetEase(ease).OnComplete(() => { OnReached(target); });
    }

    protected abstract void OnReached(Zombie target);

    protected void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        DOTween.Kill(Transform);
    }
}
