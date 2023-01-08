using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{

    [CreateAssetMenu(fileName = "PoolDataTable", menuName = "Fate/PoolDataTable", order = 1)]
    public class PoolDataTable : ScriptableObject
    {
        [SerializeField] private List<ObjectPooler.PoolData> poolData;

        public List<ObjectPooler.PoolData> PoolData { get => poolData; }

    }

}
