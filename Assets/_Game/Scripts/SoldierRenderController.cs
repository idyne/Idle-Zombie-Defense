using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierRenderController : MonoBehaviour
{
    public static SoldierRenderController Instance = null;

    [SerializeField] private float rotateSpeed = 10;
    [SerializeField] private GameObject[] soldiers = null;
    [SerializeField] private Transform soldierParent = null;

    private int lastRenderedSoldierIndex = 1;

    void Awake()
    {
        Instance = this;
    }


    public void Render(int soldierIndex)
    {
        soldiers[lastRenderedSoldierIndex].SetActive(false);
        soldiers[soldierIndex].SetActive(true);
        lastRenderedSoldierIndex = soldierIndex;
    }
}
