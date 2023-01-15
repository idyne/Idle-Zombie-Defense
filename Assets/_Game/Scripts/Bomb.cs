using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public abstract class Bomb : Trap
{
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask zombieLayerMask;
    [SerializeField] private Transform rangeIndicatorTransform;


    [SerializeField] private Outline outlinable;


    protected override void Start()
    {
        base.Start();
        rangeIndicatorTransform.localScale = Vector3.one * range;
        HideRangeIndicator();
        DisableOutlinable();
        OnSelect.AddListener(ShowRangeIndicator);
        OnDrop.AddListener(HideRangeIndicator);
        WaveController.Instance.OnWaveStart.AddListener(EnableOutlinable);
        WaveController.Instance.OnWaveEnd.AddListener(DisableOutlinable);
    }

    private void DisableOutlinable()
    {
        outlinable.enabled = false;
    }
    private void EnableOutlinable()
    {
        outlinable.enabled = true;
    }

    protected virtual void Update()
    {
        if (rangeIndicatorTransform.gameObject.activeSelf)
        {
            Vector3 pos = rangeIndicatorTransform.position;
            pos.y = 0.01f;
            rangeIndicatorTransform.position = pos;
        }
    }

    private void HideRangeIndicator()
    {
        rangeIndicatorTransform.gameObject.SetActive(false);
    }
    private void ShowRangeIndicator()
    {
        rangeIndicatorTransform.gameObject.SetActive(true);
    }

    public override void Initialize(bool exploded, Grid grid, int saveDataIndex)
    {
        mesh.SetActive(!exploded);
        explodedMesh.SetActive(exploded);
        Exploded = exploded;
        this.saveDataIndex = saveDataIndex;
        Attach(grid);
    }
}
