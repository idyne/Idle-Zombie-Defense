using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;
using System;
using UnityEngine.Events;
using static LevelManager;

[RequireComponent(typeof(Tower))]
public class Barracks : MonoBehaviour
{
    private static Barracks instance;
    public static Barracks Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Barracks>();
            return instance;
        }
    }
    private Tower tower;
    private List<List<Soldier>> soldierTable = new List<List<Soldier>> { null, new List<Soldier>(), new List<Soldier>(), new List<Soldier>(), new List<Soldier>(), new List<Soldier>(), new List<Soldier>(), new List<Soldier>(), new List<Soldier>() };
    private int numberOfSoldiers = 0;
    public int Power { get; private set; } = 0;
    public bool Merging { get; private set; } = false;
    private int mergingSoldiersPower = 0;
    public bool Full { get => tower.NumberOfPoints <= numberOfSoldiers; }
    public static int FireRateLevel { get => PlayerProgression.PlayerData.FireRateLevel; }
    private int maxMergeLevel = 0;
    public readonly UnityEvent<int> OnNewMergeUnlocked = new();
    private ThrowableWeaponsGuy throwableWeaponsGuy = null;


    private void Awake()
    {
        tower = GetComponent<Tower>();
        UpgradeController.OnSoldierMergeLevelUpgrade.AddListener(ClearLowLevelSoldiers);
    }

    private void Start()
    {
        for (int i = soldierTable.Count - 1; i >= 1; i--)
        {
            if (soldierTable[i].Count > 0)
            {
                maxMergeLevel = i;
                break;
            }
        }
        SpawnThrowableWeaponsGuy();
    }

    public void SpawnThrowableWeaponsGuy()
    {
        if (throwableWeaponsGuy) return;
        throwableWeaponsGuy = Instantiate(PrefabManager.Prefabs["Throwable Weapons Guy"], tower.GetPoint(0).position, Quaternion.identity).GetComponent<ThrowableWeaponsGuy>();
    }

    private void ClearLowLevelSoldiers(int level)
    {
        bool isNewMerge = false;
        for (int i = 1; i < level; i++)
        {
            int numberOfSoldiers = soldierTable[i].Count;
            if (numberOfSoldiers > 0)
            {
                for (int j = 0; j < numberOfSoldiers; j++)
                {
                    RemoveSoldier(i);
                }
                int numberOfHighLevelSoldiers = Mathf.CeilToInt(numberOfSoldiers / 3f);
                for (int j = 0; j < numberOfHighLevelSoldiers; j++)
                {
                    if (i + 1 > maxMergeLevel)
                    {
                        maxMergeLevel = i + 1;
                        isNewMerge = true;
                        print(maxMergeLevel);
                    }
                    AddSoldier(i + 1, out Soldier soldier, out Transform spawnPoint);
                }
            }
        }
        if (isNewMerge)
        {
            print(maxMergeLevel);

            OnNewMergeUnlocked.Invoke(maxMergeLevel);
        }

    }


    public int TotalMerge
    {
        get
        {
            int totalMerge = 0;
            int[] merges = new int[] { 0, 0, 1, 4, 13, 40, 121, 364, 1093 };
            for (int i = 2; i < soldierTable.Count; i++)
            {
                for (int j = 0; j < soldierTable[i].Count; j++)
                {
                    totalMerge += merges[i];
                }
            }
            return totalMerge;
        }
    }


    public void DeactivateSoldiers()
    {
        for (int i = 1; i < soldierTable.Count; i++)
        {
            for (int j = 0; j < soldierTable[i].Count; j++)
            {
                soldierTable[i][j].ActivateRagdoll();
                soldierTable[i][j].Deactivate();
            }
        }
        throwableWeaponsGuy.ActivateRagdoll();
        throwableWeaponsGuy.Deactivate();
    }

    public void BuySoldier()
    {
        int cost = CostManager.GetSoldierCost();
        if (!PlayerProgression.CanAfford(cost)) return;
        int soldierLevel = PlayerProgression.PlayerData.SoldierMergeLevel;
        AddSoldier(soldierLevel, out _, out _);
        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }

    public bool AddSoldier(int level, out Soldier soldier, out Transform spawnPoint, bool save = true)
    {
        soldier = null;
        spawnPoint = tower.GetPoint(numberOfSoldiers);
        if (spawnPoint == null) return false;
        soldier = ObjectPooler.SpawnFromPool("Soldier " + level, spawnPoint.position, spawnPoint.rotation).GetComponent<Soldier>();
        while (soldierTable.Count - 1 < level)
            soldierTable.Add(new List<Soldier>());
        soldierTable[level].Add(soldier);
        numberOfSoldiers++;
        Power += Mathf.RoundToInt(Mathf.Pow(3, level - 1));
        PlaceSoldiers();
        soldier.SetTarget();
        if (save) PlayerProgression.PlayerData.Soldiers[level]++;
        return true;
    }

    public void ClearSoldiers()
    {
        for (int i = 1; i < soldierTable.Count; i++)
        {
            List<Soldier> soldiers = soldierTable[i];
            while (soldiers.Count > 0)
            {
                RemoveSoldier(i);
            }
        }
        AddSoldier(1, out _, out _);
    }

    public bool CanMerge(out int level)
    {
        level = -1;
        if (Merging) return false;
        int i = 1;
        bool canMerge = false;
        int limitLevel = ZoneLevel + 3;
        if (ZoneLevel == 4)
            limitLevel = 8;
        while (i < limitLevel)
        {
            if (soldierTable[i].Count >= 3)
            {
                canMerge = true;
                break;
            }
            i++;
        }
        level = i;
        if (level == 8) return false;
        return canMerge;
    }

    public void Merge()
    {
        int cost = CostManager.GetMergeCost();
        if (!PlayerProgression.CanAfford(cost) || !CanMerge(out int soldierLevel)) return;
        Merging = true;
        int count = 0;
        Vector3 mergePosition = Vector3.up * 7;
        float transitionDuration = 0.3f;
        while (count++ < 3)
        {
            Soldier soldier = RemoveSoldier(soldierLevel);
            Power += Mathf.RoundToInt(Mathf.Pow(3, soldierLevel - 1)); ;
            void hide() => soldier.gameObject.SetActive(false);
            void hideAndAddSoldier()
            {
                hide();

                ObjectPooler.SpawnFromPool("Smoke Effect", mergePosition, Quaternion.identity);
                Power -= Mathf.RoundToInt(Mathf.Pow(3, soldierLevel));
                AddSoldier(soldierLevel + 1, out Soldier newSoldier, out Transform spawnPoint);
                Vector3 position = newSoldier.Transform.position;
                newSoldier.Transform.position = mergePosition;
                bool isNewMerge = soldierLevel + 1 > maxMergeLevel;
                if (isNewMerge)
                {
                    maxMergeLevel = soldierLevel + 1;

                }
                newSoldier.Transform.DOMove(position, transitionDuration).OnComplete(() =>
                {
                    if (isNewMerge)
                        DOVirtual.DelayedCall(0.4f, () => { OnNewMergeUnlocked.Invoke(soldierLevel + 1); });
                });
                Merging = false;

                PlayerProgression.MONEY = PlayerProgression.MONEY;
            }

            soldier.gameObject.SetActive(true);
            soldier.Transform.DOMove(mergePosition, transitionDuration).OnComplete(count == 3 ? hideAndAddSoldier : hide);
        }



        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }
    private Soldier RemoveSoldier(int level)
    {
        List<Soldier> soldiers = soldierTable[level];
        if (soldiers == null || soldiers.Count == 0) return null;
        Soldier soldier = soldiers[^1];
        soldier.Deactivate();
        soldiers.RemoveAt(soldiers.Count - 1);
        PlaceSoldiers();
        numberOfSoldiers--;
        Power -= Mathf.RoundToInt(Mathf.Pow(3, level - 1));
        PlayerProgression.PlayerData.Soldiers[level]--;
        return soldier;
    }

    public void PlaceSoldiers()
    {
        int count = 1;
        for (int i = soldierTable.Count - 1; i >= 1; i--)
        {
            List<Soldier> soldiers = soldierTable[i];
            for (int j = 0; j < soldiers.Count; j++)
            {
                Soldier soldier = soldiers[j];
                Transform point = tower.GetPoint(count++);
                soldier.Transform.SetPositionAndRotation(point.position, soldier.Transform.rotation);
            }
        }
    }


    public void BuyFireRate()
    {
        int cost = CostManager.GetFireRateCost();
        if (!PlayerProgression.CanAfford(cost)) return;
        IncreaseFireRate();
        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }

    private void IncreaseFireRate()
    {
        PlayerProgression.PlayerData.FireRateLevel++;
        for (int i = 1; i < soldierTable.Count; i++)
        {
            for (int j = 0; j < soldierTable[i].Count; j++)
            {
                soldierTable[i][j].PlayFireRateEffect();
            }
        }
    }


}
