using FateGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    public static int WaveLevel { get => PlayerProgression.PlayerData.WaveLevel; set => PlayerProgression.PlayerData.WaveLevel = value; }
    public enum TimePeriod { Morning, Noon, Evening, Night }
    public static TimePeriod CurrentTimePeriod
    {
        get => (TimePeriod)((WaveLevel - 1) % 4);
    }

    public static bool NewZone { get => NewDay && NormalizedDay == 1; }
    public static bool NewDay { get => CurrentTimePeriod == TimePeriod.Morning; }
    public static bool NewWorld { get => NewDay && WorldDay == 1; }

    public static int GetCycleNumber(int day) => day - 1 / 25 + 1;
    public static int CycleNumber { get => GetCycleNumber(Day); }
    public static int GetDay(int waveLevel) => (waveLevel - 1) / 4 + 1;

    public static int Day { get => GetDay(WaveLevel); }

    public static int GetNormalizedDay(int cycleDay)
    {
        int sum = 0;
        int i = 0;
        while (i < Settings.ZoneLengths.Length)
        {
            int j = 0;
            while (j < Settings.ZoneLengths[i].Length)
            {
                sum += Settings.ZoneLengths[i][j];
                if (cycleDay <= sum)
                    return (cycleDay) - (sum - Settings.ZoneLengths[i][j]);
                j++;
            }
            i++;
        }
        return -1;
    }
    public static int NormalizedDay { get => GetNormalizedDay(CycleDay); }
    public static int GetCycleDay(int day) => (day - 1) % Settings.NumberOfDays + 1;
    public static int CycleDay { get => GetCycleDay(Day); }

    public static int GetZoneLevel(int cycleDay)
    {
        for (int i = 0; i < Settings.CumulativeZoneLengths.Length; i++)
            for (int j = 0; j < Settings.CumulativeZoneLengths[i].Length; j++)
                if (cycleDay <= Settings.CumulativeZoneLengths[i][j])
                    return j + 1;

        return -1;
    }
    public static int ZoneLevel { get => GetZoneLevel(CycleDay); }
    public static int GetWorldLevel(int cycleDay)
    {
        for (int i = 0; i < Settings.CumulativeZoneLengths.Length; i++)
            for (int j = 0; j < Settings.CumulativeZoneLengths[i].Length; j++)
                if (cycleDay <= Settings.CumulativeZoneLengths[i][j])
                    return i + 1;

        return -1;

    }

    public static int WorldLevel { get => GetWorldLevel(CycleDay); }
    public static int GetWorldDay(int cycleDay)
    {
        int worldLevel = GetWorldLevel(cycleDay);
        if (worldLevel == 1)
            return cycleDay;
        int result = cycleDay - Settings.CumulativeZoneLengths[worldLevel - 2][^1];
        return result;
    }
    public static int WorldDay { get => GetWorldDay(CycleDay); }

}
