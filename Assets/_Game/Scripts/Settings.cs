using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FateGames.PlayerProgression;
using static WaveController;

public static class Settings
{
    public static readonly int FrostUnlockDay = 8;
    public static readonly int TurretUnlockDay = 15;
    public static readonly int TNTUnlockDay = 4;

    private static int NumberOfFrostBombs { get => PlayerData.Traps.Where(trapData => trapData.Item1 == 1).Count(); }
    private static int NumberOfTurrets { get => PlayerData.Turrets.Count; }
    private static int NumberOfExplosiveBombs { get => PlayerData.Traps.Where(trapData => trapData.Item1 == 0).Count(); }

    public static class Zone1
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 2.47f * Mathf.Pow(2.5f, PlayerData.SoldierMergeLevel - 1);
        private static readonly float BaseFireRateCost = 10.13f;
        private static readonly float BaseMergeRateCost = 15.14f;
        public static int MaxFireRateLevel = 14;
        public static int MergeCost { get => Mathf.CeilToInt(BaseMergeRateCost * (Barracks.Instance.TotalMerge + 1)); }
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * ((Barracks.Instance.Power) * 2f)); }
        public static int FireRateCost { get => Mathf.CeilToInt(BaseFireRateCost * (PlayerData.FireRateLevel) * 4); }
        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (NormalizedDay + PlayerData.BaseDefenseLevel);
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
        #region Preparation
        public static int FrostBombCost { get => Mathf.CeilToInt(NumberOfFrostBombs * 100 + 5); }
        public static int TurretCost { get => Mathf.CeilToInt(NumberOfTurrets * 800 + 200); }
        public static int ExplosiveBombCost { get => Mathf.CeilToInt(NumberOfExplosiveBombs * 100 + 5); }
        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (NormalizedDay - 2) bitirdiðimiz günü temsil eder
                float money = (NormalizedDay - 2) * 101 + 51;
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
                float money = NormalizedDay * 20;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Upgrades
        public static readonly int TrapCapacityLimit = 2;
        public static readonly int TurretCapacityLimit = 0;
        public static readonly int BaseDefenseLimit = 7;
        public static readonly int SoldierMergeLevelLimit = 2;
        public static int BaseDefenseLevelCost { get => Mathf.CeilToInt(PlayerData.BaseDefenseLevel * 100); }
        public static int TrapCapacityCost { get => Mathf.CeilToInt((PlayerData.TrapCapacity + 1) * 100); }
        public static int TurretCapacityCost { get => Mathf.CeilToInt((PlayerData.TurretCapacity + 1) * 250); }
        public static int SoldierMergeLevelCost { get => Mathf.CeilToInt(PlayerData.SoldierMergeLevel * 1500); }

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
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 1.8f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 2.5f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 4f;
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
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 0.73f) / 7f, 0, 1f) * 4); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 40;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 8;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + normalizedDayMultiplier);
            if (boss && NormalizedDay == 7)
                result *= bossMultiplier * 0.5f;
            if (NormalizedDay >= 6)
                result *= 1.2f;
            if (NormalizedDay >= 5)
                result *= 1.4f;
            if (NormalizedDay > 2)
                result *= 1.2f;
            if (NormalizedDay > 1)
                result *= 0.9f;
            if (NormalizedDay == 1 && CurrentTimePeriod == TimePeriod.Morning)
                result *= 0.7f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + normalizedDayMultiplier);
            if (boss)
                result *= bossMultiplier * 0.8f;
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (NormalizedDay) * 2) * level;
            if (boss)
                gain *= 2f;
            return Mathf.CeilToInt(gain);
        }


        #endregion
    }
    public static class Zone2
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
                float result = BaseBarrierHealth * (NormalizedDay + PlayerData.BaseDefenseLevel);
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
        #region Preparation
        public static int FrostBombCost { get => Mathf.CeilToInt(NumberOfFrostBombs * 100 + 5); }
        public static int TurretCost { get => Mathf.CeilToInt(NumberOfTurrets * 800 + 200); }
        public static int ExplosiveBombCost { get => Mathf.CeilToInt(NumberOfExplosiveBombs * 100 + 5); }
        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (NormalizedDay - 2) bitirdiðimiz günü temsil eder
                float money = (NormalizedDay - 2) * 101 + 51;
                money *= 1.5f;
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
                float money = NormalizedDay * 20;
                money *= 1.5f;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Upgrades
        public static readonly int TrapCapacityLimit = 4;
        public static readonly int TurretCapacityLimit = 2;
        public static readonly int BaseDefenseLimit = 14;
        public static readonly int SoldierMergeLevelLimit = 3;
        public static int BaseDefenseLevelCost { get => Mathf.CeilToInt(Mathf.Pow(PlayerData.BaseDefenseLevel, 1.5f) * 100); }
        public static int TrapCapacityCost { get => Mathf.CeilToInt(Mathf.Pow(PlayerData.TrapCapacity + 1, 1.9f) * 100); }
        public static int TurretCapacityCost { get => Mathf.CeilToInt(Mathf.Pow(PlayerData.TurretCapacity + 1, 2.2f) * 250); }
        public static int SoldierMergeLevelCost { get => Mathf.CeilToInt(Mathf.Pow(PlayerData.SoldierMergeLevel, 2.1f) * 1500); }

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
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 1.8f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 2.5f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 4f;
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
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 4) / 14f, 0, 1f) * 5); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 40;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 8;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + normalizedDayMultiplier);
            result *= 1.1f;
            if (boss)
                result *= bossMultiplier * 1.5f;
            if (NormalizedDay > 2 && NormalizedDay < 8)
            {
                result *= 1.0f + (NormalizedDay - 2) / 12f * 2f;
            }
            else if (NormalizedDay >= 8)
            {
                result *= 1.0f + (NormalizedDay - 2) / 12f * 2f;
                result *= 1.3f;
            }
            if (NormalizedDay > 1)
                result *= 1.2f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + normalizedDayMultiplier);
            result *= 0.5f;
            if (boss)
            {
                result *= bossMultiplier * 0.8f;
                result *= 2;
                if (NormalizedDay == 14)
                {
                    result *= 0.7f;
                }
            }
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (NormalizedDay)) * level;
            if (boss)
                gain *= 2f;
            return Mathf.CeilToInt(gain);
        }


        #endregion
    }
    public static class Zone3
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 2.47f * Mathf.Pow(2.1f, Mathf.Clamp(PlayerData.SoldierMergeLevel - 1, 0, 2));
        private static readonly float BaseFireRateCost = 10.13f;
        private static readonly float BaseMergeRateCost = 15.14f;
        public static int MaxFireRateLevel = 40;
        public static int MergeCost { get => Mathf.CeilToInt(PlayerData.SoldierMergeLevel * 1.5f * BaseMergeRateCost * (Barracks.Instance.TotalMerge + 1)); }
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * ((Barracks.Instance.Power) * 2f)); }
        public static int FireRateCost { get => Mathf.CeilToInt(1.2f * BaseFireRateCost * (PlayerData.FireRateLevel) * 4); }
        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (NormalizedDay + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                if (NormalizedDay == 1)
                    result *= 2;
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
        public static int IncomeCost { get => Mathf.CeilToInt(1.5f * BaseIncomeCost * PlayerData.IncomeLevel); }

        #endregion
        #region Preparation
        public static int FrostBombCost { get => Mathf.CeilToInt(2 * NumberOfFrostBombs * 100 + 5); }
        public static int TurretCost { get => Mathf.CeilToInt(2 * NumberOfTurrets * 800 + 200); }
        public static int ExplosiveBombCost { get => Mathf.CeilToInt(2 * NumberOfExplosiveBombs * 100 + 5); }
        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (NormalizedDay - 2) bitirdiðimiz günü temsil eder
                float money = (NormalizedDay - 2) * 101 + 51;
                money *= 2;
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
                float money = NormalizedDay * 20;
                money *= 2;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Upgrades
        public static readonly int BaseDefenseLimit = 30;
        public static readonly int TrapCapacityLimit = 5;
        public static readonly int TurretCapacityLimit = 4;
        public static readonly int SoldierMergeLevelLimit = 4;
        public static int BaseDefenseLevelCost { get => Mathf.CeilToInt(3 * PlayerData.BaseDefenseLevel * 100); }
        public static int TrapCapacityCost { get => Mathf.CeilToInt(2 * (PlayerData.TrapCapacity + 1) * 100); }
        public static int TurretCapacityCost { get => Mathf.CeilToInt(1.5f * (PlayerData.TurretCapacity + 1) * 250); }
        public static int SoldierMergeLevelCost { get => Mathf.CeilToInt(1.5f * PlayerData.SoldierMergeLevel * 1500); }

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
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 1.8f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 2.5f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 4f;
                        break;
                    default:
                        result = BaseWavePower;
                        break;
                }
                result *= 1.3f;
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
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 6) / 20f, 0, 1f) * 6); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 40;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 8;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + normalizedDayMultiplier);
            if (boss)
                result *= bossMultiplier * 1.5f * 1.3f;
            if (NormalizedDay > 2 && NormalizedDay < 11)
            {
                result *= 1.4f + (NormalizedDay - 2) / 18f * 3f;
            }
            else if (NormalizedDay >= 11)
            {
                result *= 1.4f + (NormalizedDay - 2) / 18f * 3f;
                result *= 1.3f;
            }
            if (NormalizedDay > 1)
                result *= 1.35f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + normalizedDayMultiplier);
            result *= 0.9f;
            if (boss)
            {
                result *= bossMultiplier * 0.8f;
                result *= 2;
                if (NormalizedDay == 20)
                {
                    result *= 0.7f;
                }
            }
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (NormalizedDay) * 2) * level;
            gain *= 1.7f;
            if (boss)
                gain *= 2f;
            return Mathf.CeilToInt(gain);
        }


        #endregion
    }
    public static class Zone4
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 2.47f * Mathf.Pow(2.1f, Mathf.Clamp(PlayerData.SoldierMergeLevel - 1, 0, 2));
        private static readonly float BaseFireRateCost = 10.13f;
        private static readonly float BaseMergeRateCost = 15.14f;
        public static int MaxFireRateLevel = 40;
        public static int MergeCost { get => Mathf.CeilToInt(PlayerData.SoldierMergeLevel * 1.5f * BaseMergeRateCost * (Barracks.Instance.TotalMerge + 1)); }
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * ((Barracks.Instance.Power) * 2f)); }
        public static int FireRateCost { get => Mathf.CeilToInt(1.2f * BaseFireRateCost * (PlayerData.FireRateLevel) * 4); }
        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (NormalizedDay + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 3;
                if (NormalizedDay == 1)
                    result *= 2;
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
        public static int IncomeCost { get => Mathf.CeilToInt(1.5f * BaseIncomeCost * PlayerData.IncomeLevel); }

        #endregion
        #region Preparation
        public static int FrostBombCost { get => Mathf.CeilToInt(2 * NumberOfFrostBombs * 100 + 5); }
        public static int TurretCost { get => Mathf.CeilToInt(2 * NumberOfTurrets * 800 + 200); }
        public static int ExplosiveBombCost { get => Mathf.CeilToInt(2 * NumberOfExplosiveBombs * 100 + 5); }
        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (NormalizedDay - 2) bitirdiðimiz günü temsil eder
                float money = (NormalizedDay - 2) * 101 + 51;
                money *= 2;
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
                float money = NormalizedDay * 20;
                money *= 2;
                return Mathf.CeilToInt(money);
            }
        }
        #endregion
        #region Upgrades
        public static readonly int BaseDefenseLimit = 90;
        public static readonly int TrapCapacityLimit = 6;
        public static readonly int TurretCapacityLimit = 6;
        public static readonly int SoldierMergeLevelLimit = 4;
        public static int BaseDefenseLevelCost { get => Mathf.CeilToInt(3 * PlayerData.BaseDefenseLevel * 100); }
        public static int TrapCapacityCost { get => Mathf.CeilToInt(2 * (PlayerData.TrapCapacity + 1) * 100); }
        public static int TurretCapacityCost { get => Mathf.CeilToInt(1.5f * (PlayerData.TurretCapacity + 1) * 250); }
        public static int SoldierMergeLevelCost { get => Mathf.CeilToInt(1.5f * PlayerData.SoldierMergeLevel * 1500); }

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
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1);
                        break;
                    case TimePeriod.Noon:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 1.8f;
                        break;
                    case TimePeriod.Evening:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 2.5f;
                        break;
                    case TimePeriod.Night:
                        result = BaseWavePower * ((NormalizedDay - 1) * multiplier + 1) * 4f;
                        break;
                    default:
                        result = BaseWavePower;
                        break;
                }
                result *= 1.3f;
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
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 9) / 30f, 0, 1f) * 6); }

        #endregion
        #region Zombie
        private static readonly int BaseZombieHealth = 40;
        private static readonly int BaseZombieDamage = 30;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = 8;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieHealth * (level + normalizedDayMultiplier);
            if (boss)
                result *= bossMultiplier * 1.5f * 1.3f;
            if (NormalizedDay > 2 && NormalizedDay < 11)
            {
                result *= 1.4f + (NormalizedDay - 2) / 28f * 3f;
            }
            else if (NormalizedDay >= 11)
            {
                result *= 1.4f + (NormalizedDay - 2) / 28f * 3f;
                result *= 1.3f;
            }
            if (NormalizedDay > 1)
                result *= 1.35f;
            return Mathf.CeilToInt(result);
        }
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 4;
            float normalizedDayMultiplier = (NormalizedDay - 1) * 0.8f;
            if (NormalizedDay >= 3)
                normalizedDayMultiplier *= 0.5f;
            float result = BaseZombieDamage * (level + normalizedDayMultiplier);
            result *= 0.9f;
            if (boss)
            {
                result *= bossMultiplier * 0.8f;
                result *= 2;
                if (NormalizedDay == 30)
                {
                    result *= 0.7f;
                }
            }
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (NormalizedDay) * 2) * level;
            gain *= 1.7f;
            if (boss)
                gain *= 2f;
            return Mathf.CeilToInt(gain);
        }


        #endregion
    }

}
