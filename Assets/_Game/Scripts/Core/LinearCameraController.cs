
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LinearCameraController : CameraController
{
    [SerializeField] private Transform outWavePoint, inWavePoint;
    public override void ZoomOut()
    {
        //TODO
        //throw new System.NotImplementedException();
    }

    private void Awake()
    {
        PlacementModeController.OnExitPlacementMode.AddListener(() =>
        {
            camTransform.DOMove(inWavePoint.position, 1);
            camTransform.DORotateQuaternion(inWavePoint.rotation, 1);
        });
        PlacementModeController.OnEnterPlacementMode.AddListener(() =>
        {
            camTransform.DOMove(outWavePoint.position, 1);
            camTransform.DORotateQuaternion(outWavePoint.rotation, 1);
        });
    }

    private void Start()
    {
        camTransform.SetPositionAndRotation(inWavePoint.position, inWavePoint.rotation);
    }
}
