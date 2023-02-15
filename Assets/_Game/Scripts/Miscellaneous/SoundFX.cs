using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;

public class SoundFX : MonoBehaviour, IPooledObject
{
    [SerializeField] private AudioClip clip;
    [Range(-3f, 3f)]
    [SerializeField] private float pitchRangeMin = 1, pitchRangeMax = 1;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1;
    private AudioSource audioSource;
    public float Duration { get => clip ? clip.length : 0; }
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1;
        Settings.OnSoundChange.AddListener((soundOn) => { if (!soundOn) audioSource.Stop(); });
    }

    private void Play()
    {
        if (!Settings.SoundOn)
        {
            Deactivate();
            return;
        }
        audioSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
        audioSource.Play();
        DOVirtual.DelayedCall(Duration, Deactivate);
    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        Play();
    }
}
