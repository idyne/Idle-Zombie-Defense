using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    new Transform transform = null;
    Transform Transform
    {
        get
        {
            if (transform == null)
                transform = base.transform;
            return transform;
        }
    }
    private void Update()
    {
        Transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
