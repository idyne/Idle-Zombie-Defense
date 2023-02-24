using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBomb : MonoBehaviour
{
    private static WaveBomb instance = null;
    public static WaveBomb Instance
    {
        get
        {

            if (instance == null) instance = FindObjectOfType<WaveBomb>();
            return instance;
        }
    }
    private float range = 100;
    private bool explode = false;
    private float currentRange = 1;
    private Transform _transform = null;
    [SerializeField] private LayerMask zombieLayerMask;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }

    private void Awake()
    {
        WaveController.Instance.OnWaveEnd.AddListener(() => { explode = false; });
    }

    private void Update()
    {
        if (!explode && Input.GetKeyDown(KeyCode.U)) Explode();
    }

    private void FixedUpdate()
    {
        if (!explode) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, currentRange, zombieLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Zombie zombie = colliders[i].GetComponent<Zombie>();
            zombie.GetHit(999999, true);
        }
        if (currentRange >= range)
        {
            print("false explode");
            explode = false;
            return;
        }
        currentRange += Time.fixedDeltaTime * 26;

    }
    public void Explode()
    {
        currentRange = 1;
        explode = true;
    }
}
