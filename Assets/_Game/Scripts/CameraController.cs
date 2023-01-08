using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;

[RequireComponent(typeof(Swerve))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    private bool zoomingOut = false;
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CameraController>();
            return instance;
        }
    }
    private Swerve swerve;
    private float anchorAngle = 0;
    private float angle = 270;
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
        swerve = GetComponent<Swerve>();
        swerve.OnStart.AddListener(SetAnchorAngle);
        swerve.OnSwerve.AddListener(() =>
        {
            if (zoomingOut) return;
            angle = anchorAngle + swerve.XRate * 180;
            Transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, angle, 0) * Vector3.right);
        });
    }

    private void SetAnchorAngle()
    {
        anchorAngle = angle;
    }

    public void ZoomOut()
    {
        zoomingOut = true;
        DOTween.To(() => camTransform.position, x => camTransform.position = x, camTransform.position - camTransform.forward * 5, 3);
    }

}
