using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.EventSystems;


public abstract class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform camTransform;
    public static bool InPlacementMode = false;
    private Transform _transform = null;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }

    public abstract void ZoomOut();


}
