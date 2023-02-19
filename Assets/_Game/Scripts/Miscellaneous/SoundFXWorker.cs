using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXWorker : MonoBehaviour
{

    private AudioSource audioSource = null;
    public bool Paused { get; private set; } = false;
    public bool Working { get => audioSource.isPlaying || Paused; }
    private Transform _transform = null;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Settings.OnSoundChange.AddListener((soundOn) => audioSource.mute = !soundOn);
        PauseButton.OnPause.AddListener(() => { if (Working) Pause(); });
        PauseButton.OnResume.AddListener(() => { if (Working) Unpause(); });
    }
    public void Initialize(AudioClip clip, float volume, float pitch, float spatialBlend, bool loop, Vector3 position, bool ignoreListenerPause)
    {
        transform.position = position;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = loop;
        audioSource.ignoreListenerPause = ignoreListenerPause;
    }

    public void Play()
    {
        audioSource.Play();
        Paused = false;
    }
    public void Stop()
    {
        audioSource.Stop();
        Paused = false;
    }
    public void Pause()
    {
        audioSource.Pause();
        Paused = true;
    }
    public void Unpause()
    {
        audioSource.UnPause();
        Paused = false;
    }
}
