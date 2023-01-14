using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrapController : MonoBehaviour
{
    [SerializeField] private LayerMask trapLayerMask;
    private Camera _mainCamera = null;
    private Camera mainCamera { get { if (_mainCamera == null) _mainCamera = Camera.main; return _mainCamera; } }
    private bool firstTap = false;
    private Tween firstTapTween = null;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // Perform the raycast for 'Trap' layermask
            if (Physics.Raycast(ray, out RaycastHit hit, 100, trapLayerMask))
            {
                if (!firstTap)
                {
                    if (firstTapTween != null)
                    {
                        firstTapTween.Kill();
                        firstTapTween = null;
                    }
                    firstTap = true;
                    firstTapTween = DOVirtual.DelayedCall(0.8f, () => { firstTap = false; });
                }
                else
                {
                    firstTap = false;
                    if (firstTapTween != null)
                    {
                        firstTapTween.Kill();
                        firstTapTween = null;
                    }
                    Trap trap = hit.transform.GetComponent<Trap>();
                    if (trap.Exploded && WaveController.State == WaveController.WaveState.WAITING)
                        trap.Rebuy();
                    else if (!trap.Exploded && WaveController.State == WaveController.WaveState.RUNNING)
                        trap.Detonate();
                }
            }
        }
    }
}
