using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static FateGames.PlayerProgression;
using static LevelManager;

public static class Settings
{
    public static int BaseDefenseLevelCost { get => Mathf.CeilToInt((PlayerData.BaseDefenseLevel - 1) * 1 + 5); }
    public static int SoldierMergeLevelCost { get => Mathf.CeilToInt((PlayerData.SoldierMergeLevel - 1) * 5 + 15); }
    public static int CommanderLevelPrice { get => Mathf.CeilToInt((PlayerData.ThrowableWeaponsGuyLevel) * 10 + 10); }
    public static int AirstrikeLevelPrice { get => Mathf.CeilToInt((PlayerData.AirstrikeLevel - 1) * 10 + 25); }

    public static int TNTLevelCost { get => Mathf.CeilToInt((PlayerData.TNTLevel - 1) * 2 + 3); }
    public static int BarbwireLevelCost { get => Mathf.CeilToInt((PlayerData.BarbwireLevel - 1) * 2 + 6); }
    public static int FrostLevelCost { get => Mathf.CeilToInt((PlayerData.FrostLevel - 1) * 2 + 8); }
    public static int TurretLevelCost { get => Mathf.CeilToInt((PlayerData.TurretLevel - 1) * 2 + 10); }

    public static int ExplosiveBombCost { get => Mathf.CeilToInt((NumberOfExplosiveBombs * 100 + 50)); }
    public static int BarbwireCost { get => Mathf.CeilToInt((NumberOfBarbwires * 120 + 80)); }
    public static int FrostBombCost { get => Mathf.CeilToInt((NumberOfFrostBombs * 150 + 100)); }
    public static int TurretCost { get => Mathf.CeilToInt((NumberOfTurrets * 1000 + 500)); }

    public static int BarbwireDPS { get => Mathf.CeilToInt(PlayerData.BarbwireLevel * 10 * (WorldDay / 3 + 1)); }
    public static int TNTDamage { get => Mathf.CeilToInt(PlayerData.TNTLevel * 15 * (WorldDay / 3 + 1)); }
    public static int FrostDuration { get => Mathf.CeilToInt(PlayerData.FrostLevel * 10); }
    public static int BarbwireMaxDamage { get => BarbwireDPS * 3; }
    public static float ThrowableWeaponCooldown { get => 12f / (PlayerData.ThrowableWeaponsGuyLevel / 1.6f); }
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
        private static readonly float BaseSoldierCost = 10f;
        private static readonly float SoldierCostExponentialIncreaseRatio = 0.02f;
        private static readonly float SoldierCostLinearIncreaseRatio = CycleNumber > 1 ? 16f/81f : 16f;

        private static readonly float BaseMergeCost = 30;
        private static readonly float MergeCostExponentialIncreaseRatio = 0.1f;
        private static readonly float MergeCostLinearIncreaseRatio = 31f;

        private static readonly float BaseFireRateCost = 40f;
        private static readonly float FireRateCostExponentialIncreaseRatio = 0.3f;
        private static readonly float FireRateCostLinearIncreaseRatio = 30f;

