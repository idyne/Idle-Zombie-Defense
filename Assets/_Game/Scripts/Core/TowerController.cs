using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static LevelManager;

public class TowerController : MonoBehaviour
{
    private static TowerController instance = null;
    public static TowerController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TowerController>();
            return instance;
        }
    }
    [SerializeField] private List<Tower> towers;

    private void Start()
    {
        for (int i = 0; i < towers.Count; i++)
        {
            towers[i].Deactivate();
        }
        GetCurrentTower().Activate();
    }

    public Tower GetCurrentTower()
    {
        Tower currentTower = towers.Find((tower) => tower.TowerWorldLevel == WorldLevel && tower.TowerZoneLevel == ZoneLevel);
        return currentTower;
    }
}
