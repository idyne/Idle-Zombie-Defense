using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using System.Linq;
public static class ProgressInitializer
{
    public static void Initialize()
    {
        InitializePlaceables();
        InitializeSoldiers();
    }

    private static void InitializePlaceables()
    {
        List<Grid> grids = new();
        PlacementController[] placementControllers = Object.FindObjectsOfType<PlacementController>();
        for (int i = 0; i < placementControllers.Length; i++)
        {
            grids.AddRange(placementControllers[i].Grids);
        }

        for (int i = 0; i < PlayerProgression.PlayerData.Traps.Count; i++)
        {
            (int, int, bool) data = PlayerProgression.PlayerData.Traps[i];
            string trapTag;
            switch (data.Item1)
            {
                case 0:
                    trapTag = "Explosive Bomb";
                    break;
                case 1:
                    trapTag = "Frost Bomb";
                    break;
                case 2:
                    trapTag = "Barbwire";
                    break;
                default:
                    trapTag = "";
                    break;
            }
            Trap trap = Object.Instantiate(PrefabManager.Prefabs[trapTag]).GetComponent<Trap>();
            Grid grid = grids.Find((grid) => grid.Id == data.Item2);
            trap.Initialize(data.Item3, grid, i);
        }
        for (int i = 0; i < PlayerProgression.PlayerData.Turrets.Count; i++)
        {
            int data = PlayerProgression.PlayerData.Turrets[i];
            Turret turret = Object.Instantiate(PrefabManager.Prefabs["Turret"]).GetComponent<Turret>();
            Grid grid = grids.Find((grid) => grid.Id == data);
            turret.Initialize(grid, i);
        }
    }

    private static void InitializeSoldiers()
    {
        for (int i = 1; i < PlayerProgression.PlayerData.Soldiers.Count; i++)
        {
            int number = PlayerProgression.PlayerData.Soldiers[i];
            for (int j = 0; j < number; j++)
                Barracks.Instance.AddSoldier(i, out _, out _, false);
        }
    }
}
