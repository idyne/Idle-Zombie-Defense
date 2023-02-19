using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FateGames;
using DG.Tweening;
using UnityEngine.UI;
using static LevelManager;

public class UIAreaClearingEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI dayText, moneyText, upgradePointText;
    [SerializeField] private RectTransform fromMoney, toMoney, fromUpgrade, toUpgrade;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Button button;
    private int money = 0;
    private int upgradePoints = 0;

    public void PlaySound()
    {
        SoundFX.PlaySound("UI Button Sound");
    }

    public void Show(int money, int upgradePoints)
    {
        SoundFX.PlaySound("Area Cleared Sound");
        gameObject.SetActive(true);
        button.interactable = true;
        animator.SetTrigger("Fade In");
        int day = GetCycleDay(Day - 1);
        dayText.text = "DAY " + day + "\nCOMPLETED";
        moneyText.text = "+ " + money;
        upgradePointText.text = "+ " + upgradePoints;
        this.money = money;
        this.upgradePoints = upgradePoints;
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        PlayerProgression.UPGRADE_POINT = PlayerProgression.UPGRADE_POINT;
    }
    public void SpreadMoney(int count)
    {
        button.interactable = false;
        int delta = money / count;
        int last = money - delta * count;
        for (int i = 0; i < count; i++)
        {
            RectTransform rectTransform = ObjectPooler.SpawnFromPool("UI Money Image", Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();

            rectTransform.SetParent(canvasTransform);
            rectTransform.position = fromMoney.position;
            float radius = Screen.width / 3f;
            Vector2 pos = (Vector2)rectTransform.position + new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
            bool isLast = i == count - 1;

            rectTransform.DOMove(pos, 0.5f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                bool isReallyLast = isLast;
                rectTransform.DOMove(toMoney.position, 1.5f + Random.Range(-0.3f, 0.3f)).SetEase(Ease.InQuint).OnComplete(() =>
                {
                    rectTransform.gameObject.SetActive(false);
                    if (isReallyLast) PlayerProgression.MONEY += delta + last;
                    else PlayerProgression.MONEY += delta;
                });
            });
        }
        DOVirtual.DelayedCall(3, Hide, false);
    }

    public void SpreadUpgradePoints(int count)
    {
        //button.interactable = false;
        count = Mathf.Clamp(count, 0, upgradePoints);
        int delta = upgradePoints / count;
        int last = upgradePoints - delta * count;
        for (int i = 0; i < count; i++)
        {
            RectTransform rectTransform = ObjectPooler.SpawnFromPool("UI Upgrade Point Image", Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();

            rectTransform.SetParent(canvasTransform);
            rectTransform.position = fromUpgrade.position;
            float radius = Screen.width / 3f;
            Vector2 pos = (Vector2)rectTransform.position + new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
            bool isLast = i == count - 1;

            rectTransform.DOMove(pos, 0.5f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                bool isReallyLast = isLast;
                rectTransform.DOMove(toUpgrade.position, 1.5f + Random.Range(-0.3f, 0.3f)).SetEase(Ease.InQuint).OnComplete(() =>
                {
                    rectTransform.gameObject.SetActive(false);
                    if (isReallyLast) PlayerProgression.UPGRADE_POINT += delta + last;
                    else PlayerProgression.UPGRADE_POINT += delta;
                });
            });
        }
        //DOVirtual.DelayedCall(3, Hide);
    }

    public void Hide()
    {
        animator.SetTrigger("Fade Out");
        DOVirtual.DelayedCall(0.163f, () => { gameObject.SetActive(false); }, false);
    }
}
