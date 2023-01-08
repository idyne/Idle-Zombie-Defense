using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectTable", menuName = "Fate/GameObjectTable", order = 1)]
public class GameObjectTable : ScriptableObject
{
    [SerializeField] private List<Entity> entityList = new();
    private Dictionary<string, GameObject> entities = new();
    private bool initialized { get => entities.Count > 0; }
    public Dictionary<string, GameObject> Entities
    {
        get
        {
            if (!initialized) Initialize();
            return entities;
        }
    }


    private void Initialize()
    {
        if (initialized || entityList.Count == 0) return;
        foreach (Entity entity in entityList)
            entities.Add(entity.Tag, entity.GameObject);
    }

    [Serializable]
    private class Entity
    {
        [SerializeField] private string tag;
        [SerializeField] private GameObject gameObject;

        public string Tag { get => tag; }
        public GameObject GameObject { get => gameObject; }
    }
}
