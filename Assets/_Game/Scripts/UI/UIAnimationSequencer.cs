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
    //[SerializeField] private Animator mapAnimator;
    [SerializeField] private TimePeriodAnimation timePeriodAnimation;
    [SerializeField] private DayCycler dayCycler;
    [SerializeField] private EnvironmentChanger environmentChanger;
    //[SerializeField] private MapController mapController;
    //[SerializeField] private RegionColorChanger regionColorChanger;
    //[SerializeField] private GameObject mapControllerButton;
    [SerializeField] private UIDayBar uiDayBar;
    private Tower tower;
    [SerializeField] private SoldierUnlocked soldierUnlocked;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject surviveText;
    [SerializeField] private UIAreaClearingEffect areaCleared;
    [SerializeField] private PhaseCleared phaseCleared;
    [SerializeField] private GameObject areaClearedText;
    [SerializeField] private GameObject UIContainer;
    [SerializeField] private ZoneMapController zoneMapController;
    private bool zoneMapSetup = false;
    public static UnityEvent OnOutWaveUIActivated = new();
    public static UnityEvent OnNewZone = new();

    //private bool goNext = false;
    private bool soldierUnlockedHide = false;
    private bool isNewSession = true;
    private bool areaClearedNext = false;
    private SoundFXWorker backgroundMusicSoundWorker;

    private IEnumerator FinishDay()
    {
        yield return new WaitForSeconds(1);
        SoundFX.PlaySound("Phase Cleared Sound");
        areaClearedText.SetActive(true);
        yield return new WaitForSeconds(3f);
        areaClearedText.SetActive(false);
        UIContainer.SetActive(false);
        uiDayBar.Hide();
        UILevelBar.Instance.Hide();
        int money = 1;
        int upgradePoints = 1;
        switch (WorldLevel)
        {
            case 1:
                money = Settings.World1.FinishDayPrize;
                break;
            case 2:
                money = Settings.World2.FinishDayPrize;
                break;
        }
        switch (WorldLevel)
        {
            case 1:
                upgradePoints = Settings.World1.FinishDayUpgradePrize;
                break;
            case 2:
                upgradePoints = Settings.World2.FinishDayUpgradePrize;
                break;
        }
        areaCleared.Show(money, upgradePoints);
        yield return new WaitUntil(() => areaClearedNext);
        areaClearedNext = false;
        yield return new WaitForSeconds(3.2f);
    }
    private IEnumerator FinishPhase()
    {
        int money = 1;
        int upgradePoints = 1;
        switch (WorldLevel)
        {
            case 1:
                money = Settings.World1.FinishPhasePrize;
                break;
            case 2:
                money = Settings.World2.FinishPhasePrize;
                break;
        }
        switch (WorldLevel)
        {
            case 1:
                upgradePoints = Settings.World1.FinishPhaseUpgradePrize;
                break;
            case 2:
                upgradePoints = Settings.World2.FinishPhaseUpgradePrize;
                break;
        }
        yield return phaseCleared.Show(money, upgradePoints);
        uiDayBar.Hide();
        UILevelBar.Instance.Hide();
        yield return new WaitForSeconds(0.7f);
    }

    public void GoAreaClearedNext() => areaClearedNext = true;

    public IEnumerator GoCurrentTimePeriod()
    {
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        PlayerProgression.UPGRADE_POINT = PlayerProgression.UPGRADE_POINT;
        //Oyuna yeni baþlama
        if (isNewSession && Day == 1 && NewDay)
        {
            print("start new game");
            zoneMapController.Open();
            yield return new WaitForSeconds(0.5f);
            zoneMapController.BeginingShow();
            SetTower();
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
            SetTower();
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
            yield return AdManager.ShowInterstitial();
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
            yield return AdManager.ShowInterstitial();
            if (Day == 4)
                MoonSDK.OpenRateUsScreen();
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
            yield return AdManager.ShowInterstitial();
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
        backgroundMusicSoundWorker = SoundFX.PlaySound("Background Music Sound");
        void StopBackgroundMusic()
        {
            backgroundMusicSoundWorker.Stop();
            WaveController.Instance.OnWaveStart.RemoveListener(StopBackgroundMusic);
        }
        WaveController.Instance.OnWaveStart.AddListener(StopBackgroundMusic);

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

    private void SetTower()
    {
        tower?.Deactivate();
        tower = TowerController.Instance.GetCurrentTower();
        tower.Activate();
    }

    private void ResetProgress()
    {
        Barracks.Instance.ClearSoldiers();
        PlayerProgression.PlayerData.IncomeLevel = 1;
        PlayerProgression.PlayerData.FireRateLevel = 1;
        //PlayerProgression.PlayerData.BaseDefenseLevel = 1;
        //PlayerProgression.PlayerData.TNTLevel = 1;
        //PlayerProgression.PlayerData.FrostLevel = 1;
        //PlayerProgression.PlayerData.BarbwireLevel = 1;
        //PlayerProgression.PlayerData.TurretLevel = 1;
        //PlayerProgression.PlayerData.SoldierMergeLevel = 1;
        //PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel = 0;
        //PlayerProgression.PlayerData.AirstrikeLevel = 0;
        PlayerProgression.PlayerData.Traps.Clear();
        PlayerProgression.PlayerData.Turrets.Clear();
        //PlayerProgression.UPGRADE_POINT = 0;
        PlayerProgression.MONEY = 0;
    }
    private IEnumerator GoCurrentZone()
    {
        //mapController.Show();
        //ResetProgress();
        environmentChanger.SetEnvironment();
        SetTower();
        OnNewZone.Invoke();
        Barracks.Instance.PlaceSoldiers(true);
        surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
        zoneMapController.Open();
        if (!zoneMapSetup)
        {
            int lastAchievedZoneIndex = (WorldLevel - 1) * 3 + ZoneLevel - 2;
            zoneMapController.SetupZonesAndPaths(lastAchievedZoneIndex);
            zoneMapSetup = true;
        }
        yield return new WaitForSeconds(0.5f);
        int previousZoneIndex = (WorldLevel - 1) * 3 + ZoneLevel - 2;
        zoneMapController.FindNextPathOnWorld(previousZoneIndex);
        yield return new WaitForSeconds(1.5f);
        zoneMapController.PlayPath(previousZoneIndex);
        //mapController.GoPosition(ZoneLevel - 2);
        //mapAnimator.SetTrigger("Fade");
        yield return new WaitUntil(() => zoneMapController.Closed);

        //yield return regionColorChanger.AnimateRegion(ZoneLevel - 1);
        //mapControllerButton.SetActive(true);
        //yield return new WaitUntil(() => goNext);
        //goNext = false;
        //mapControllerButton.SetActive(false);
        //mapController.FillPath(ZoneLevel - 2);
        //yield return new WaitForSeconds(2);
        //mapController.Hide();
        if (CycleNumber == 1)
        {
            int soldierLevel = 3;
            if (WorldLevel == 1)
            {
                soldierLevel = ZoneLevel + 2;
            }
            else if (WorldLevel == 2 && ZoneLevel == 2)
                soldierLevel = ZoneLevel + 5;
            soldierUnlocked.Show(soldierLevel);
            yield return new WaitUntil(() => soldierUnlockedHide);
            soldierUnlockedHide = false;
        }

    }
    private IEnumerator GoCurrentWorld()
    {
        //mapController.Show();
        ResetProgress();
        environmentChanger.SetEnvironment();
        SetTower();
        surviveText.SetActive(ZoneLevel == 1 && WorldLevel == 1);
        zoneMapController.Open();
        if (!zoneMapSetup)
        {
            int lastAchievedZoneIndex = (WorldLevel - 1) * 3 + ZoneLevel - 2;
            zoneMapController.SetupZonesAndPaths(lastAchievedZoneIndex);
            zoneMapSetup = true;
        }
        yield return new WaitForSeconds(0.5f);
        int previousZoneIndex = (WorldLevel - 1) * 3 + ZoneLevel - 2;
        zoneMapController.FindNextPathOnWorld(previousZoneIndex);
        yield return new WaitForSeconds(1.5f);
        zoneMapController.PlayPath(previousZoneIndex);
        yield return new WaitUntil(() => zoneMapController.Closed);
        //mapController.GoPosition(1);
        //mapAnimator.SetTrigger("Fade");
        //yield return new WaitForSeconds(1);
        //yield return regionColorChanger.AnimateRegion(ZoneLevel - 1);
        //mapControllerButton.SetActive(true);
        //yield return new WaitUntil(() => goNext);
        //goNext = false;
        //mapControllerButton.SetActive(false);
        //mapController.FillPath(ZoneLevel - 2);
        //yield return new WaitForSeconds(2);
        //mapController.Hide();
        /*soldierUnlocked.Show(ZoneLevel + 3);
        yield return new WaitUntil(() => soldierUnlockedHide);
        soldierUnlockedHide = false;*/
        if (CycleNumber == 1)
        {
            int soldierLevel = ZoneLevel + 5;
            soldierUnlocked.Show(soldierLevel);
            yield return new WaitUntil(() => soldierUnlockedHide);
            soldierUnlockedHide = false;
        }

    }
    private IEnumerator PlayDayAnimation()
    {
        dayText.text = "DAY " + WorldDay;
        dayAnimator.SetTrigger("Day");
        yield return new WaitForSeconds(3.183f);
    }
    public void GoNext()
    {
        //goNext = true;
    }
    public void SoldierUnlockedNext()
    {
        soldierUnlockedHide = true;
    }
}
