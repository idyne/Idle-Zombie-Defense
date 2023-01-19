using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Financier : MonoBehaviour
{
    private float baseIncomeCost = 50.37f;
    private int incomeLevel { get => PlayerProgression.PlayerData.IncomeLevel; set => PlayerProgression.PlayerData.IncomeLevel = value; }

    private void Awake()
    {
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            ButtonManager.Instance.UpdateIncomeButton(GetIncomeCost());
        });
    }

    private int GetIncomeCost()
    {
        float result = baseIncomeCost * (incomeLevel);
        switch (WaveController.ZoneLevel)
        {
            case 3:
                result *= 1.5f;
                break;
            case 4:
                result *= 1.5f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(result);
    }

    public void IncreaseIncome()
    {
        int cost = GetIncomeCost();
        if (!PlayerProgression.CanAfford(cost)) return;
        incomeLevel++;
        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }
}
