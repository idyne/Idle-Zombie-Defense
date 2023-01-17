using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    [System.Serializable]
    public class PlayerData : Data
    {
        public int CurrentLevel = 1;
        public int Money = 100000;
        public List<int> Soldiers = new() { 0, 3, 0, 0, 0, 0, 0, 0, 0 };
        public int WaveLevel = 67;
        public int IncomeLevel = 1;
        public int FireRateLevel = 1;
        public int BaseDefenseLevel = 1;
        public int TrapCapacity = 0;
        public int TurretCapacity = 0;
        public int SoldierMergeLevel = 1;
        public List<(int, int, bool)> Traps = new() {  };
        public List<int> Turrets = new() {  };
    }



}
