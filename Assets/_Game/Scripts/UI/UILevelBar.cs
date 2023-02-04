using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LevelManager;
public class UILevelBar : MonoBehaviour
{
    
    private static UILevelBar instance = null;
    public static UILevelBar Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UILevelBar>();
            return instance;
        }
    }
    [SerializeField] private List<UIZoneBar> zoneBars;

    public void SetDay(int day)
    {
        HideZoneBars();
        UIZoneBar zoneBar = zoneBars[ZoneLevel - 1];
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
