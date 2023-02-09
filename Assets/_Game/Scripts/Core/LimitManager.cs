using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class LimitManager
{
    public static int GetTNTUpgradeLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetFrostUpgradeLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetBarbwireUpgradeLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetTurretUpgradeLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetTNTLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetFrostLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetBarbwireLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }

    public static int GetTurretLimit()
    {
        if (CycleDay < 14) return 0;
        return 4;
    }

    public static int GetBaseDefenseLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }

    public static int GetSoldierMergeLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }

    public static int GetThrowableWeaponsGuyLevelLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
        return 6;
    }
    public static int GetAirstrikeLevelLimit()
    {
        if (CycleDay < 4) return 2;
        if (CycleDay < 8) return 3;
        if (CycleDay < 13) return 4;
        if (CycleDay < 17) return 5;
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
