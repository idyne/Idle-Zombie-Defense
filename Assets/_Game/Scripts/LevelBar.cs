using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBar : MonoBehaviour
{
    
    private static LevelBar instance = null;
    public static LevelBar Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LevelBar>();
            return instance;
        }
    }
    [SerializeField] private List<ZoneBar> zoneBars;

    public void SetDay(int day)
    {
        HideZoneBars();
        ZoneBar zoneBar = zoneBars[WaveController.ZoneLevel - 1];
        zoneBar.Show();
        zoneBar.SetDay(day);
    }

    private void HideZoneBars()
    {
        for (int i = 0; i < zoneBars.Count; i++)
            zoneBars[i].Hide();
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);

}
