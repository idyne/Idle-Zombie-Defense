using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    public Transform Transform { get; private set; }
    private static Transform cameraTransform = null;
    private void Awake()
    {
        Transform = transform;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        /*Transform.LookAt(cameraTransform);
        Transform.forward = -cameraTransform.forward;*/
        transform.LookAt(Transform.position + cameraTransform.rotation * Vector3.back, cameraTransform.rotation * Vector3.up);
    }

}
