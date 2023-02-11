using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform indicatorTransform;
    Camera _mainCamera = null;
    private Zombie target = null;
    Camera mainCamera
    {
        get
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera;
        }
    }
    private void Start()
    {
        Hide();
    }
    private void Update()
    {
        if (target)
        {
            indicatorTransform.position = mainCamera.WorldToScreenPoint(target.ShotPoint.position);
        }
    }
    public void SetTarget(Zombie target)
    {
        this.target = target;
        Show();
        animator.SetTrigger("Target");
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
