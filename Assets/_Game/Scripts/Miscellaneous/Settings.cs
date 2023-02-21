using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static FateGames.PlayerProgression;
using static LevelManager;

public static class Settings
{
    public static int BaseDefenseLevelCost { get => Mathf.CeilToInt(PlayerData.BaseDefenseLevel * 5); }
    public static int SoldierMergeLevelCost { get => Mathf.CeilToInt(PlayerData.SoldierMergeLevel * 50); }
    public static int CommanderLevelPrice { get => Mathf.CeilToInt((PlayerData.ThrowableWeaponsGuyLevel + 1) * 50); }
    public static int AirstrikeLevelPrice { get => Mathf.CeilToInt((PlayerData.AirstrikeLevel + 1) * 50); }
    public static int TNTLevelCost { get => Mathf.CeilToInt(PlayerData.TNTLevel * 50); }
    public static int FrostLevelCost { get => Mathf.CeilToInt(PlayerData.FrostLevel * 50); }
    public static int BarbwireLevelCost { get => Mathf.CeilToInt(PlayerData.BarbwireLevel * 50); }
    public static int TurretLevelCost { get => Mathf.CeilToInt(PlayerData.TurretLevel * 50); }
    public static int ExplosiveBombCost { get => Mathf.CeilToInt(NumberOfExplosiveBombs * 100 + 5); }
    public static int FrostBombCost { get => Mathf.CeilToInt(NumberOfFrostBombs * 100 + 5); }
    public static int BarbwireCost { get => Mathf.CeilToInt(NumberOfBarbwires * 100 + 5); }
    public static int TurretCost { get => Mathf.CeilToInt(NumberOfTurrets * 800 + 200); }
    public static int BarbwireDPS { get => Mathf.CeilToInt(PlayerData.BarbwireLevel * 50 * (WorldDay / 3 + 1)); }
    public static int TNTDamage { get => Mathf.CeilToInt(PlayerData.TNTLevel * 50 * (WorldDay / 3 + 1)); }
    public static int FrostDuration { get => Mathf.CeilToInt(PlayerData.FrostLevel * 10); }
    public static int BarbwireMaxDamage { get => BarbwireDPS * 6; }
    public static float ThrowableWeaponCooldown { get => 20f / (PlayerData.ThrowableWeaponsGuyLevel / 2f); }
    public static float AirstrikeCooldown { get => 30f / (PlayerData.AirstrikeLevel / 2f); }
    public static readonly int FrostUnlockDay = 9;
    public static readonly int TurretUnlockDay = 14;
    public static readonly int TNTUnlockDay = 2;
    public static readonly int BarbwireUnlockDay = 6;
    public static readonly int AirstrikeUnlockDay = 13;
    public static readonly int ThrowableWeaponsUnlockDay = 4;
    public static bool SoundOn { get => soundOn; set { soundOn = value; OnSoundChange.Invoke(soundOn); } }
    private static bool soundOn = true;
    public static UnityEvent<bool> OnSoundChange { get; private set; } = new();

    private static int NumberOfFrostBombs { get => PlayerData.Traps.Where(trapData => trapData.Item1 == 1).Count(); }
    private static int NumberOfBarbwires { get => PlayerData.Traps.Where(trapData => trapData.Item1 == 2).Count(); }
    private static int NumberOfTurrets { get => PlayerData.Turrets.Count; }
    private static int NumberOfExplosiveBombs { get => PlayerData.Traps.Where(trapData => trapData.Item1 == 0).Count(); }

    public static int[][] ZoneLengths = new int[][] { new int[] { 3, 4, 5 }, new int[] { 4, 4, 5 } };
    public static int[][] CumulativeZoneLengths = new int[][] { new int[] { 3, 7, 12 }, new int[] { 16, 20, 25 } };
    public static int NumberOfDays { get => CumulativeZoneLengths[^1][^1]; }

