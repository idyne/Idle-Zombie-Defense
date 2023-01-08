using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class BarrierController : MonoBehaviour
{
    private static BarrierController instance;
    public static BarrierController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BarrierController>();
            return instance;
        }
    }
    private List<Barrier> barriers;
    private int baseRepairCost = 20;
    private bool canRepair { get => damageTaken > 0; }
    private int damageTaken = 0;

    private void Awake()
    {
        barriers = new(FindObjectsOfType<Barrier>());

        foreach (Barrier barrier in barriers)
            barrier.OnHit.AddListener((damage) =>
            {
                damageTaken += damage;
            });
    }

    private int GetRepairCost()
    {
        int totalMaxHealth = barriers.Count * Barrier.MaxHealth;

        return Mathf.CeilToInt(Mathf.Clamp((damageTaken / (float)totalMaxHealth) * baseRepairCost * Mathf.Pow(1.2f, WaveController.WaveLevel - 1), 5 * Mathf.Pow(1.2f, WaveController.WaveLevel - 1), 99999999));
    }

    public void RepairAll()
    {
        foreach (Barrier barrier in barriers)
            barrier.Repair();
        damageTaken = 0;
        HapticManager.DoHaptic();
    }
}
