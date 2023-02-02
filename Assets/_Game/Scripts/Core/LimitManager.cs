using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LimitManager
{

    public static int GetTrapLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TrapCapacityLimit;
            case 2:
                return Settings.Zone2.TrapCapacityLimit;
            case 3:
                return Settings.Zone3.TrapCapacityLimit;
            case 4:
                return Settings.Zone4.TrapCapacityLimit;
        }
        return 1;
    }

    public static int GetTurretLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.TurretCapacityLimit;
            case 2:
                return Settings.Zone2.TurretCapacityLimit;
            case 3:
                return Settings.Zone3.TurretCapacityLimit;
            case 4:
                return Settings.Zone4.TurretCapacityLimit;
        }
        return 1;
    }

    public static int GetBaseDefenseLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.BaseDefenseLimit;
            case 2:
                return Settings.Zone2.BaseDefenseLimit;
            case 3:
                return Settings.Zone3.BaseDefenseLimit;
            case 4:
                return Settings.Zone4.BaseDefenseLimit;
        }
        return 1;
    }

    public static int GetSoldierMergeLimit()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.SoldierMergeLevelLimit;
            case 2:
                return Settings.Zone2.SoldierMergeLevelLimit;
            case 3:
                return Settings.Zone3.SoldierMergeLevelLimit;
            case 4:
                return Settings.Zone4.SoldierMergeLevelLimit;
        }
        return 1;
    }

    public static int GetMaxFireRateLevel()
    {
        switch (WaveController.ZoneLevel)
        {
            case 1:
                return Settings.Zone1.MaxFireRateLevel;
            case 2:
                return Settings.Zone2.MaxFireRateLevel;
            case 3:
                return Settings.Zone3.MaxFireRateLevel;
            case 4:
                return Settings.Zone4.MaxFireRateLevel;
        }
        return 1;
    }
}
