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
    [SerializeField] private List<UIWorldBar> worldBars;

    public void SetDay()
    {
        HideZoneBars();
        UIWorldBar worldBar = worldBars[WorldLevel - 1];
        worldBar.Show();
        worldBar.SetDay();
    }

    private void HideZoneBars()
    {
        for (int i = 0; i < worldBars.Count; i++)
            worldBars[i].Hide();
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);

}
