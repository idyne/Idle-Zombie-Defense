using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class CostManager
{

    public static int GetFrostPrice()
    {
        return Settings.FrostBombCost;
    }

    public static int GetBarbwirePrice()
    {
        return Settings.BarbwireCost;
    }

    public static int GetTurretPrice()
    {
        return Settings.TurretCost;
    }

    public static int GetTNTPrice()
    {
        return Settings.ExplosiveBombCost;
    }

    public static int GetBaseDefenseLevelPrice()
    {
        return Settings.BaseDefenseLevelCost;
    }
    public static int GetThrowableWeaponsGuyLevelPrice()
    {
        return Settings.CommanderLevelPrice;
    }
    public static int GetAirstrikePrice()
    {
        return Settings.AirstrikeLevelPrice;
    }
    public static int GetTNTUpgradePrice()
    {
        return Settings.TNTLevelCost;
    }
    public static int GetFrostUpgradePrice()
    {
        return Settings.FrostLevelCost;
    }
    public static int GetBarbwireUpgradePrice()
    {
        return Settings.BarbwireLevelCost;
    }
    public static int GetTurretUpgradePrice()
    {
        return Settings.TurretLevelCost;
    }

    public static int GetSoldierMergeLevelPrice()
    {
        return Settings.SoldierMergeLevelCost;
    }

    public static int GetIncomeCost()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.IncomeCost;
            case 2:
                return Settings.World2.IncomeCost;
        }
        return 1;
    }


    public static int GetMergeCost()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.MergeCost;
            case 2:
                return Settings.World2.MergeCost;
        }
        return 1;
    }

    public static int GetSoldierCost()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.SoldierCost;
            case 2:
                return Settings.World2.SoldierCost;
            default:
                return 1;
        }
    }


    public static int GetFireRateCost()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.FireRateCost;
            case 2:
                return Settings.World2.FireRateCost;
        }
        return 1;
    }
}
