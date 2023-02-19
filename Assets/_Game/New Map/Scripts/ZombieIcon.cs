using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class ZombieIcon : MonoBehaviour
{
    [SerializeField] private Image icons = null;

    public void Fade()
    {
        icons.DOFade(0, 0.5f);
    }

    public void InstantFade()
    {
        Color tempColor = icons.color;
        tempColor.a = 0;
        icons.color = tempColor;
    }
}
