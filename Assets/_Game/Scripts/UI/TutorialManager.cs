using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private GameObject UIBackground;

    public void Show()
    {
        UIBackground.SetActive(true);
        anim.Play("Scale");
    }

    // Start Wave butonunun Button componentinin onClick eventine ba�l�
    public void HideBackground()
    {
        UIBackground.SetActive(false);
    }
}
