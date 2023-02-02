using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Financier : MonoBehaviour
{
    private int incomeLevel { get => PlayerProgression.PlayerData.IncomeLevel; set => PlayerProgression.PlayerData.IncomeLevel = value; }

    public void IncreaseIncome()
    {
        int cost = CostManager.GetIncomeCost();
        if (!PlayerProgression.CanAfford(cost)) return;
        incomeLevel++;
        PlayerProgression.MONEY -= cost;
        HapticManager.DoHaptic();
    }
}
