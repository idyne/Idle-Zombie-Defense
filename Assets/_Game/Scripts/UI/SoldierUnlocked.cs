using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SoldierUnlocked : MonoBehaviour
{
    [SerializeField] private List<GameObject> UIToHide;
    [SerializeField] private List<GameObject> soldiers;
    [SerializeField] private List<string> soldierNames;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI soldierName = null;

    public void Show(int soldierLevel)
    {
        gameObject.SetActive(true);
        button.SetActive(false);
        soldierName.text = soldierNames[soldierLevel];
        for (int i = 0; i < UIToHide.Count; i++)
        {
            UIToHide[i].SetActive(false);
        }
        for (int i = 4; i < soldiers.Count; i++)
        {
            soldiers[i]?.SetActive(false);
        }
        soldiers[soldierLevel].SetActive(true);
        DOVirtual.DelayedCall(1, () => { button.SetActive(true); }, false);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < UIToHide.Count; i++)
        {
            UIToHide[i].SetActive(true);
        }

    }
}
