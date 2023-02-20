using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static LevelManager;
using UnityEngine.Events;

public class TowerController : MonoBehaviour
{
    private static TowerController instance = null;
    private Tower currentTower = null;
    public UnityEvent<Tower> OnTowerChange { get; private set; } = new();
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
        if (currentTower && currentTower.TowerWorldLevel == WorldLevel && currentTower.TowerZoneLevel == ZoneLevel)
            return currentTower;
        currentTower = towers.Find((tower) => tower.TowerWorldLevel == WorldLevel && tower.TowerZoneLevel == ZoneLevel);
        OnTowerChange.Invoke(currentTower);
        return currentTower;
    }
}
