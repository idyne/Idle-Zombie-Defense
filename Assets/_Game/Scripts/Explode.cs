using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public float radius = 5;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(5, transform.position, radius, 1, ForceMode.Impulse);
            }
        }
    }
}
