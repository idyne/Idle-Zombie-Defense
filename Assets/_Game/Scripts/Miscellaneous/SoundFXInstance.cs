using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXInstance
{
    private readonly GameObject gameObject;
    private readonly AudioSource audioSource;
    private bool loop;
    private Tween deactivationTween = null;
    public float Duration { get => audioSource.clip.length; }
    public float TimeLeft { get => Duration - audioSource.time; }

    public SoundFXInstance(string tag, AudioClip clip, float spatialBlend, bool loop, float volume, float pitchRangeMin = 1, float pitchRangeMax = 1)
    {
        gameObject = new();
        this.loop = loop;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = loop;
        audioSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
        Settings.OnSoundChange.AddListener((soundOn) => { audioSource.mute = !soundOn; });
    }

    public void Play()
    {
        audioSource.Play();
        if (!loop) deactivationTween?.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
        audioSource.time = 0;
        deactivationTween?.Kill();
        deactivationTween = DOVirtual.DelayedCall(TimeLeft, Deactivate, false).Pause();
    }

    public void Pause()
    {
        audioSource.Pause();
        deactivationTween?.Pause();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        deactivationTween?.Kill();
        deactivationTween = null;
    }
}
