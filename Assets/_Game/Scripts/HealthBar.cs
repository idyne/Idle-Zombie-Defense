using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private Tween tween = null;
    private float desiredValue = 1;
    public float Percent { get => slider.value; }
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
    }

    private void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, desiredValue, Time.deltaTime);
    }

    public void SetPercent(float percent)
    {
        desiredValue = percent * slider.maxValue;
    }

    public void Hide(float duration = -1)
    {
        if (tween != null) tween.Kill();
        gameObject.SetActive(false);
        if (duration > 0)
            tween = DOVirtual.DelayedCall(duration, () => { Show(); });
    }

    public void Show(float duration = -1, bool instant = false)
    {
        if (tween != null) tween.Kill();
        if (instant)
            slider.value = desiredValue;
        gameObject.SetActive(true);
        if (duration > 0)
            tween = DOVirtual.DelayedCall(duration, () => { Hide(); });
    }
}
