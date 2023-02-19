using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundFXTable", menuName = "Fate/Sound", order = 1)]
public class SoundFXTable : ScriptableObject
{
    [SerializeField] private List<SoundFX.SoundFXEntity> entities;
    private Dictionary<string, SoundFX.SoundFXEntity> table;

    public SoundFX.SoundFXEntity this[string key]
    {
        get => table[key];
    }

    public void Initialize()
    {
        table = new();
        for (int i = 0; i < entities.Count; i++)
        {
            SoundFX.SoundFXEntity entity = entities[i];
            table.Add(entity.Tag, entity);
        }
        Debug.Log("SoundFX Table initialized");
    }
}
