using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

public class TurretAmmo : StandardAmmo
{
    public override int Damage { get => GetDamage();  }
    private int GetDamage()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.TurretDamage;
            case 2:
                return Settings.World2.TurretDamage;
        }
        return 50;
    }
}
