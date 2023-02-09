using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldBar : MonoBehaviour
{
    [SerializeField] private Image[] bars;
    [SerializeField] private Color doneColor, currentColor, notColor;

    public void SetDay()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            Color color;
            if (i + 1 < LevelManager.WorldDay)
                color = doneColor;
            else if (i + 1 == LevelManager.WorldDay)
                color = currentColor;
            else
                color = notColor;
            bars[i].color = color;
        }
    }
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}
