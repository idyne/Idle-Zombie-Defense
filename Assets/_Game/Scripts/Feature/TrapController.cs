using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TrapController : MonoBehaviour
{
    [SerializeField] private LayerMask trapLayerMask;
    private Camera mainCamera { get => TowerController.Instance.GetCurrentTower().CameraController.Camera; }
    private bool firstTap = false;
    private Tween firstTapTween = null;
    private void Update()
    {
        if (PauseButton.Paused) return;
        if (Input.GetMouseButtonDown(0) && !(EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null))
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
                    firstTapTween = DOVirtual.DelayedCall(0.6f, () => { firstTap = false; }, false);
                    Bomb bomb = hit.transform.GetComponent<Bomb>();
                    if (bomb && !bomb.Exploded)
                        bomb.ShowRangeIndicator(1);
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
                    /* if (trap.Exploded && WaveController.State == WaveController.WaveState.WAITING)
                         trap.Rebuy();
                     else */
                    if (!trap.Exploded && WaveController.State == WaveController.WaveState.RUNNING)
                        trap.Detonate();
                }
            }
        }
    }
}