        public static int MaxFireRateLevel = 20;
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * Mathf.Pow(1 + SoldierCostExponentialIncreaseRatio, CycleNumber > 1 ? Barracks.Instance.Power/81f : Barracks.Instance.Power ) + Barracks.Instance.Power * SoldierCostLinearIncreaseRatio); }
        public static int MergeCost { get => Mathf.CeilToInt(BaseMergeCost * Mathf.Pow(1 + MergeCostExponentialIncreaseRatio, PlayerData.MergeCount) + PlayerData.MergeCount * MergeCostLinearIncreaseRatio); }
        public static int FireRateCost { get => Mathf.CeilToInt(BaseFireRateCost * Mathf.Pow(1 + FireRateCostExponentialIncreaseRatio, PlayerData.FireRateLevel - 1) + (PlayerData.FireRateLevel - 1) * FireRateCostLinearIncreaseRatio); }

        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (WorldDay - 1 + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 2.5f;
                return Mathf.CeilToInt(result);
            }
        }
        public static int TowerMaxHealth
        {
            get
            {
                float result = BaseTowerHealth * (WorldDay - 1 + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 2;
                return Mathf.CeilToInt(result);
            }
        }
        #endregion
        #region Financier
        public static readonly float BaseIncomeCost = 50;
        private static float IncomeCostExponentialIncreaseRatio = 0.2f;
        private static float IncomeCostLinearIncreaseRatio = 51f;
        public static int IncomeCost { get => Mathf.CeilToInt(BaseIncomeCost * Mathf.Pow(1 + IncomeCostExponentialIncreaseRatio, PlayerData.IncomeLevel - 1) + (PlayerData.IncomeLevel - 1) * IncomeCostLinearIncreaseRatio); }

        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay == 1) ? 0 : (WorldDay - 2) * 101 + 51;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (Day == 4 || Day == 8)
                    money *= 2;
                return Mathf.CeilToInt(money * 1);
            }
        }
        public static int FinishDayUpgradePrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay == 1) ? 10 : 5;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (Day == 4 || Day == 8)
                    money = 10;
                return Mathf.CeilToInt(money * (1));
            }
        }
        public static int FinishPhasePrize
        {
            get
            {
                float money = WorldDay * 10;
                return Mathf.CeilToInt(money * (1));
            }
        }
        public static int FinishPhaseUpgradePrize
        {
            get
            {
                float money = 3;
                return Mathf.CeilToInt(money * (1));
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
        public static int MaxZombieLevel { get => Mathf.CeilToInt(Mathf.Clamp(WorldDay / 10f , 0, 1f) * 6); }

        #endregion
        #region Zombie

        private static float BaseZombieHealth
        {
            get
            {
                float result = CycleNumber > 1 ? 1000 : 30f;
                return result;
            }
        }
        private static readonly float ZombieHealthExponentialIncreaseRatio = 0.05f;
        private static readonly float ZombieHealthLinearIncreaseRatio = CycleNumber > 1 ? 300f : 20f;
        public static int ZombieHealth(int level, bool boss)
        {
            int bossMultiplier = CycleNumber > 1 ? 10 : 5;
            
            float result = level * (BaseZombieHealth * Mathf.Pow(1 + ZombieHealthExponentialIncreaseRatio, WorldDay - 1) + (WorldDay - 1) * ZombieHealthLinearIncreaseRatio);

            /*if (WorldDay >= 10)
                result *= 1.2f;*/
            if (CycleNumber >= 1 && ( WorldDay == CumulativeZoneLengths[0][1] || WorldDay == CumulativeZoneLengths[0][2]))
                result *= 0.5f;
            if (CycleNumber == 1 && WorldDay == CumulativeZoneLengths[0][0])
                result *= 0.5f;
            if (CycleNumber == 1 && WorldDay == 2)
                result *= 0.6f;
            if (boss && (WorldDay == CumulativeZoneLengths[0][0] || WorldDay == CumulativeZoneLengths[0][1] || WorldDay == CumulativeZoneLengths[0][2]))
                result *= bossMultiplier * 1.3f;
            else if (boss)
                result *= bossMultiplier;


            /*if (WorldDay >= 5)
                result *= 1.4f;
            if (WorldDay > 2)
                result *= 1.2f;
            if (WorldDay > 1)
                result *= 0.9f;
            if (WorldDay == 1 && CurrentTimePeriod == TimePeriod.Morning)
                result *= 0.7f;*/
            return Mathf.CeilToInt(result);
        }


        private static readonly int BaseZombieDamage = CycleNumber > 1 ? 200 : 30;
        private static readonly float ZombieDamagaLinearIncreaseRatio = 0.5f;
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 2;
            float WorldDayMultiplier = (WorldDay - 1) * ZombieDamagaLinearIncreaseRatio;
            /*if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;*/
            float result = level * (BaseZombieDamage * (1 + WorldDayMultiplier));
            if (boss)
                result *= bossMultiplier * 0.5f;
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float baseZombieIncome = 8f;
            /*float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (WorldDay) * 2) * level;*/
            float gain = (baseZombieIncome * (Mathf.Pow(1.05f, PlayerData.IncomeLevel - 1) + WorldDay * 0.1f)) * level;
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

        public static float BaseCommanderWeaponDamage { get { return CycleNumber > 1 ? 350 : 15; } }
        private static readonly float CommanderWeaponDamageExponentialIncreaseRatio = 0.05f;
        private static readonly float CommanderWeaponDamageLinearIncreaseRatio = CycleNumber > 1 ? 100 : 15f;
        public static int CommanderWeaponDamage { get => Mathf.CeilToInt(BaseCommanderWeaponDamage * Mathf.Pow(1 + CommanderWeaponDamageExponentialIncreaseRatio, WorldDay - 1) + (WorldDay - 1) * CommanderWeaponDamageLinearIncreaseRatio); }
        public static int GrenadeDamage { get => Mathf.CeilToInt(35 * (WorldDay / 3f + 1)); }
        public static int MolotovDPS { get => Mathf.CeilToInt(CycleNumber > 1 ? 100 : 50 * (WorldDay / 3f + 1)); }
        #endregion
        #region Airstrike
        public static int AirstrikeDamage { get => Mathf.CeilToInt(PlayerData.AirstrikeLevel * 20 * (WorldDay / 3f + 1)); }
        #endregion
    }


    public static class World2
    {
        #region Barracks
        private static readonly float BaseSoldierCost = 10f;
        private static readonly float SoldierCostExponentialIncreaseRatio = 0.0025f;
        private static readonly float SoldierCostLinearIncreaseRatio = CycleNumber > 1 ? 16 / 7f / 81f: 16 / 7f;

        private static readonly float BaseMergeCost = 30;
        private static readonly float MergeCostExponentialIncreaseRatio = 0.1f;
        private static readonly float MergeCostLinearIncreaseRatio = 31f;

        private static readonly float BaseFireRateCost = 40f;
        private static readonly float FireRateCostExponentialIncreaseRatio = 0.3f;
        private static readonly float FireRateCostLinearIncreaseRatio = 30f;

        public static int MaxFireRateLevel = 20;
        public static int SoldierCost { get => Mathf.CeilToInt(BaseSoldierCost * Mathf.Pow(1 + SoldierCostExponentialIncreaseRatio, CycleNumber > 1 ? Barracks.Instance.Power / 9f : Barracks.Instance.Power) + Barracks.Instance.Power * SoldierCostLinearIncreaseRatio); }
        public static int MergeCost { get => Mathf.CeilToInt(BaseMergeCost * Mathf.Pow(1 + MergeCostExponentialIncreaseRatio, PlayerData.MergeCount) + PlayerData.MergeCount * MergeCostLinearIncreaseRatio); }
        public static int FireRateCost { get => Mathf.CeilToInt(BaseFireRateCost * Mathf.Pow(1 + FireRateCostExponentialIncreaseRatio, PlayerData.FireRateLevel - 1) + (PlayerData.FireRateLevel - 1) * FireRateCostLinearIncreaseRatio); }

        #endregion
        #region Base
        public static readonly float BaseBarrierHealth = 300;
        public static readonly float BaseTowerHealth = 1000;
        public static int BarrierMaxHealth
        {
            get
            {
                float result = BaseBarrierHealth * (WorldDay - 1 + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 2.5f;
                return Mathf.CeilToInt(result);
            }
        }
        public static int TowerMaxHealth
        {
            get
            {
                float result = BaseTowerHealth * (WorldDay - 1 + PlayerData.BaseDefenseLevel);
                if (CurrentTimePeriod == TimePeriod.Night)
                    result *= 2;
                return Mathf.CeilToInt(result);
            }
        }
        #endregion
        #region Financier
        public static readonly float BaseIncomeCost = 50;
        private static float IncomeCostExponentialIncreaseRatio = 0.2f;
        private static float IncomeCostLinearIncreaseRatio = 51f;
        public static int IncomeCost { get => Mathf.CeilToInt(BaseIncomeCost * Mathf.Pow(1 + IncomeCostExponentialIncreaseRatio, PlayerData.IncomeLevel - 1) + (PlayerData.IncomeLevel - 1) * IncomeCostLinearIncreaseRatio); }

        #endregion
        #region Prize
        public static int FinishDayPrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay == 1) ? 0 : (WorldDay - 2) * 101 + 51;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (WorldDay == 5 || WorldDay == 9)
                    money *= 2;
                return Mathf.CeilToInt(money);
            }
        }
        public static int FinishDayUpgradePrize
        {
            get
            {
                // (WorldDay - 2) bitirdiðimiz günü temsil eder
                float money = (WorldDay == 1) ? 10 : 5;
                // Zonelarýn son günlerindeki para önemsiz olduðu için sabit bir para veriyoruz
                if (WorldDay == 5 || WorldDay == 9)
                    money = 10;
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
                float money = 3;
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

        
        private static readonly float BaseZombieHealth = CycleNumber > 1 ? 1000 : 200f;
        private static readonly float ZombieHealthExponentialIncreaseRatio = CycleNumber > 1 ? 0.05f : 0.08f;
        private static readonly float ZombieHealthLinearIncreaseRatio = CycleNumber > 1 ? 300f : 100f;
        public static int ZombieHealth(int level, bool boss)
        {
            
            int bossMultiplier = 10;
            /*if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;*/
            float result = level * (BaseZombieHealth * Mathf.Pow(1 + ZombieHealthExponentialIncreaseRatio, WorldDay - 1) + (WorldDay - 1) * ZombieHealthLinearIncreaseRatio);
            if (CycleNumber == 1 && WorldDay >= 9)
            {
                result /= 2f;
            }
            /*if (WorldDay >= 10)
                result *= 1.2f;*/
            if (CycleNumber >= 1 && (WorldDay == CumulativeZoneLengths[1][1] || WorldDay == CumulativeZoneLengths[1][2]))
                result *= 0.5f;
            if (CycleNumber >= 1 && WorldDay == 8)
                result /= 0.5f;
            if (boss && (WorldDay == CumulativeZoneLengths[0][0] || WorldDay == CumulativeZoneLengths[0][1] || WorldDay == CumulativeZoneLengths[0][2]))
                result *= bossMultiplier * 1.5f;
            else if (boss)
                result *= bossMultiplier;


            /*if (WorldDay >= 5)
                result *= 1.4f;
            if (WorldDay > 2)
                result *= 1.2f;
            if (WorldDay > 1)
                result *= 0.9f;
            if (WorldDay == 1 && CurrentTimePeriod == TimePeriod.Morning)
                result *= 0.7f;*/
            return Mathf.CeilToInt(result);
        }


        private static readonly int BaseZombieDamage = CycleNumber > 1 ? 200 : 150;
        private static readonly float ZombieDamagaLinearIncreaseRatio = 0.1f;
        public static int ZombieDamage(int level, bool boss)
        {

            int bossMultiplier = 2;
            float WorldDayMultiplier = (WorldDay - 1) * ZombieDamagaLinearIncreaseRatio;
            /*if (WorldDay >= 3)
                WorldDayMultiplier *= 0.5f;*/
            float result = level * (BaseZombieDamage * (1 + WorldDayMultiplier));
            if (boss)
                result *= bossMultiplier * 0.6f;
            return Mathf.CeilToInt(result);
        }

        public static int ZombieGain(int level, bool boss)
        {
            float baseZombieIncome = 8f;
            /*float gain = (((PlayerData.IncomeLevel - 1) / 3f) + (WorldDay) * 2) * level;*/
            float gain = (baseZombieIncome * (Mathf.Pow(1.05f, PlayerData.IncomeLevel - 1) + WorldDay * 0.1f)) * level;
            if (boss)
            {
                Debug.Log(level);
                gain *= 2f;
            }

            return Mathf.CeilToInt(gain);
        }


        #endregion
        #region Turret
        public static int TurretDamage { get => Mathf.CeilToInt(PlayerData.TurretLevel * 20 * (WorldDay / 3f + 1)); }
        #endregion
        #region Commander

        public static readonly float BaseCommanderWeaponDamage = CycleNumber > 1 ? 350 : 80;
        private static readonly float CommanderWeaponDamageExponentialIncreaseRatio = CycleNumber > 1 ? 0.05f : 0.06f;
        private static readonly float CommanderWeaponDamageLinearIncreaseRatio = CycleNumber > 1 ? 100 : 35f;
        public static int CommanderWeaponDamage { get => Mathf.CeilToInt(BaseCommanderWeaponDamage * Mathf.Pow(1 + CommanderWeaponDamageExponentialIncreaseRatio, WorldDay - 1) + (WorldDay - 1) * CommanderWeaponDamageLinearIncreaseRatio); }
        public static int GrenadeDamage { get => Mathf.CeilToInt(35 * (WorldDay / 3f + 1)); }
        public static int MolotovDPS { get => Mathf.CeilToInt(CycleNumber > 1 ? 100 : 70 * (WorldDay / 3f + 1)); }
        #endregion
        #region Airstrike
        public static int AirstrikeDamage { get => Mathf.CeilToInt(PlayerData.AirstrikeLevel * 30 * (WorldDay / 3f + 1)); }
        #endregion
    }

}
