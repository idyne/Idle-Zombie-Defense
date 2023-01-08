using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FateGames
{
    public static class PrefabManager
    {
        private static GameObjectTable prefabTable = null;
        public static Dictionary<string, GameObject> Prefabs
        {
            get
            {
                if (prefabTable == null)
                    prefabTable = Resources.Load<GameObjectTable>("Fate Games/ScriptableObjects/GameObjectTables/Prefab Table");
                return prefabTable.Entities;
            }
        }
    }

}