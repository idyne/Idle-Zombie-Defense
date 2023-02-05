using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class CostManager
{

    public static int GetFrostPrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.FrostBombCost;
            case 2:
                return Settings.World2.FrostBombCost;
        }
        return 1;
    }

    public static int GetBarbwirePrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.BarbwireCost;
            case 2:
                return Settings.World2.BarbwireCost;
        }
        return 1;
    }

    public static int GetTurretPrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.TurretCost;
            case 2:
                return Settings.World2.TurretCost;
        }
        return 1;
    }

    public static int GetTNTPrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.ExplosiveBombCost;
            case 2:
                return Settings.World2.ExplosiveBombCost;
        }
        return 1;
    }

    public static int GetBaseDefenseLevelPrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.BaseDefenseLevelCost;
            case 2:
                return Settings.World2.BaseDefenseLevelCost;
        }
        return 1;
    }
    public static int GetTNTUpgradePrice()
    {
        //TODO
        return 1;
    }
    public static int GetFrostUpgradePrice()
    {
        //TODO
        return 1;
    }
    public static int GetBarbwireUpgradePrice()
    {
        //TODO
        return 1;
    }
    public static int GetTurretUpgradePrice()
    {
        //TODO
        return 1;
    }

    public static int GetSoldierMergeLevelPrice()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.SoldierMergeLevelCost;
            case 2:
                return Settings.World2.SoldierMergeLevelCost;
        }
        return 1;
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
