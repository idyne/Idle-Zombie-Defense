using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FateGames;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }
    [SerializeField] private UIButton soldierButton, repairButton, incomeButton, mergeButton, startButton, upgradesButton;
    [SerializeField] private GameObject inWaveButtons, outWaveButtons;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        WaveController.Instance.OnWaveStart.AddListener(SwitchInWave);
        WaveController.Instance.OnWaveStart.AddListener(UpdateUpgradesButton);
        WaveController.Instance.OnWaveEnd.AddListener(HideInWaveButtons);
        PauseButton.OnPause.AddListener(HideInWaveButtons);
        PauseButton.OnResume.AddListener(ShowInWaveButtons);
    }

    public void HideAll()
    {
        soldierButton.Hide();
        repairButton.Hide();
        incomeButton.Hide();
        mergeButton.Hide();
    }

    public void ShowAll()
    {
        soldierButton.Show();
        repairButton.Show();
        incomeButton.Show();
        //mergeButton.Show();
    }

    public void SwitchInWave()
    {
        HideOutWaveButtons();
        ShowInWaveButtons();
    }
    public void SwitchOutWave()
    {
        HideInWaveButtons();
        ShowOutWaveButtons();
    }

    public void ShowInWaveButtons() => inWaveButtons.SetActive(true);
    public void HideInWaveButtons() => inWaveButtons.SetActive(false);
    public void ShowOutWaveButtons() => outWaveButtons.SetActive(true);
    public void HideOutWaveButtons() => outWaveButtons.SetActive(false);

    public void UpdateSoldierButton(int cost, bool isFull, bool isMerging)
    {
        if (!isFull)
            soldierButton.SetText(UIMoney.FormatMoney(cost));
        else soldierButton.SetText("Full");
        if (!PlayerProgression.CanAfford(cost) || isFull || isMerging) soldierButton.Deactivate();
        else soldierButton.Activate();
    }

    public void UpdateFireRateButton(int cost, bool isMaxedOut)
    {
        if (!isMaxedOut)
            repairButton.SetText(UIMoney.FormatMoney(cost));
        else
            repairButton.SetText("Maxed Out");
        if (!PlayerProgression.CanAfford(cost) || isMaxedOut) repairButton.Deactivate();
        else repairButton.Activate();
    }

    public void UpdateIncomeButton(int cost)
    {
        incomeButton.SetText(UIMoney.FormatMoney(cost));
        if (!PlayerProgression.CanAfford(cost)) incomeButton.Deactivate();
        else incomeButton.Activate();
    }

    public void UpdateStartButton(WaveController.WaveState state)
    {
        if (state == WaveController.WaveState.RUNNING) startButton.Hide();
        else if (state == WaveController.WaveState.WAITING) startButton.Show();
    }

    public void UpdateUpgradesButton()
    {
        if (WaveController.State == WaveController.WaveState.RUNNING) upgradesButton.Hide();
        else if (WaveController.State == WaveController.WaveState.WAITING) upgradesButton.Show();
    }

    public void UpdateMergeButton(int cost, bool canMerge)
    {
        mergeButton.SetText(UIMoney.FormatMoney(cost));
        if (!canMerge) mergeButton.Hide();
        else mergeButton.Show();
        if (!PlayerProgression.CanAfford(cost)) mergeButton.Deactivate();
        else mergeButton.Activate();
    }

}
