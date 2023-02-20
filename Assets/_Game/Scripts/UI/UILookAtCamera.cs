using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    public Transform Transform { get; private set; }
    private Transform cameraTransform { get => TowerController.Instance.GetCurrentTower().CameraController.CamTransform; }
    private void Awake()
    {
        Transform = transform;
    }

    private void Update()
    {
        /*Transform.LookAt(cameraTransform);
        Transform.forward = -cameraTransform.forward;*/
        transform.LookAt(Transform.position + cameraTransform.rotation * Vector3.back, cameraTransform.rotation * Vector3.up);
    }

}
