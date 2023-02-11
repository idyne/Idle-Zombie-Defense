using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LevelManager;
public class UIUpgradeButton : UIButton
{
    [SerializeField] private GameObject levelContainer;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject lockedContainer, unlockedContainer;
    [SerializeField] private TextMeshProUGUI unlockDayText;

    public void SetLevel(int level)
    {
        if (level <= 0)
        {
            //levelContainer.SetActive(level > 0);
            levelText.text = "Locked";
        }
        else
        {
            levelText.text = "Level " + level;
        }
    }
    public void Lock(int unlockDay)
    {
        unlockedContainer.SetActive(false);
        lockedContainer.SetActive(true);
        int cycleDay = GetCycleDay(unlockDay);
        int worldLevel = GetWorldLevel(cycleDay);
        if (worldLevel > WorldLevel)
        {
            unlockDayText.text = "Unlocks in the World " + worldLevel.ToString();
        }
        else
        {
            unlockDayText.text = "Unlocks in the Day " + cycleDay.ToString();
        }
    }
    public void Unlock()
    {
        unlockedContainer.SetActive(true);
        lockedContainer.SetActive(false);
    }
}
