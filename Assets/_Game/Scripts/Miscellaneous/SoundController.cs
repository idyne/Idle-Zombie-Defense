using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public void Play(string soundTag)
    {
        SoundFX.PlaySound(soundTag);
    }
}
