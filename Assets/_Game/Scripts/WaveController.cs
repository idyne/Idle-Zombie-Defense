using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class WaveController : MonoBehaviour
{

    [SerializeField] private ZombieBar zombieBar;
    [SerializeField] private UIAnimationSequencer UIAnimationSequencer;
    private static WaveController instance = null;
    public static WaveController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<WaveController>();
            return instance;
        }
    }
    public static int WaveLevel { get => PlayerProgression.PlayerData.WaveLevel; set => PlayerProgression.PlayerData.WaveLevel = value; }
    public enum TimePeriod { Morning, Noon, Evening, Night }
    public Wave CurrentWave { get; private set; } = null;
    public static WaveState State { get; private set; }
    private readonly int baseLevelPower = 5;

    private int CurrentMaxZombieLevel { get => GetCurrentMaxZombieLevel(); }
    public UnityEvent<Wave> OnNewWave { get; private set; } = new();
    public UnityEvent OnWaveStart { get; private set; } = new();
    public UnityEvent OnWaveEnd { get; private set; } = new();
    private int CurrentBossLevel { get => CurrentTimePeriod == TimePeriod.Night ? NormalizedDay : -1; }

    public static TimePeriod CurrentTimePeriod
    {
        get => (TimePeriod)((WaveLevel - 1) % 4);
    }

    public static int Day { get => (WaveLevel - 1) / 4 + 1; }
    public static int NormalizedDay
    {
        get
        {
            int normalizedDay;
            if (Day <= 7) normalizedDay = Day;
            else if (Day <= 21) normalizedDay = Day - 7;
            else if (Day <= 41) normalizedDay = Day - 21;
            else normalizedDay = Day - 41;
            return normalizedDay;
        }
    }

    public static int ZoneLevel
    {
        get
        {
            if (Day <= 7) return 1;
            if (Day <= 21) return 2;
            if (Day <= 41) return 3;
            return 4;
        }
    }

    private int GetCurrentLevelPower()
    {
        float result = baseLevelPower;
        float multiplier = 1.5f;
        switch (CurrentTimePeriod)
        {
            case TimePeriod.Morning:
                result = baseLevelPower * ((NormalizedDay - 1) * multiplier + 1);
                break;
            case TimePeriod.Noon:
                result = baseLevelPower * ((NormalizedDay - 1) * multiplier + 1) * 1.8f;
                break;
            case TimePeriod.Evening:
                result = baseLevelPower * ((NormalizedDay - 1) * multiplier + 1) * 2.5f;
                break;
            case TimePeriod.Night:
                result = baseLevelPower * ((NormalizedDay - 1) * multiplier + 1) * 4f;
                break;
            default:
                break;
        }
        switch (ZoneLevel)
        {
            case 2:
                result *= 1f;
                break;
            case 3:
                result *= 1.3f;
                break;
            case 4:
                result *= 1.5f;
                break;
            default:
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

    private int GetCurrentMaxZombieLevel()
    {

        int result = 1;
        switch (ZoneLevel)
        {
            case 1:
                result = Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 0.73f) / 7f, 0, 1f) * 4);
                break;
            case 2:
                result = Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 4) / 14f, 0, 1f) * 5);
                break;
            case 3:
                result = Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 6) / 20f, 0, 1f) * 6);
                break;
            case 4:
                result = Mathf.CeilToInt(Mathf.Clamp((NormalizedDay + 9) / 30f, 0, 1f) * 6);
                break;
        }
        return result;
    }

    private void Start()
    {
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        State = WaveState.WAITING;

        CreateWave(GetCurrentLevelPower(), CurrentMaxZombieLevel, CurrentBossLevel);
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        StartCoroutine(UIAnimationSequencer.GoCurrentTimePeriod());
        BarrierController.Instance.RepairAll();
    }

    private void GoNextLevel()
    {
        MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Complete, WaveLevel);
        WaveLevel++;
        OnWaveEnd.Invoke();
        LoadCurrentLevel();
    }


    public void StartCurrentWave()
    {
        CurrentWave.OnPercentChange.AddListener((percent) =>
        {
            switch (CurrentTimePeriod)
            {
                case TimePeriod.Morning:
                    percent /= 4;
                    break;
                case TimePeriod.Noon:
                    percent = 0.25f + percent / 4;
                    break;
                case TimePeriod.Evening:
                    percent = 0.50f + percent / 4;
                    break;
                case TimePeriod.Night:
                    percent = 0.75f + percent / 4;
                    break;
                default:
                    break;
            }
            zombieBar.SetPercent(percent);

        });
        MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Start, WaveLevel);
        CurrentWave.Start();
        State = WaveState.RUNNING;
        ButtonManager.Instance.UpdateStartButton(State);
        LevelBar.Instance.Hide();
        zombieBar.GoUp();
        OnWaveStart.Invoke();
        Turret.Stopped = false;
        HapticManager.DoHaptic();
    }


    public void StopWave()
    {
        CurrentWave.Stop();
    }

    public void CreateWave(int power, int maxZombieLevel, int bossLevel = -1)
    {
        Wave wave = new Wave(power, maxZombieLevel, bossLevel);
        CurrentWave = wave;
        CurrentWave.OnWaveClear.AddListener(GoNextLevel);
        OnNewWave.Invoke(wave);
    }

    public class Wave
    {
        public UnityEvent OnWaveClear { get; private set; } = new();
        public UnityEvent<float> OnPercentChange { get; private set; } = new();
        private List<Zombie> zombies = new();
        private int power;
        private int bossLevel;
        private int maxZombieLevel;
        private int totalNumberOfZombies = 0;
        private int numberOfDeadZombies = 0;

        public Wave(int power, int maxZombieLevel, int bossLevel = -1)
        {
            this.power = power;
            this.maxZombieLevel = maxZombieLevel;
            this.bossLevel = bossLevel;
        }

        public void Start()
        {
            SpawnZombies();

        }

        public void Stop()
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.Stop();
            }
        }

        private void SpawnZombies()
        {
            int totalNumberOfZombies = 0;
            List<int> numbersOfZombies = GenerateNumbersOfZombies(power, maxZombieLevel);
            for (int i = 1; i <= maxZombieLevel; i++)
                totalNumberOfZombies += numbersOfZombies[i];
            float duration = 0.1f * power * (CurrentTimePeriod == TimePeriod.Night ? 0.6f : 1);
            float period = duration / totalNumberOfZombies;
            int count = 0;
            while (count < totalNumberOfZombies)
            {
                int level = Random.Range(1, maxZombieLevel + 1);
                if (numbersOfZombies[level] > 0)
                {
                    numbersOfZombies[level]--;
                    DOVirtual.DelayedCall(++count * period, () => { SpawnZombie(level); });
                }
            }
            if (bossLevel > 0)
                DOVirtual.DelayedCall(++count * period, () => { SpawnBoss(bossLevel); });
            this.totalNumberOfZombies = totalNumberOfZombies;
        }

        private List<int> GenerateNumbersOfZombies(int power, int maxZombieLevel)
        {
            List<int> numbersOfZombies = new List<int> { -1, power / maxZombieLevel };
            for (int i = 2; i < maxZombieLevel + 1; i++)
                numbersOfZombies.Add(numbersOfZombies[1] / i);
            int currentPower = 0;
            for (int i = 1; i < numbersOfZombies.Count; i++)
                currentPower += i * numbersOfZombies[i];
            numbersOfZombies[1] += power - currentPower;
            return numbersOfZombies;
        }

        private void SpawnZombie(int level)
        {
            Vector3 position = GenerateSpawnPosition(30, 5);
            Zombie zombie = ObjectPooler.SpawnFromPool("Zombie " + level, position, Quaternion.LookRotation(-position)).GetComponent<Zombie>();
            zombies.Add(zombie);
            zombie.OnDeath.AddListener(() =>
            {
                zombies.Remove(zombie);
                numberOfDeadZombies++;
                OnPercentChange.Invoke(numberOfDeadZombies / (float)totalNumberOfZombies);
                if (zombies.Count == 0) OnWaveClear.Invoke();
            });
        }
        private void SpawnBoss(int level)
        {
            Vector3 position = GenerateSpawnPosition(30, 5);
            string bossTag = "Boss";
            if (Day == 7 || Day == 21 || Day == 41 || Day == 71)
            {
                level *= 2;
                bossTag = "Big Boss";
            }
            Zombie zombie = ObjectPooler.SpawnFromPool(bossTag, position, Quaternion.LookRotation(-position)).GetComponent<Zombie>();
            zombie.SetLevel(level);
            zombies.Add(zombie);
            zombie.OnDeath.AddListener(() =>
            {
                zombies.Remove(zombie);
                numberOfDeadZombies++;
                OnPercentChange.Invoke(numberOfDeadZombies / (float)totalNumberOfZombies);
                if (zombies.Count == 0) OnWaveClear.Invoke();
            });
        }

        private Vector3 GenerateSpawnPosition(float circleRadius, float circleWidth)
        {
            Vector3 position = Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.right * circleRadius;
            position += position.normalized * Random.Range(0, circleWidth);
            return position;
        }

        public Zombie GetClosest(Vector3 to)
        {
            if (zombies.Count == 0) return null;
            Zombie closest = zombies[0];
            float minDistance = Vector3.Distance(closest.Transform.position, to);
            for (int i = 1; i < zombies.Count; i++)
            {
                Zombie zombie = zombies[i];
                float distance = Vector3.Distance(zombie.Transform.position, to);
                if (distance < minDistance)
                {
                    closest = zombie;
                    minDistance = distance;
                }
            }
            return closest;
        }
    }

    public enum WaveState { WAITING, RUNNING };
}
