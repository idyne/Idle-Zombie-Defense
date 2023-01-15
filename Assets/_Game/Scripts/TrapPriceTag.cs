using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FateGames;

public class TrapPriceTag : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI priceText;

    private void Awake()
    {
        //canvas.worldCamera = Camera.main;
    }
    public bool ButtonEnabled { get => button.interactable; }
    public void DisableButton() => button.interactable = false;

    public void EnableButton() => button.interactable = true;
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
    public void SetPrice(int price) => priceText.text = UIMoney.FormatMoney(price);
}
