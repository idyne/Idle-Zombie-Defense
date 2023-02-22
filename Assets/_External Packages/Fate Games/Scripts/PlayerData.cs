using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    [System.Serializable]
    public class PlayerData : Data
    {
        public int CurrentLevel = 1;
        public int Money = 50000;
        public int UpgradePoint = 0;
        public List<int> Soldiers = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int WaveLevel = 49;
        public int IncomeLevel = 1;
        public int FireRateLevel = 1;
        public int BaseDefenseLevel = 1;
        public int TNTLevel = 1;
        public int FrostLevel = 1;
        public int BarbwireLevel = 1;
        public int TurretLevel = 1;
        public int SoldierMergeLevel = 1;
        public int ThrowableWeaponsGuyLevel = 2;
        public int AirstrikeLevel = 2;
        public List<(int, int, bool)> Traps = new() { };
        public List<int> Turrets = new() { };
        public bool HasEverDetonated = false;
        public bool HasEverPlaced = false;
        public bool HasEverAimed = false;
    }



}