    public static class World1
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 2.47f * Mathf.Pow(2.5f, PlayerData.SoldierMergeLevel - 1);
        private static readonly float BaseFireRateCost = 10.13f;
        private static readonly float BaseMergeRateCost = 15.14f;
        public static int MaxFireRateLevel = 14;
        public static int MergeCost { get => Mathf.CeilToInt(BaseMergeRateCost * (Barracks.Instance.TotalMerge + 1)); }
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * ((Barracks.Instance.Power + 1) * 2f)); }
        public static int FireRateCost { get => Mathf.CeilToInt(BaseFireRateCost * (PlayerData.FireRateLevel) * 4); }
        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 30000;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (WorldDay + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                return Mathf.CeilToInt(result);
            }
        }
        public static int TowerMaxHealth
        {
            get
            {
                float result = BaseTowerHealth * PlayerData.BaseDefenseLevel;
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                return Mathf.CeilToInt(result);
            }
        }
        #endregion
        #region Financier
        public static readonly float BaseIncomeCost = 50;
        public static int IncomeCost { get => Mathf.CeilToInt(BaseIncomeCost * Mathf.Pow(1.2f, PlayerData.IncomeLevel - 1)); }

        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay - 2) * 101 + 51;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (Day == 4 || Day == 8 || Day == 13)
                    money *= 2;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishDayUpgradePrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = 2;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (Day == 4 || Day == 8 || Day == 13)
                    money = 5;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishPhasePrize
        {
            get
            {
                float money = WorldDay * 10;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishPhaseUpgradePrize
        {
            get
            {
                float money = 1;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Wave
        private static readonly int BaseWavePower = 10;
        public static int WavePower
        {
            get
            {
                float result;
                float multiplier = 1.5f;
                switch (CurrentTimePeriod)
                {
                    case TimePeriod.Morning:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 1.5f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 2.25f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 3.5f;
                        break;
                    default:
                        result = BaseWavePower;
                        break;
                }
                if (Day == 1 && CurrentTimePeriod == TimePeriod.Morning)
                    result /= 2;
                /*else if (Day == 8)
                    result *= 1.3f;
                else if (Day == 22)
                    result *= 1.3f;
                else if (Day == 42)
                    result *= 1.3f;*/
                return Mathf.CeilToInt(result);
            }
        }
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((WorldDay + 0.73f) / 7f, 0, 1f) * 4); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 40;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 10;
            float WorldDayMultiplier = (WorldDay - 1) * 0.8f;
            if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + WorldDayMultiplier);
            if (boss && WorldDay == CumulativeZoneLengths[0][0])
                result *= bossMultiplier * 0.5f;
            else if (boss)
                result *= bossMultiplier;
            if (WorldDay >= 6)
                result *= 1.2f;
            if (WorldDay >= 5)
                result *= 1.4f;
            if (WorldDay > 2)
                result *= 1.2f;
            if (WorldDay > 1)
                result *= 0.9f;
            if (WorldDay == 1 && CurrentTimePeriod == TimePeriod.Morning)
                result *= 0.7f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float WorldDayMultiplier = (WorldDay - 1) * 0.8f;
            if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + WorldDayMultiplier);
            if (boss)
                result *= bossMultiplier * 0.8f;
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float baseZombieIncome = 10f;
            /*float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (WorldDay) * 2) * level;*/
            float gain = (baseZombieIncome * (Mathf.Pow(1.1f, PlayerData.IncomeLevel - 1) + WorldDay * 0.1f)) * level;
            if (boss)
            {
                Debug.Log(level);
                gain *= 2f;
            }
                
            return Mathf.CeilToInt(gain);
        }


        #endregion
        #region Turret
        public static int TurretDamage { get => Mathf.CeilToInt(PlayerData.TurretLevel * 50 * (WorldDay / 3f + 1)); }
        #endregion
        #region Commander
        public static int CommanderWeaponDamage { get => Mathf.CeilToInt(100000000 * (WorldDay / 3f + 1)); }
        public static int GrenadeDamage { get => Mathf.CeilToInt(25 * (WorldDay / 3f + 1)); }
        public static int MolotovDPS { get => Mathf.CeilToInt(50 * (WorldDay / 3f + 1)); }
        #endregion
        #region Airstrike
        public static int AirstrikeDamage { get => Mathf.CeilToInt(PlayerData.AirstrikeLevel * 50000000 * (WorldDay / 3f + 1)); }
        #endregion
    }
    public static class World2
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 2.47f * Mathf.Pow(1.75f, PlayerData.SoldierMergeLevel - 1);
        private static readonly float BaseFireRateCost = 10.13f;
        private static readonly float BaseMergeRateCost = 15.14f;
        public static int MaxFireRateLevel = 28;
        public static int MergeCost { get => Mathf.CeilToInt(BaseMergeRateCost * (Barracks.Instance.TotalMerge + 1)); }
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * ((Barracks.Instance.Power) * 2f)); }
        public static int FireRateCost { get => Mathf.CeilToInt(1.1f * BaseFireRateCost * (PlayerData.FireRateLevel) * 4); }
        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (WorldDay + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                return Mathf.CeilToInt(result);
            }
        }
        public static int TowerMaxHealth
        {
            get
            {
                float result = BaseTowerHealth * PlayerData.BaseDefenseLevel;
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                return Mathf.CeilToInt(result);
            }
        }
        #endregion
        #region Financier
        public static readonly float BaseIncomeCost = 50.37f;
        public static int IncomeCost { get => Mathf.CeilToInt(BaseIncomeCost * PlayerData.IncomeLevel); }

        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay - 2) * 101 + 51;
                money *= 1.5f;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                for (int i = 0; i < Settings.CumulativeZoneLengths.Length; i++)
                {
                    for (int j = 0; j < Settings.CumulativeZoneLengths[i].Length; j++)
                    {
                        if (CycleDay == Settings.CumulativeZoneLengths[i][j] + 1)
                        {
                            money = 1500;
                            break;
                        }
                    }
                }
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishDayUpgradePrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay - 2) * 101 + 51;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (Day == 8 || Day == 22 || Day == 42)
                    money = 1500;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishPhasePrize
        {
            get
            {
                float money = WorldDay * 20;
                money *= 1.5f;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishPhaseUpgradePrize
        {
            get
            {
                float money = WorldDay * 20;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Wave
        private static readonly int BaseWavePower = 5;
        public static int WavePower
        {
            get
            {
                float result;
                float multiplier = 1.5f;
                switch (CurrentTimePeriod)
                {
                    case TimePeriod.Morning:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 1.8f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 2.5f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((WorldDay - 1) * multiplier + 1) * 4f;
                        break;
                    default:
                        result = BaseWavePower;
                        break;
                }
                if (Day == 1)
                    result *= 2;
                else if (Day == 8)
                    result *= 1.3f;
                else if (Day == 22)
                    result *= 1.3f;
                else if (Day == 42)
                    result *= 1.3f;
                return Mathf.CeilToInt(result);
            }
        }
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((WorldDay + 4) / 14f, 0, 1f) * 5); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 400;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 8;
            float WorldDayMultiplier = (WorldDay - 1) * 0.8f;
            if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + WorldDayMultiplier);
            result *= 1.1f;
            if (boss)
                result *= bossMultiplier * 1.5f;
            if (WorldDay > 2 && WorldDay < 8)
            {
                result *= 1.0f + (WorldDay - 2) / 12f * 2f;
            }
            else if (WorldDay >= 8)
            {
                result *= 1.0f + (WorldDay - 2) / 12f * 2f;
                result *= 1.3f;
            }
            if (WorldDay > 1)
                result *= 1.2f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float WorldDayMultiplier = (WorldDay - 1) * 0.8f;
            if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + WorldDayMultiplier);
            result *= 0.5f;
            if (boss)
            {
                result *= bossMultiplier * 0.8f;
                result *= 2;
                if (WorldDay == 14)
                {
                    result *= 0.7f;
                }
            }
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (WorldDay)) * level;
            if (boss)
                gain *= 2f;
            return Mathf.CeilToInt(gain);
        }


        #endregion
        #region Turret
        public static int TurretDamage { get => Mathf.CeilToInt(PlayerData.TurretLevel * 50 * (WorldDay / 3f + 1)); }
        #endregion
        #region Commander
        public static int CommanderWeaponDamage { get => Mathf.CeilToInt(50000000000 * (WorldDay / 3f + 1)); }
        public static int GrenadeDamage { get => Mathf.CeilToInt(50 * (WorldDay / 3f + 1)); }
        public static int MolotovDPS { get => Mathf.CeilToInt(50 * (WorldDay / 3f + 1)); }
        #endregion
        #region Airstrike
        public static int AirstrikeDamage { get => Mathf.CeilToInt(PlayerData.AirstrikeLevel * 5000000000 * (WorldDay / 3f + 1)); }
        #endregion
    }

}
