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
        return Mathf.RoundToInt(baseIncomeCost * (incomeLevel));
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
