using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public static class LimitManager
{

    public static int GetTrapLimit()
    {
        if (CycleDay < 4) return 1;
        if (CycleDay < 8) return 2;
        if (CycleDay < 13) return 3;
        if (CycleDay < 17) return 4;
        return 5;
    }

    public static int GetTurretLimit()
    {
        if (CycleDay < 14) return 0;
        return 4;
    }

    public static int GetBaseDefenseLimit()
    {
        if (CycleDay < 4) return 1;
        if (CycleDay < 8) return 2;
        if (CycleDay < 13) return 3;
        if (CycleDay < 17) return 4;
        return 5;
    }

    public static int GetSoldierMergeLimit()
    {
        if (CycleDay < 4) return 1;
        if (CycleDay < 8) return 2;
        if (CycleDay < 13) return 3;
        if (CycleDay < 17) return 4;
        return 5;
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
