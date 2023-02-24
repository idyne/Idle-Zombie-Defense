using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.EventSystems;


public abstract class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform camTransform;
    [SerializeField] protected Camera camera;
    private Transform _transform = null;
    public static bool InPlacementMode = false;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }

    public Transform CamTransform { get => camTransform; }
    public Camera Camera { get => camera;  }

    public abstract void ZoomOut();
    public abstract void ZoomIn();


}
