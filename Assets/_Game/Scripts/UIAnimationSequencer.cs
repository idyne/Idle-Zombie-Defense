using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static WaveController;

public class UIAnimationSequencer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Animator dayAnimator;
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private TimePeriodAnimation timePeriodAnimation;
    [SerializeField] private DayCycler dayCycler;
    [SerializeField] private EnvironmentChanger environmentChanger;
    [SerializeField] private MapController mapController;
    [SerializeField] private RegionColorChanger regionColorChanger;
    [SerializeField] private GameObject mapControllerButton;
    [SerializeField] private ZombieBar zombieBar;
    [SerializeField] private Tower tower;
    [SerializeField] private SoldierUnlocked soldierUnlocked;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject surviveText;
    [SerializeField] private AreaCleared areaCleared;
    [SerializeField] private GameObject areaClearedText;
    [SerializeField] private GameObject UIContainer;
    public static UnityEvent OnOutWaveUIActivated = new();

    private bool goNext = false;
    private bool soldierUnlockedHide = false;
    private bool isNewSession = true;
    private bool areaClearedNext = false;
    private IEnumerator FinishDay()
    {
        yield return new WaitForSeconds(1);
        areaClearedText.SetActive(true);
        yield return new WaitForSeconds(3f);
        areaClearedText.SetActive(false);
        UIContainer.SetActive(false);
        zombieBar.Hide();
        LevelBar.Instance.Hide();
        areaCleared.Show(Barracks.Instance.GetSoldierCost() * 2 + 6);
        yield return new WaitUntil(() => areaClearedNext);
        areaClearedNext = false;
        yield return new WaitForSeconds(3.2f);
    }

    public void GoAreaClearedNext() => areaClearedNext = true;

    public IEnumerator GoCurrentTimePeriod()
    {
        PlayerProgression.MONEY = PlayerProgression.MONEY;

        //Oyuna yeni baþlama
        if (isNewSession && Day == 1 && CurrentTimePeriod == TimePeriod.Morning)
        {
            print("start new game");
            surviveText.SetActive(ZoneLevel == 1);
            dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
            environmentChanger.SetZone(ZoneLevel);
            zombieBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            zombieBar.SetDay(NormalizedDay);
            LevelBar.Instance.SetDay(NormalizedDay);
            tutorialManager.Show();
            zombieBar.Show();
            zombieBar.GoDown(false);
            LevelBar.Instance.Show();
            LevelBar.Instance.SetDay(NormalizedDay);
            ButtonManager.Instance.UpdateStartButton(State);
            ButtonManager.Instance.UpdateUpgradesButton();
            ButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Oyuna giriþ
        else if (isNewSession & !(Day == 1 && CurrentTimePeriod == TimePeriod.Morning))
        {
            print("start game");
            surviveText.SetActive(ZoneLevel == 1);
            dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
            environmentChanger.SetZone(ZoneLevel);
            zombieBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            zombieBar.SetDay(NormalizedDay);
            LevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            zombieBar.Show();
            zombieBar.GoDown(false);
            LevelBar.Instance.Show();
            LevelBar.Instance.SetDay(NormalizedDay);
            ButtonManager.Instance.UpdateStartButton(State);
            ButtonManager.Instance.UpdateUpgradesButton();
            ButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki zonea geçme
        else if (!isNewSession && CurrentTimePeriod == TimePeriod.Morning && (Day == 8 || Day == 22 || Day == 42))
        {
            print("newZone");
            yield return FinishDay();
            yield return GoCurrentZone();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            zombieBar.Show();
            zombieBar.GoDown(false);
            LevelBar.Instance.Show();
            LevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            ButtonManager.Instance.UpdateStartButton(State);
            ButtonManager.Instance.UpdateUpgradesButton();
            ButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki güne geçme
        else if (!isNewSession && CurrentTimePeriod == TimePeriod.Morning && !(Day == 8 || Day == 22 || Day == 42))
        {
            print("next day");
            yield return FinishDay();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            zombieBar.Show();
            zombieBar.GoDown(false);
            LevelBar.Instance.Show();
            LevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            ButtonManager.Instance.UpdateStartButton(State);
            ButtonManager.Instance.UpdateUpgradesButton();
            ButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki time perioda geçme
        else if (!isNewSession && CurrentTimePeriod != TimePeriod.Morning)
        {
            print("next time period");
            StartCoroutine(dayCycler.SetTimePeriod(CurrentTimePeriod));
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            zombieBar.Show();
            zombieBar.GoDown().OnComplete(() =>
            {
                LevelBar.Instance.Show();
                LevelBar.Instance.SetDay(NormalizedDay);
                ButtonManager.Instance.UpdateStartButton(State);
                ButtonManager.Instance.UpdateUpgradesButton();
                ButtonManager.Instance.ShowOutWaveButtons();
                OnOutWaveUIActivated.Invoke();
            });
        }
        isNewSession = false;
    }

    private void SetBars()
    {
        zombieBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, true);
        zombieBar.SetDay(NormalizedDay);
        if (!isNewSession)
        {
            zombieBar.GoDown().OnComplete(() =>
            {
                LevelBar.Instance.Show();
                LevelBar.Instance.SetDay(NormalizedDay);
            });
        }
        else
        {
            zombieBar.GoDown(false);
            LevelBar.Instance.Show();
            LevelBar.Instance.SetDay(NormalizedDay);
        }
    }

    private IEnumerator GoCurrentDay()
    {
        dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
        zombieBar.SetPercent(0, false);
        zombieBar.SetDay(NormalizedDay);
        yield return PlayDayAnimation();
    }
    private IEnumerator GoCurrentZone()
    {
        mapController.Show();
        Barracks.Instance.ClearSoldiers();
        PlayerProgression.PlayerData.IncomeLevel = 1;
        PlayerProgression.PlayerData.FireRateLevel = 1;
        PlayerProgression.PlayerData.BaseDefenseLevel = 1;
        PlayerProgression.PlayerData.TrapCapacity = 0;
        PlayerProgression.PlayerData.TurretCapacity = 0;
        PlayerProgression.PlayerData.SoldierMergeLevel = 1;
        PlayerProgression.MONEY = 0;
        environmentChanger.SetZone(ZoneLevel);
        tower.SetTower();
        surviveText.SetActive(ZoneLevel == 1);
        mapController.GoPosition(ZoneLevel - 2);
        mapAnimator.SetTrigger("Fade");
        yield return new WaitForSeconds(1);
        yield return regionColorChanger.AnimateRegion(ZoneLevel - 1);
        mapControllerButton.SetActive(true);
        yield return new WaitUntil(() => goNext);
        goNext = false;
        mapControllerButton.SetActive(false);
        mapController.FillPath(ZoneLevel - 2);
        yield return new WaitForSeconds(2);
        mapController.Hide();
        soldierUnlocked.Show(ZoneLevel + 3);
        yield return new WaitUntil(() => soldierUnlockedHide);
        soldierUnlockedHide = false;
    }
    private IEnumerator PlayDayAnimation()
    {
        dayText.text = "DAY " + NormalizedDay;
        dayAnimator.SetTrigger("Day");
        yield return new WaitForSeconds(3.183f);
    }
    public void GoNext()
    {
        goNext = true;
    }
    public void SoldierUnlockedNext()
    {
        soldierUnlockedHide = true;
    }
}
