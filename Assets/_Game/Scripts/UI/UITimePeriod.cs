using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimePeriod : MonoBehaviour
{
    [SerializeField] private Image morning, noon, evening, night;

    public void SetTimePeriod(WaveController.TimePeriod timePeriod)
    {
        HideAll();
        switch (timePeriod)
        {
            case WaveController.TimePeriod.Morning:
                ShowMorning();
                break;
            case WaveController.TimePeriod.Noon:
                ShowNoon();
                break;
            case WaveController.TimePeriod.Evening:
                ShowEvening();
                break;
            case WaveController.TimePeriod.Night:
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
