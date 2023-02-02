using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlacementButton : UIButton
{
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private GameObject capacity;

    public void SetCapacityText(string text)
    {
        capacityText.text = text;
    }

    public void HideCapacity()
    {
        capacity.SetActive(false);
    }

    public void ShowCapacity()
    {
        capacity.SetActive(true);
    }
}
