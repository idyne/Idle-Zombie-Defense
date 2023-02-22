using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Swerve))]
public class PointCameraController : CameraController
{

    private bool zoomingOut = false;

    [SerializeField] private float cameraForwardRange = 10;
    [SerializeField] private float cameraBackwardRange = -10;
    [SerializeField] private float cameraZoomSpeed = 10;
    private Vector3 direction;
    private Vector3 initialCameraPosition;

    private float zoomDistance = 0;
    private float anchorDistance = 0;
    private bool onUI = false;


    private Swerve swerve;
    private float anchorAngle = 0;
    private float angle = 270;
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
        Init();
        swerve = GetComponent<Swerve>();
        swerve.OnStart.AddListener(SetAnchorAngle);
        swerve.OnStart.AddListener(SetAnchorDistance);
        swerve.OnStart.AddListener(() => { onUI = EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null; });
        swerve.OnSwerve.AddListener(() =>
        {
            if (zoomingOut || InPlacementMode || onUI) return;
            angle = anchorAngle + swerve.XRate * 180;
            zoomDistance = Mathf.Clamp(anchorDistance - swerve.YRate * cameraZoomSpeed, cameraBackwardRange, cameraForwardRange);
            DayCycler.Instance.ChangeFogOffset(-zoomDistance);
            camTransform.localPosition = initialCameraPosition + direction * zoomDistance;
            Transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, angle, 0) * Vector3.right);
        });

    }

    private void Start()
    {
        zoomDistance = Mathf.Clamp(anchorDistance - swerve.YRate * cameraZoomSpeed, cameraBackwardRange, cameraForwardRange);
        DayCycler.Instance.ChangeFogOffset(-zoomDistance);
        camTransform.localPosition = initialCameraPosition + direction * zoomDistance;
    }

    private void SetAnchorAngle()
    {
        if (InPlacementMode) return;
        anchorAngle = angle;
    }

    private void SetAnchorDistance()
    {
        if (InPlacementMode) return;
        anchorDistance = zoomDistance;
    }


    public override void ZoomOut()
    {
        zoomingOut = true;
        DOTween.To(() => camTransform.position, x => camTransform.position = x, camTransform.position - camTransform.forward * 5, 3);
    }

    private void Init()
    {
        direction = camTransform.localRotation * Vector3.forward;
        initialCameraPosition = camTransform.localPosition;
    }
}
