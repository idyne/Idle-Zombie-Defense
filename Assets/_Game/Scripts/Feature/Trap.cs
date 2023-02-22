using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using TMPro;
public abstract class Trap : Placeable
{
    public enum TrapType { EXPLOSIVE, FROST, BARBWIRE }
    [SerializeField] private TrapType trapType;
    protected virtual int price { get; set; } = 10;
    [SerializeField] private TrapPriceTag priceTag;
    [SerializeField] protected GameObject explodedMesh, mesh;
    public int saveDataIndex = -1;
    private bool bought { get => saveDataIndex >= 0; }
    public bool Exploded { get; protected set; } = false;
    public override bool CanSelect { get => !Exploded; }

    protected virtual void Awake()
    {
        UIAnimationSequencer.OnOutWaveUIActivated.AddListener(() => { if (Exploded) priceTag.Show(); });
        WaveController.Instance.OnWaveStart.AddListener(priceTag.Hide);
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            if (money >= price && !priceTag.ButtonEnabled) priceTag.EnableButton();
            else if (money < price && priceTag.ButtonEnabled) priceTag.DisableButton();
            priceTag.SetPrice(price);
        });
        UIAnimationSequencer.OnNewZone.AddListener(() =>
        {
            if (grid)
                Attach(TowerController.Instance.GetCurrentTower().TrapPlacementController.GetGrid(grid.Id), false, true);
        });
    }

    protected virtual void Start()
    {
        if (PlayerProgression.MONEY >= price && !priceTag.ButtonEnabled) priceTag.EnableButton();
        else if (PlayerProgression.MONEY < price && priceTag.ButtonEnabled) priceTag.DisableButton();
        priceTag.SetPrice(price);
        if (Exploded)
            priceTag.Show();
        else
            priceTag.Hide();
    }

    public abstract void Initialize(bool exploded, Grid grid, int saveDataIndex);

    public void Detonate()
    {
        if (PauseButton.Paused || !Explode()) return;
        if (!PlayerProgression.HasEverDetonated)
            PlayerProgression.HasEverDetonated = true;
        (int, int, bool) saveData = PlayerProgression.PlayerData.Traps[saveDataIndex];
        saveData.Item3 = true;
        PlayerProgression.PlayerData.Traps[saveDataIndex] = saveData;
    }

    protected abstract bool Explode();
    public void Rebuy()
    {
        if (!Exploded || PlayerProgression.MONEY < price) return;
        mesh.SetActive(true);
        explodedMesh.SetActive(false);
        Exploded = false;
        priceTag.Hide();
        (int, int, bool) saveData = PlayerProgression.PlayerData.Traps[saveDataIndex];
        saveData.Item3 = false;
        PlayerProgression.PlayerData.Traps[saveDataIndex] = saveData;
        PlayerProgression.MONEY -= price;
    }

    public override void Attach(Grid grid, bool sound = true, bool init = false)
    {
        base.Attach(grid, sound, init);
        if (bought)
        {
            (int, int, bool) data = PlayerProgression.PlayerData.Traps[saveDataIndex];
            data.Item2 = grid.Id;
            PlayerProgression.PlayerData.Traps[saveDataIndex] = data;
        }
        else
        {
            if (trapType == TrapType.EXPLOSIVE)
                PlayerProgression.MONEY -= CostManager.GetTNTPrice();
            else if (trapType == TrapType.FROST)
                PlayerProgression.MONEY -= CostManager.GetFrostPrice();
            else if (trapType == TrapType.BARBWIRE)
                PlayerProgression.MONEY -= CostManager.GetBarbwirePrice();
            saveDataIndex = PlayerProgression.PlayerData.Traps.Count;
            PlayerProgression.PlayerData.Traps.Add(((int)trapType, grid.Id, false));
            PlayerProgression.MONEY = PlayerProgression.MONEY;
        }

    }
}
