using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using TMPro;

public class UILevitatingText : MonoBehaviour, IPooledObject
{
    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        Transform = transform;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public Transform Transform { get; private set; }
    public void OnObjectSpawn()
    {
        Levitate();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    private void Levitate()
    {
        float currentHeight = Transform.position.y;
        float duration = 1f;
        Transform.DOMoveY(currentHeight + 2, duration).SetEase(Ease.OutQuint);
        canvasGroup.alpha = 1;
        DOTween.To((val) =>
        {
            canvasGroup.alpha = val;
        }, 1, 0, duration).SetEase(Ease.OutQuint).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }


}
