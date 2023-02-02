using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIDayBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float desiredValue = 0;
    [SerializeField] private TextMeshProUGUI dayText;

    private void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, desiredValue, Time.deltaTime / 2);
    }


    public void SetPercent(float percent, bool withAnimation = true)
    {
        desiredValue = percent;
        if (!withAnimation) slider.value = percent;
    }

    public void SetDay(int day)
    {
        dayText.text = "DAY " + day;
    }

    public void GoUp()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(-266, 0.5f);
        rectTransform.DOScale(1.25f, 0.5f);
    }
    
    public Tween GoDown(bool animate = true)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (animate)
        {
            rectTransform.DOAnchorPosY(-345.7f, 0.5f);
            return rectTransform.DOScale(1.0f, 0.5f);
        }
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = -345.7f;
        rectTransform.anchoredPosition = pos;
        rectTransform.localScale = new Vector2(1, 1);
        return null;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
