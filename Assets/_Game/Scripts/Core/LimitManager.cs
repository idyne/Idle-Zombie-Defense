using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class LimitManager
{
    public static int GetTNTUpgradeLimit()
    {
        if (Day < 4) return 1;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetFrostUpgradeLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetBarbwireUpgradeLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetTurretUpgradeLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetTNTLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetFrostLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetBarbwireLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }

    public static int GetTurretLimit()
    {
        if (Day < 14) return 0;
        return 4;
    }

    public static int GetBaseDefenseLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }

    public static int GetSoldierMergeLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }

    public static int GetThrowableWeaponsGuyLevelLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }
    public static int GetAirstrikeLevelLimit()
    {
        if (Day < 4) return 2;
        if (Day < 8) return 3;
        if (Day < 13) return 4;
        if (Day < 17) return 5;
        return 6;
    }

    public static int GetMaxFireRateLevel()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.MaxFireRateLevel;
            case 2:
                return Settings.World2.MaxFireRateLevel;
        }
        return 1;
    }
}
