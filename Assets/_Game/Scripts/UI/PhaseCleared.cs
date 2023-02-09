using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseCleared : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText, upgradePointsText;
    [SerializeField] private RectTransform from, to, fromUpgrade, toUpgrade;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject phaseClearedText;
    private int money = 0;
    private int upgradePoints = 0;


    public IEnumerator Show(int money, int upgradePoints)
    {
        phaseClearedText.SetActive(true);
        this.money = money;
        this.upgradePoints = upgradePoints;
        SpreadMoney(15);
        SpreadUpgradePoints(1);
        yield return new WaitForSeconds(2.5f);
        phaseClearedText.SetActive(false);
        moneyText.text = "+ " + money;
        upgradePointsText.text = "+ " + upgradePoints;
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        PlayerProgression.UPGRADE_POINT = PlayerProgression.UPGRADE_POINT;
        
    }
    public void SpreadMoney(int count)
    {
        int delta = money / count;
        int last = money - delta * count;
        for (int i = 0; i < count; i++)
        {
            RectTransform rectTransform = ObjectPooler.SpawnFromPool("UI Money Image", Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();

            rectTransform.SetParent(canvasTransform);
            rectTransform.position = from.position;
            float radius = Screen.width / 3f;
            Vector2 pos = (Vector2)rectTransform.position + new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
            bool isLast = i == count - 1;

            rectTransform.DOMove(pos, 0.5f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                bool isReallyLast = isLast;
                rectTransform.DOMove(to.position, 1.5f + Random.Range(-0.3f, 0.3f)).SetEase(Ease.InQuint).OnComplete(() =>
                {
                    rectTransform.gameObject.SetActive(false);
                    if (isReallyLast) PlayerProgression.MONEY += delta + last;
                    else PlayerProgression.MONEY += delta;
                });
            });
        }
    }

    public void SpreadUpgradePoints(int count)
    {
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
    }
}
