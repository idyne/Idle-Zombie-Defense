using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using System.Linq;
public static class SoundFX
{
    private static List<SoundFXWorker> workers = new();
    private static SoundFXTable _table = null;
    private static SoundFXTable table
    {
        get
        {
            if (_table == null)
            {

                _table = Resources.Load<SoundFXTable>("Fate Games/ScriptableObjects/SoundFXTables/SoundFXTable");
                _table.Initialize();
            }
            return _table;
        }
    }

    public static void StopWorkers()
    {
        foreach (SoundFXWorker worker in workers)
            worker.Stop();
    }

    private static SoundFXWorker GetAvailableWorker()
    {
        SoundFXWorker worker;
        void GetWorker() { worker = workers.Find((worker) => !worker.Working); }
        GetWorker();
        while (worker == null)
        {
            DoubleWorkers();
            GetWorker();
        }
        return worker;
    }

    private static void DoubleWorkers()
    {
        int number = workers.Count > 0 ? workers.Count : 1;
        for (int i = 0; i < number; i++)
            AddWorker();
    }

    private static void AddWorker()
    {
        SoundFXWorker worker = new GameObject("SoundFX Worker").AddComponent<SoundFXWorker>();
        workers.Add(worker);
    }

    public static SoundFXWorker PlaySound(string soundTag, bool ignoreListenerPause = false)
    {
        return PlaySound(soundTag, Vector3.zero, ignoreListenerPause);
    }

    public static SoundFXWorker PlaySound(string soundTag, Vector3 position, bool ignoreListenerPause = false)
    {
        if (soundTag == "") return null;
        SoundFXWorker worker = GetAvailableWorker();
        SoundFXEntity entity = table[soundTag];
        float pitch = Random.Range(entity.PitchRangeMin, entity.PitchRangeMax);
        worker.Initialize(entity.Clip, entity.Volume, pitch, entity.SpatialBlend, entity.Loop, position, ignoreListenerPause);
        worker.Play();
        return worker;
    }

    [System.Serializable]
    public class SoundFXEntity
    {
        [SerializeField] private string tag;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop = false;
        [Range(0f, 1f)]
        [SerializeField] private float spatialBlend = 1;
        [Range(-3f, 3f)]
        [SerializeField] private float pitchRangeMin = 1, pitchRangeMax = 1;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1;

        public AudioClip Clip { get => clip; }
        public bool Loop { get => loop; }
        public float SpatialBlend { get => spatialBlend; }
        public float PitchRangeMin { get => pitchRangeMin; }
        public float PitchRangeMax { get => pitchRangeMax; }
        public float Volume { get => volume; }
        public string Tag { get => tag; }
    }
}
