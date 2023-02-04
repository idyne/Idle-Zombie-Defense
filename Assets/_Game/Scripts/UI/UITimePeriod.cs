using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelManager;

public class UITimePeriod : MonoBehaviour
{
    [SerializeField] private Image morning, noon, evening, night;

    public void SetTimePeriod(TimePeriod timePeriod)
    {
        HideAll();
        switch (timePeriod)
        {
            case TimePeriod.Morning:
                ShowMorning();
                break;
            case TimePeriod.Noon:
                ShowNoon();
                break;
            case TimePeriod.Evening:
                ShowEvening();
                break;
            case TimePeriod.Night:
                ShowNight();
                break;
            default:
                break;
        }
    }

    public void HideAll()
    {
        morning.gameObject.SetActive(false);
        noon.gameObject.SetActive(false);
        evening.gameObject.SetActive(false);
        night.gameObject.SetActive(false);
    }

    public void ShowMorning() => morning.gameObject.SetActive(true);
    public void ShowNoon() => noon.gameObject.SetActive(true);
    public void ShowEvening() => evening.gameObject.SetActive(true);
    public void ShowNight() => night.gameObject.SetActive(true);
}
