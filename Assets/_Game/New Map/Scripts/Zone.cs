using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    [SerializeField] private Transform root = null;
    [SerializeField] private Transform cleanSign = null;
    [SerializeField] private Transform cleaningWave = null;

    public void Hop()
    {
        root.DOKill(true);
        root.DOScale(Vector3.one * 1.5f, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void Wave()
    {
        cleaningWave.DOScale(Vector3.one * 5, 1f);
        cleaningWave.GetComponent<Image>().DOFade(0, 1f);
    }

    public void CleanSign()
    {
        cleanSign.DOScale(Vector3.one, 0.5f);
        cleanSign.GetComponent<Image>().DOFade(1, 0.5f);
        cleanSign.GetChild(0).GetComponent<Image>().DOFade(1, 0.5f);
    }

    public void InstantCleanSign()
    {
        cleanSign.localScale = Vector3.one;
        Color tempColor = cleanSign.GetComponent<Image>().color;
        tempColor.a = 1;
        cleanSign.GetComponent<Image>().color = tempColor;
        cleanSign.GetChild(0).GetComponent<Image>().color = tempColor;
    }
}
