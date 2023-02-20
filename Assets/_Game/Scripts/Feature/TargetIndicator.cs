using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform indicatorTransform;
    [SerializeField] private GameObject canvas;
    private Zombie target = null;
    Camera mainCamera { get => TowerController.Instance.GetCurrentTower().CameraController.Camera; }
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

    public void Show() => canvas.SetActive(true);
    public void Hide() => canvas.SetActive(false);
}
