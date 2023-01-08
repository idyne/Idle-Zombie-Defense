using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;
using UnityEngine.EventSystems;

public class SpeedBooster : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float duration = 0.5f;
    private Tween tween;
    public Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return _transform;
        }
    }
    private Transform _transform;
    public void Boost()
    {
        tween?.Kill();
        Time.timeScale = 1.5f;
        tween = DOVirtual.DelayedCall(duration, () =>
        {
            tween = null;
            Deboost();
        });
        HapticManager.DoHaptic();
    }

    private void Deboost()
    {
        Time.timeScale = 1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Boost();
    }
}
