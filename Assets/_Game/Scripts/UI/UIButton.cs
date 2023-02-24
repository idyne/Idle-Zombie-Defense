using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private Image disabledImage;
    public bool Hidden { get => root.activeSelf; }

    public bool Active { get => button.interactable; }

    public void SetText(string text) => this.text.text = text;
    public void Activate()
    {
        disabledImage?.gameObject.SetActive(false);
        button.interactable = true;
    }
    public void Deactivate()
    {
        disabledImage?.gameObject.SetActive(true);
        button.interactable = false;
    }
    public void Hide() => root.SetActive(false);
    public void Show() => root.SetActive(true);
}
