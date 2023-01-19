using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseCleared : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private RectTransform from, to;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject phaseClearedText;
    private int money = 0;


    public IEnumerator Show(int money)
    {
        phaseClearedText.SetActive(true);
        this.money = money;
        SpreadMoney(15);
        yield return new WaitForSeconds(2.5f);
        phaseClearedText.SetActive(false);
        moneyText.text = "+ " + money;
        PlayerProgression.MONEY = PlayerProgression.MONEY;
        
    }
    public void SpreadMoney(int count)
    {
        print(money);
        int delta = money / count;
        print(delta);
        int last = money - delta * count;
        print(last);
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
}
