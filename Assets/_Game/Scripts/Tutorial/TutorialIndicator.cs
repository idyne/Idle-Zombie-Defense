using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : MonoBehaviour
{
    private Canvas _canvas = null;
    public Canvas canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = GetComponentInChildren<Canvas>(true);
            return _canvas;
        }
    }
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private RectTransform indicatorTransform;
    

    Camera mainCamera { get => TowerController.Instance.GetCurrentTower().CameraController.Camera; }


    private void Update()
    {
        if (target)
        {
            indicatorTransform.position = mainCamera.WorldToScreenPoint(target.position);
        }
    }

    public void Show()
    {
        if (target)
        {
            indicatorTransform.position = mainCamera.WorldToScreenPoint(target.position);
        }
        gameObject.SetActive(true);
        if (canvas)
            canvas.enabled = true;
        animator.SetTrigger("Play");
    }

    public void Hide()
    {
        if (canvas)
            canvas.enabled = false;
        gameObject.SetActive(false);
    }
}
