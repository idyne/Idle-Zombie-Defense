using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using System.Linq;
public class SaveLoader : MonoBehaviour
{
    private void Start()
    {
        List<Grid> grids = new();
        PlacementController[] placementControllers = FindObjectsOfType<PlacementController>();
        for (int i = 0; i < placementControllers.Length; i++)
        {
            grids.AddRange(placementControllers[i].Grids);
        }

        for (int i = 0; i < PlayerProgression.PlayerData.Traps.Count; i++)
        {
            (int, int, bool) data = PlayerProgression.PlayerData.Traps[i];
            Trap trap = Instantiate(PrefabManager.Prefabs[data.Item1 == 0 ? "Explosive Bomb" : "Frost Bomb"]).GetComponent<Trap>();
            Grid grid = grids.Find((grid) => grid.Id == data.Item2);
            trap.Initialize(data.Item3, grid, i);
        }
        for (int i = 0; i < PlayerProgression.PlayerData.Turrets.Count; i++)
        {
            int data = PlayerProgression.PlayerData.Turrets[i];
            Turret turret = Instantiate(PrefabManager.Prefabs["Turret"]).GetComponent<Turret>();
            Grid grid = grids.Find((grid) => grid.Id == data);
            turret.Initialize(grid, i);
        }
    }
}
