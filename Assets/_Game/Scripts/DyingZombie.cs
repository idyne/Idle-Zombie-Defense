using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.Events;

public class DyingZombie : MonoBehaviour, IPooledObject
{
    [SerializeField] private Transform explosionForcePoint;
    [SerializeField] private Animation anim;
    private Renderer rend;
    private Rigidbody[] rigidbodies;
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
    }

    public void OnObjectSpawn()
    {
        anim.Play("Pose");
        rend.material.SetFloat("_DissolveValue", 0);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddExplosionForce(10, Transform.position - (Transform.position).normalized - new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), 5, 0.05f, ForceMode.Impulse);
        }
        StartCoroutine(DelayedCall(1 + Random.Range(-0.5f, 0.5f), () =>
       {
           DOTween.To((val) =>
           {
               rend.material.SetFloat("_DissolveValue", val);
           }, 0, 1, 1).OnComplete(() =>
           {
               gameObject.SetActive(false);
           });
       }));
    }

    private IEnumerator DelayedCall(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

}
