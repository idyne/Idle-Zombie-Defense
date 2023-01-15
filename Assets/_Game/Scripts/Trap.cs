using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using TMPro;
public abstract class Trap : Placeable
{
    [SerializeField] protected int price = 100;
    [SerializeField] private TrapPriceTag priceTag;
    [SerializeField] protected GameObject explodedMesh, mesh;
    public int saveDataIndex = -1;
    public bool Exploded { get; protected set; } = false;
    public override bool CanSelect { get => !Exploded; }

    private void Awake()
    {
        UIAnimationSequencer.OnOutWaveUIActivated.AddListener(() => { if (Exploded) priceTag.Show(); });
        WaveController.Instance.OnWaveStart.AddListener(priceTag.Hide);
        PlayerProgression.OnMoneyChanged.AddListener((money, change) =>
        {
            if (money >= price && !priceTag.ButtonEnabled) priceTag.EnableButton();
            else if (money < price && priceTag.ButtonEnabled) priceTag.DisableButton();
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

    public override void Attach(Grid grid)
    {
        base.Attach(grid);
        (int, int, bool) data = PlayerProgression.PlayerData.Traps[saveDataIndex];
        data.Item2 = grid.Id;
        PlayerProgression.PlayerData.Traps[saveDataIndex] = data;
    }
}
