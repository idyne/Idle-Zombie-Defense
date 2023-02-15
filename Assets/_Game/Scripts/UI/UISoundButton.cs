using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundButton : MonoBehaviour
{
    [SerializeField] private GameObject soundOnImage, soundOffImage;
    public void ToggleSound()
    {
        Settings.SoundOn = !Settings.SoundOn;
        soundOnImage.SetActive(Settings.SoundOn);
        soundOffImage.SetActive(!Settings.SoundOn);
    }
}
