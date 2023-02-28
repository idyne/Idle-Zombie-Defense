using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;

public abstract class Bomb : Trap
{
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask zombieLayerMask;
    [SerializeField] private Transform rangeIndicatorTransform;


    [SerializeField] private Outline outlinable;

    protected override void Awake()
    {
        base.Awake();
        rangeIndicatorTransform.localScale = Vector3.one * range;
        OnSelect.AddListener(() => { ShowRangeIndicator(); });
        OnDrop.AddListener(() => { HideRangeIndicator(); });
        HideRangeIndicator();
        DisableOutlinable();
    }
    protected override void Start()
    {
        base.Start();
        
        
        
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

    public void HideRangeIndicator(float duration = -1)
    {
        if (DOTween.Kill(this) > 0) print("Turret hiderangeindicator tween killed");
        rangeIndicatorTransform.gameObject.SetActive(false);
        if (duration > 0)
            DOVirtual.DelayedCall(duration, () => { ShowRangeIndicator(); }, false);
    }
    public void ShowRangeIndicator(float duration = -1)
    {
        if (DOTween.Kill(this) > 0) print("Turret showrangeindicator tween killed");
        rangeIndicatorTransform.gameObject.SetActive(true);
        if (duration > 0)
            DOVirtual.DelayedCall(duration, () => { HideRangeIndicator(); }, false);
    }

    public override void Initialize(bool exploded, Grid grid, int saveDataIndex)
    {
        mesh.SetActive(!exploded);
        explodedMesh.SetActive(exploded);
        Exploded = exploded;
        this.saveDataIndex = saveDataIndex;
        Attach(grid, init: true);
    }

}
