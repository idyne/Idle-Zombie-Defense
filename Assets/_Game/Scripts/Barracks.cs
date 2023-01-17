using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;
using System;
using UnityEngine.Events;

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
    private float baseSoldierCost { get => 2.47f * Mathf.Pow(2.5f, PlayerProgression.PlayerData.SoldierMergeLevel - 1); }
    private float baseFireRateCost = 10.13f;
    private float baseMergeCost = 15.14f;
    private int power = 0;
    private int lastWaveLevelOfBuySoldier = 1;
    private bool isMerging = false;
    private int mergingSoldiersPower = 0;
    private bool isFull { get => tower.NumberOfPoints <= numberOfSoldiers; }
    public static int FireRateLevel { get => PlayerProgression.PlayerData.FireRateLevel; }
    private int maxMergeLevel = 0;
    public readonly UnityEvent<int> OnNewMergeUnlocked = new();

    private void Awake()
    {
        tower = GetComponent<Tower>();
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            ButtonManager.Instance.UpdateMergeButton(GetMergeCost(), CanMerge(out _));
            ButtonManager.Instance.UpdateSoldierButton(GetSoldierCost(), isFull, isMerging);
            ButtonManager.Instance.UpdateFireRateButton(GetFireRateCost(), FireRateLevel >= GetMaxFireRateLevel());
        });
        UpgradeController.Instance.OnSoldierMergeLevelUpgrade.AddListener(ClearLowLevelSoldiers);

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

    public static int GetMaxFireRateLevel()
    {
        int result = 1;
        switch (WaveController.ZoneLevel)
        {
            case 1:
                result = 14;
                break;
            case 2:
                result = 28;
                break;
            case 3:
                result = 40;
                break;
            case 4:
                result = 40;
                break;
        }
        return result;
    }
    private void Start()
    {
        Initialize();
        ButtonManager.Instance.UpdateMergeButton(GetMergeCost(), CanMerge(out _));
        ButtonManager.Instance.UpdateSoldierButton(GetSoldierCost(), isFull, isMerging);
        for (int i = soldierTable.Count - 1; i >= 0; i--)
        {
            if (soldierTable[i].Count > 0)
            {
                maxMergeLevel = i;
                break;
            }
        }
    }

    private void Initialize()
    {
        lastWaveLevelOfBuySoldier = WaveController.NormalizedDay;
        for (int i = 1; i < PlayerProgression.PlayerData.Soldiers.Count; i++)
        {
            int number = PlayerProgression.PlayerData.Soldiers[i];
            for (int j = 0; j < number; j++)
                AddSoldier(i, out _, out _, false);
        }
    }

    private int GetMergeCost()
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
        return Mathf.RoundToInt(baseMergeCost * (totalMerge + 1));
    }

    public int GetSoldierCost()
    {
        return Mathf.CeilToInt(
            (baseSoldierCost * ((power - 1) * 2f)) +
            (baseSoldierCost * (lastWaveLevelOfBuySoldier / 3f)) +
            (baseSoldierCost)
            );
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
    }

    public void BuySoldier()
    {
        int cost = GetSoldierCost();
        if (!PlayerProgression.CanAfford(cost)) return;
        int soldierLevel = PlayerProgression.PlayerData.SoldierMergeLevel;
        AddSoldier(soldierLevel, out _, out _);
        lastWaveLevelOfBuySoldier = WaveController.NormalizedDay;
        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }

    private bool AddSoldier(int level, out Soldier soldier, out Transform spawnPoint, bool save = true)
    {
        soldier = null;
        spawnPoint = tower.GetPoint(numberOfSoldiers);
        if (spawnPoint == null) return false;
        soldier = ObjectPooler.SpawnFromPool("Soldier " + level, spawnPoint.position, spawnPoint.rotation).GetComponent<Soldier>();
        while (soldierTable.Count - 1 < level)
            soldierTable.Add(new List<Soldier>());
        soldierTable[level].Add(soldier);
        numberOfSoldiers++;
        power += Mathf.RoundToInt(Mathf.Pow(3, level - 1));
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
        lastWaveLevelOfBuySoldier = 1;
    }

    private bool CanMerge(out int level)
    {
        level = -1;
        if (isMerging) return false;
        int i = 1;
        bool canMerge = false;
        int limitLevel = WaveController.ZoneLevel + 3;
        if (WaveController.ZoneLevel == 4)
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
        int cost = GetMergeCost();
        if (!PlayerProgression.CanAfford(cost) || !CanMerge(out int soldierLevel)) return;
        isMerging = true;
        int count = 0;
        Vector3 mergePosition = Vector3.up * 7;
        float transitionDuration = 0.3f;
        while (count++ < 3)
        {
            Soldier soldier = RemoveSoldier(soldierLevel);
            power += Mathf.RoundToInt(Mathf.Pow(3, soldierLevel - 1)); ;
            void hide() => soldier.gameObject.SetActive(false);
            void hideAndAddSoldier()
            {
                hide();

                ObjectPooler.SpawnFromPool("Smoke Effect", mergePosition, Quaternion.identity);
                power -= Mathf.RoundToInt(Mathf.Pow(3, soldierLevel));
                AddSoldier(soldierLevel + 1, out Soldier newSoldier, out Transform spawnPoint);
                Vector3 position = newSoldier.Transform.position;
                newSoldier.Transform.position = mergePosition;
                newSoldier.Transform.DOMove(position, transitionDuration);
                isMerging = false;
                bool isNewMerge = soldierLevel + 1 > maxMergeLevel;
                if (isNewMerge)
                {
                    maxMergeLevel = soldierLevel + 1;
                    OnNewMergeUnlocked.Invoke(soldierLevel + 1);
                }
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
        power -= Mathf.RoundToInt(Mathf.Pow(3, level - 1));
        PlayerProgression.PlayerData.Soldiers[level]--;
        return soldier;
    }

    private void PlaceSoldiers()
    {
        int count = 0;
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

    private int GetFireRateCost()
    {
        float result = baseFireRateCost * (FireRateLevel) * 4;
        switch (WaveController.ZoneLevel)
        {
            case 2:
                result *= 2;
                break;
            case 3:
                result *= 3;
                break;
            case 4:
                result *= 4;
                break;
        }
        return Mathf.CeilToInt(result);
    }

    public void BuyFireRate()
    {
        int cost = GetFireRateCost();
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
