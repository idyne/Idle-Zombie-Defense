using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIOutWaveButton : UIButton
{
    [SerializeField] private TextMeshProUGUI capacityText;

    public void SetCapacityText(string text)
    {
        capacityText.text = text;
    }
}
