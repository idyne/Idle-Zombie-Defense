using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIUpgradeButton : UIButton
{
    [SerializeField] private TextMeshProUGUI levelText;

    public void SetLevel(int level)
    {
        levelText.text = "Level " + level;
    }
}
