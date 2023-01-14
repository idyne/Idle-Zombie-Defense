using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FateGames;

public class TrapPriceTag : MonoBehaviour
{
    [SerializeField] private GameObject enabledButtonImage, disabledButtonImage;
    [SerializeField] private TextMeshProUGUI priceText;
    public bool ButtonEnabled { get; private set; } = true;
    public void DisableButton()
    {
        enabledButtonImage.SetActive(false);
        disabledButtonImage.SetActive(true);
        ButtonEnabled = false;
    }

    public void EnableButton()
    {
        enabledButtonImage.SetActive(true);
        disabledButtonImage.SetActive(false);
        ButtonEnabled = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetPrice(int price)
    {
        priceText.text = UIMoney.FormatMoney(price);
    }
}
