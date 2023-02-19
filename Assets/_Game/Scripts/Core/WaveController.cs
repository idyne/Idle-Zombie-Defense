using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;
using static LevelManager;
using UnityEngine.AI;

public class WaveController : MonoBehaviour
{

    [SerializeField] private UIDayBar uiDayBar;
    [SerializeField] private UIAnimationSequencer UIAnimationSequencer;
    private static WaveController instance = null;
    private SoundFXWorker crowdSoundWorker = null;
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

    public Wave CurrentWave { get; private set; } = null;
    public static WaveState State { get; private set; }

    private int CurrentMaxZombieLevel { get => GetCurrentMaxZombieLevel(); }
    public UnityEvent<Wave> OnNewWave { get; private set; } = new();
    public UnityEvent OnWaveStart { get; private set; } = new();
    public UnityEvent OnWaveEnd { get; private set; } = new();
    private int CurrentBossLevel { get => CurrentTimePeriod == TimePeriod.Night ? NormalizedDay : -1; }


    private int GetCurrentLevelPower()
    {
        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.WavePower;
            case 2:
                return Settings.World2.WavePower;
        }
        return 1;
    }

    private int GetCurrentMaxZombieLevel()
    {

        switch (WorldLevel)
        {
            case 1:
                return Settings.World1.MaxZombieLevel;
            case 2:
                return Settings.World2.MaxZombieLevel;
        }
        return 1;
    }

    private void Start()
    {
        TowerController.Instance.GetCurrentTower();
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        State = WaveState.WAITING;
        CreateWave(GetCurrentLevelPower(), CurrentMaxZombieLevel, CurrentBossLevel);
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        StartCoroutine(UIAnimationSequencer.GoCurrentTimePeriod());
    }

    private void GoNextLevel()
    {
        crowdSoundWorker.Stop();
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
            uiDayBar.SetPercent(percent);

        });
        MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Start, WaveLevel);
        CurrentWave.Start();
        crowdSoundWorker = SoundFX.PlaySound("Zombie Crowd Sound");
        State = WaveState.RUNNING;
        UIButtonManager.Instance.UpdateStartButton();
        UILevelBar.Instance.Hide();
        uiDayBar.GoUp();
        OnWaveStart.Invoke();
        Turret.Stopped = false;
        HapticManager.DoHaptic();
    }


    public void StopWave()
    {
        CurrentWave.Stop();
        crowdSoundWorker.Stop();
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
                    DOVirtual.DelayedCall(++count * period, () => { SpawnZombie(level); }, false);
                }
            }
            if (bossLevel > 0)
                DOVirtual.DelayedCall(++count * period, () => { SpawnBoss(bossLevel); }, false);
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
            Tower tower = TowerController.Instance.GetCurrentTower();
            Vector3 position;
            if (tower.OverrideZombieSpawnPoints)
                position = tower.GetRandomZombieSpawnPoint().position;
            else
                position = GenerateSpawnPosition(30, 5);
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
            Tower tower = TowerController.Instance.GetCurrentTower();
            Vector3 position;
            if (tower.OverrideZombieSpawnPoints)
                position = tower.GetRandomZombieSpawnPoint().position;
            else
                position = GenerateSpawnPosition(30, 5);
            string bossTag = "Boss";
            bool bigBoss = false;
            for (int i = 0; i < Settings.CumulativeZoneLengths.Length; i++)
            {
                for (int j = 0; j < Settings.CumulativeZoneLengths[i].Length; j++)
                {
                    if (CycleDay == Settings.CumulativeZoneLengths[i][j])
                    {
                        bigBoss = true;
                        break;
                    }
                }
            }
            if (bigBoss)
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
            int count = 0;
            while (count++ < 100 && !NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                position = Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.right * circleRadius;
                position += position.normalized * Random.Range(0, circleWidth);
            }
            if (count == 100) print("error");
            return position;
        }

        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
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
