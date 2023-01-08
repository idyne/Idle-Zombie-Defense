using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ZoneBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fromZone, toZone;
    private Transform _transform = null;
    public Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return _transform;
        }
    }
    [SerializeField] private int numberOfDays = 7;
    [SerializeField] private Slider fillSlider, handleSlider;

    private int currentDay = 1;

    public void SetDay(int day)
    {
        int previousDay = currentDay;
        currentDay = day;
        float from, to;
        from = (float)previousDay;
        to = (float)currentDay;
        fromZone.text = "Zone " + WaveController.ZoneLevel;
        toZone.text = "Zone " + (WaveController.ZoneLevel + 1);
        DOTween.To((val) =>
        {
            fillSlider.value = val;
            handleSlider.value = val;
        }, from, to, 0.5f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
