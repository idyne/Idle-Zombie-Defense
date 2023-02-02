using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    public void Hide() => container.SetActive(false);
    public void Show() => container.SetActive(true);
}
