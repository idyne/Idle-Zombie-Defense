using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    [System.Serializable]
    public class PlayerData : Data
    {
        public int CurrentLevel = 1;
        public int Money = 100;
        public List<int> Soldiers = new() { 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        public int WaveLevel = 5;
        public int IncomeLevel = 1;
        public int FireRateLevel = 1;
        public List<(int, int, bool)> Traps = new() { (0, 100, false), (1, 101, true) };
        public List<int> Turrets = new() { 200 };
    }



}
