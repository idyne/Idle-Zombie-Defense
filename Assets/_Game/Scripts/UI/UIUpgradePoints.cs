using DG.Tweening;
using FateGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUpgradePoints : MonoBehaviour
{
    private Transform _transform = null;
    private bool animating = false;

    public Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return _transform;
        }
    }
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private RectTransform moneyImage;

    private void Awake()
    {
        PlayerProgression.OnUpgradePointChanged.AddListener(SetMoney);
    }

    private void SetMoney(int money, int change = 0)
    {
        moneyText.text = UIMoney.FormatMoney(PlayerProgression.UPGRADE_POINT);
        if (animating || change == 0) return;
        animating = true;
        Transform.DOScale(1.1f, 0.05f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { animating = false; });
    }


}
