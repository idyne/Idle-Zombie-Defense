using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static LevelManager;

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
                money = Settings.World1.FinishDayPrize;
                break;
            case 2:
                money = Settings.World2.FinishDayPrize;
                break;
        }
        areaCleared.Show(Mathf.CeilToInt(money), 5);
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
        yield return phaseCleared.Show(Mathf.CeilToInt(money), 1);
        uiDayBar.Hide();
        UILevelBar.Instance.Hide();
        yield return new WaitForSeconds(0.7f);
    }

    public void GoAreaClearedNext() => areaClearedNext = true;

    public IEnumerator GoCurrentTimePeriod()
    {
        PlayerProgression.MONEY = PlayerProgression.MONEY;

        //Oyuna yeni baþlama
        if (isNewSession && Day == 1 && NewDay)
        {
            print("start new game");
            surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
            dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
            environmentChanger.SetEnvironment();
            uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            uiDayBar.SetDay(WorldDay);
            UILevelBar.Instance.SetDay();
            tutorialManager.Show();
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateBaseUpgradesButton();
            UIButtonManager.Instance.UpdateTrapUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Oyuna giriþ
        else if (isNewSession & !(Day == 1 && CurrentTimePeriod == TimePeriod.Morning))
        {
            print("start game");
            if (CurrentTimePeriod == TimePeriod.Night)
                tower.TurnOnLights();
            surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
            dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
            environmentChanger.SetEnvironment();
            uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, false);
            uiDayBar.SetDay(WorldDay);
            UILevelBar.Instance.SetDay();
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateTrapUpgradesButton();
            UIButtonManager.Instance.UpdateBaseUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki worlde geçme
        else if (!isNewSession && NewWorld)
        {
            print("newworld");
            yield return FinishDay();
            yield return GoCurrentWorld();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateTrapUpgradesButton();
            UIButtonManager.Instance.UpdateBaseUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }
        //Sonraki zonea geçme
        else if (!isNewSession && NewZone)
        {
            print("newZone");
            yield return FinishDay();
            yield return GoCurrentZone();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateTrapUpgradesButton();
            UIButtonManager.Instance.UpdateBaseUpgradesButton();
            UIButtonManager.Instance.ShowOutWaveButtons();
            OnOutWaveUIActivated.Invoke();
        }

        //Sonraki güne geçme
        else if (!isNewSession && NewDay && !NewZone)
        {
            print("next day");
            yield return FinishDay();
            yield return GoCurrentDay();
            UIContainer.SetActive(true);
            uiDayBar.Show();
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
            yield return timePeriodAnimation.SetTimePeriod(CurrentTimePeriod);
            yield return AdManager.ShowInterstitial();
            UIButtonManager.Instance.UpdateStartButton();
            UIButtonManager.Instance.UpdateTrapUpgradesButton();
            UIButtonManager.Instance.UpdateBaseUpgradesButton();
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
            if (WaveLevel > RemoteConfigValues.int_grace_level)
            {
                bool isAdDone = false;
                void PauseGame() => Time.timeScale = 0;
                void ContinueAfterAd() { isAdDone = true; Time.timeScale = 1; }
                if (AdvertisementManager.IsCanShowInterstital && AdvertisementManager.IsInterstitialdAdReady())
                    AdvertisementManager.ShowInterstitial(OnStartAdEvent: PauseGame, OnFinishAdEvent: ContinueAfterAd, OnFailedAdEvent: ContinueAfterAd);
                yield return new WaitUntil(() => isAdDone);
            }
            if (CurrentTimePeriod == TimePeriod.Night)
                tower.TurnOnLights();
            uiDayBar.Show();
            uiDayBar.GoDown().OnComplete(() =>
            {
                UILevelBar.Instance.Show();
                UILevelBar.Instance.SetDay();
                UIButtonManager.Instance.UpdateStartButton();
                UIButtonManager.Instance.UpdateTrapUpgradesButton();
                UIButtonManager.Instance.UpdateBaseUpgradesButton();
                UIButtonManager.Instance.ShowOutWaveButtons();
                OnOutWaveUIActivated.Invoke();
            });
        }
        isNewSession = false;
    }

    private void SetBars()
    {
        uiDayBar.SetPercent(((int)CurrentTimePeriod) * 0.25f, true);
        uiDayBar.SetDay(WorldDay);
        if (!isNewSession)
        {
            uiDayBar.GoDown().OnComplete(() =>
            {
                UILevelBar.Instance.Show();
                UILevelBar.Instance.SetDay();
            });
        }
        else
        {
            uiDayBar.GoDown(false);
            UILevelBar.Instance.Show();
            UILevelBar.Instance.SetDay();
        }
    }

    private IEnumerator GoCurrentDay()
    {
        dayCycler.SetTimePeriodWithoutAnimation(CurrentTimePeriod);
        uiDayBar.SetPercent(0, false);
        uiDayBar.SetDay(WorldDay);
        tower.TurnOffLights();
        yield return PlayDayAnimation();
    }

    private void ResetProgress()
    {
        Barracks.Instance.ClearSoldiers();
        PlayerProgression.PlayerData.IncomeLevel = 1;
        PlayerProgression.PlayerData.FireRateLevel = 1;
        PlayerProgression.PlayerData.BaseDefenseLevel = 1;
        PlayerProgression.PlayerData.TNTLevel = 1;
        PlayerProgression.PlayerData.FrostLevel = 1;
        PlayerProgression.PlayerData.BarbwireLevel = 1;
        PlayerProgression.PlayerData.TurretLevel = 1;
        PlayerProgression.PlayerData.SoldierMergeLevel = 1;
        PlayerProgression.PlayerData.Traps.Clear();
        PlayerProgression.PlayerData.Turrets.Clear();
        PlayerProgression.UPGRADE_POINT = 0;
        PlayerProgression.MONEY = 0;
    }
    private IEnumerator GoCurrentZone()
    {
        mapController.Show();
        //ResetProgress();
        environmentChanger.SetEnvironment();
        tower.SetTower();
        surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
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
    private IEnumerator GoCurrentWorld()
    {
        mapController.Show();
        ResetProgress();
        environmentChanger.SetEnvironment();
        tower.SetTower();
        surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
        mapController.GoPosition(1);
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
        /*soldierUnlocked.Show(ZoneLevel + 3);
        yield return new WaitUntil(() => soldierUnlockedHide);
        soldierUnlockedHide = false;*/
    }
    private IEnumerator PlayDayAnimation()
    {
        dayText.text = "DAY " + WorldDay;
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
