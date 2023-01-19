using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Financier : MonoBehaviour
{
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
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.IncomeCost;
            case 2:
                return Settings.Zone2.IncomeCost;
            case 3:
                return Settings.Zone3.IncomeCost;
            case 4:
                return Settings.Zone4.IncomeCost;
        }
        return 1;
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
