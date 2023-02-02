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
    [SerializeField] private UIDayBar uiDayBar;
    [SerializeField] private Tower tower;
    [SerializeField] private SoldierUnlocked soldierUnlocked;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject surviveText;
    [SerializeField] private UIAreaClearingEffect areaCleared;
    [SerializeField] private PhaseCleared phaseCleared;
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
        uiDayBar.Hide();
        UILevelBar.Instance.Hide();
        float money = 1;
        switch (ZoneLevel)
        {
            case 1:
                money = Settings.Zone1.FinishDayPrize;
                break;
            case 2:
                money = Settings.Zone2.FinishDayPrize;
                break;
            case 3:
                money = Settings.Zone3.FinishDayPrize;
                break;
            case 4:
                money = Settings.Zone4.FinishDayPrize;
                break;
        }
        areaCleared.Show(Mathf.CeilToInt(money));
        yield return new WaitUntil(() => areaClearedNext);
        areaClearedNext = false;
        yield return new WaitForSeconds(3.2f);
    }
    private IEnumerator FinishPhase()
    {
        float money = NormalizedDay * 20;
        switch (ZoneLevel)
        {
            case 2:
                money *= 1.5f;
                break;
            case 3:
                money *= 2f;
                break;
            case 4:
                money *= 2f;
                break;
            default:
                break;
        }
        yield return phaseCleared.Show(Mathf.CeilToInt(money));
        uiDayBar.Hide();
        UILevelBar.Instance.Hide();
        yield return new WaitForSeconds(0.7f);
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
            uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            uiDayBar.SetDay(NormalizedDay);
            UILevelBar.Instance.SetDay(NormalizedDay);
            tutorialManager.Show();
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay(NormalizedDay);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Oyuna giriþ
        else if (isNewSession & !(Day == 1 && CurrentTimePeriod == TimePeriod.Morning))
        {
            print("start game");
            if (CurrentTimePeriod == TimePeriod.Night)
                tower.TurnOnLights();
            surviveText.SetActive(ZoneLevel == 1);
            dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
            environmentChanger.SetZone(ZoneLevel);
            uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            uiDayBar.SetDay(NormalizedDay);
            UILevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay(NormalizedDay);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
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
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki güne geçme
        else if (!isNewSession && CurrentTimePeriod == TimePeriod.Morning && !(Day == 8 || Day == 22 || Day == 42))
        {
            print("next day");
            yield return FinishDay();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay(NormalizedDay);
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki time perioda geçme
        else if (!isNewSession && CurrentTimePeriod != TimePeriod.Morning)
        {
            print("next time period");
            yield return FinishPhase();
            StartCoroutine(dayCycler.SetTimePeriod(CurrentTimePeriod));
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            if (CurrentTimePeriod == TimePeriod.Night)
                tower.TurnOnLights();
            uiDayBar.Show();
            uiDayBar.GoDown().OnComplete(() =>
            {
                UILevelBar.Instance.Show();
                UILevelBar.Instance.SetDay(NormalizedDay);
                UIButtonManager.Instance.UpdateStartButton();
                UIButtonManager.Instance.UpdateUpgradesButton();
                UIButtonManager.Instance.ShowOutWaveButtons();
                OnOutWaveUIActivated.Invoke();
            });
        }
        isNewSession = false;
    }

    private void SetBars()
    {
        uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, true);
        uiDayBar.SetDay(NormalizedDay);
        if (!isNewSession)
        {
            uiDayBar.GoDown().OnComplete(() =>
            {
                UILevelBar.Instance.Show();
                UILevelBar.Instance.SetDay(NormalizedDay);
            });
        }
        else
        {
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay(NormalizedDay);
        }
    }

    private IEnumerator GoCurrentDay()
    {
        dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
        uiDayBar.SetPercent(0, false);
        uiDayBar.SetDay(NormalizedDay);
        tower.TurnOffLights();
        yield return PlayDayAnimation();
    }
    private IEnumerator GoCurrentZone()
    {
        mapController.Show();
        Barracks.Instance.ClearSoldiers();
        PlayerProgression.PlayerData.IncomeLevel = 1;
        PlayerProgression.PlayerData.FireRateLevel = 1;
        PlayerProgression.PlayerData.BaseDefenseLevel = 1;
        PlayerProgression.PlayerData.TrapCapacity = 1;
        PlayerProgression.PlayerData.TurretCapacity = 1;
        PlayerProgression.PlayerData.SoldierMergeLevel = 1;
        PlayerProgression.PlayerData.Traps.Clear();
        PlayerProgression.PlayerData.Turrets.Clear();
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
