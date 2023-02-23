using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class LimitManager
{
    public static int GetTNTUpgradeLimit()
    {
        return 10;
    }
    public static int GetBarbwireUpgradeLimit()
    {
        return 8;
    }
    public static int GetFrostUpgradeLimit()
    {
        return 6;
    }
    public static int GetTurretUpgradeLimit()
    {
        return 5;
    }

    
    


    public static int GetTNTLimit()
    {
        if (Day < 2) return 0;
        if (Day < 4) return 1;
        return 2;
    }
    public static int GetFrostLimit()
    {
        if (Day < 9) return 0;
        if (Day < 13) return 1;
        return 2;
    }
    public static int GetBarbwireLimit()
    {
        if (Day < 6) return 0;
        if (Day < 8) return 1;
        return 2;
    }

    public static int GetTurretLimit()
    {
        if (Day < 14) return 0;
        if (Day < 17) return 1;
        return 2;
    }

    public static int GetBaseDefenseLimit()
    {
        if (Day < 4) return 3;
        if (Day < 8) return 5;
        if (Day < 13) return 8;
        if (Day < 17) return 11;
        return 15;
    }

    public static int GetSoldierMergeLimit()
    {
        if (Day < 4) return 0;
        if (Day < 8) return 1;
        if (Day < 13) return 2;
        if (Day < 17) return 3;
        return 4;
    }

    public static int GetThrowableWeaponsGuyLevelLimit()
    {
        if (Day < 4) return 0;
        if (Day < 8) return 1;
        return 2;
    }
    public static int GetAirstrikeLevelLimit()
    {
        if (Day < 13) return 0;
        if (Day < 17) return 1;
        return 2;
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
