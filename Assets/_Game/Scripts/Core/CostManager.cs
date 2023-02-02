using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CostManager
{

    public static int GetFrostPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.FrostBombCost;
            case 2:
                return Settings.Zone2.FrostBombCost;
            case 3:
                return Settings.Zone3.FrostBombCost;
            case 4:
                return Settings.Zone4.FrostBombCost;
        }
        return 1;
    }

    public static int GetTurretPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCost;
            case 2:
                return Settings.Zone2.TurretCost;
            case 3:
                return Settings.Zone3.TurretCost;
            case 4:
                return Settings.Zone4.TurretCost;
        }
        return 1;
    }

    public static int GetTNTPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.ExplosiveBombCost;
            case 2:
                return Settings.Zone2.ExplosiveBombCost;
            case 3:
                return Settings.Zone3.ExplosiveBombCost;
            case 4:
                return Settings.Zone4.ExplosiveBombCost;
        }
        return 1;
    }

    public static int GetBaseDefenseLevelPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.BaseDefenseLevelCost;
            case 2:
                return Settings.Zone2.BaseDefenseLevelCost;
            case 3:
                return Settings.Zone3.BaseDefenseLevelCost;
            case 4:
                return Settings.Zone4.BaseDefenseLevelCost;
        }
        return 1;
    }
    public static int GetTrapCapacityPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TrapCapacityCost;
            case 2:
                return Settings.Zone2.TrapCapacityCost;
            case 3:
                return Settings.Zone3.TrapCapacityCost;
            case 4:
                return Settings.Zone4.TrapCapacityCost;
        }
        return 1;
    }
    public static int GetTurretCapacityPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCapacityCost;
            case 2:
                return Settings.Zone2.TurretCapacityCost;
            case 3:
                return Settings.Zone3.TurretCapacityCost;
            case 4:
                return Settings.Zone4.TurretCapacityCost;
        }
        return 1;
    }
    public static int GetSoldierMergeLevelPrice()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.SoldierMergeLevelCost;
            case 2:
                return Settings.Zone2.SoldierMergeLevelCost;
            case 3:
                return Settings.Zone3.SoldierMergeLevelCost;
            case 4:
                return Settings.Zone4.SoldierMergeLevelCost;
        }
        return 1;
    }

    public static int GetIncomeCost()
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


    public static int GetMergeCost()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.MergeCost;
            case 2:
                return Settings.Zone2.MergeCost;
            case 3:
                return Settings.Zone3.MergeCost;
            case 4:
                return Settings.Zone4.MergeCost;
        }
        return 1;
    }

    public static int GetSoldierCost()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.SoldierCost;
            case 2:
                return Settings.Zone2.SoldierCost;
            case 3:
                return Settings.Zone3.SoldierCost;
            case 4:
                return Settings.Zone4.SoldierCost;
            default:
                return 1;
        }
    }


    public static int GetFireRateCost()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.FireRateCost;
            case 2:
                return Settings.Zone2.FireRateCost;
            case 3:
                return Settings.Zone3.FireRateCost;
            case 4:
                return Settings.Zone4.FireRateCost;
        }
        return 1;
    }
}
