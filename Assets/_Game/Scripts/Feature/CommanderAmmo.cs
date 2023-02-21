using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

public class CommanderAmmo : StandardAmmo
{
    public override int Damage { get => GetDamage(); }
    private int GetDamage()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.CommanderWeaponDamage;
            case 2:
                return Settings.World2.CommanderWeaponDamage;
        }
        return 50;
    }
}
