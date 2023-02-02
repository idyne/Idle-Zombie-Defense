using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FateGames;
using DG.Tweening;
using UnityEngine.UI;

public class UIAreaClearingEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI dayText, moneyText;
    [SerializeField] private RectTransform from, to;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Button button;
    private int money = 0;


    public void Show(int money)
    {
        gameObject.SetActive(true);
        button.interactable = true;
        animator.SetTrigger("Fade In");
        int day = WaveController.NormalizedDay - 1;
        if (day == 0)
        {
            if (WaveController.ZoneLevel == 2)
                day = 7;
            else if (WaveController.ZoneLevel == 3)
            {
                day = 14;
            }
            else if (WaveController.ZoneLevel == 4)
            {
                day = 20;
            }
        }
        dayText.text = "DAY " + day + "\nCOMPLETED";
        moneyText.text = "+ " + money;
        this.money = money;
        PlayerProgression.MONEY = PlayerProgression.MONEY;
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
        DOVirtual.DelayedCall(3, Hide);
    }

    public void Hide()
    {
        animator.SetTrigger("Fade Out");
        DOVirtual.DelayedCall(0.163f, () => { gameObject.SetActive(false); });
    }
}
