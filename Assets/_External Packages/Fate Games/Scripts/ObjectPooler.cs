using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public static class ObjectPooler
    {
        private static List<PoolData> pools = new();
        private struct Pool
        {
            private Queue<GameObject> instances;
            private bool canActiveObjectsBeDequeued;

            public Pool(Queue<GameObject> instances, bool canActiveObjectsBeDequeued)
            {
                this.instances = instances;
                this.canActiveObjectsBeDequeued = canActiveObjectsBeDequeued;
            }

            public Queue<GameObject> Instances { get => instances; }
            public bool CanActiveObjectsBeDequeued { get => canActiveObjectsBeDequeued; }
        }

        private static Dictionary<string, Pool> poolDictionary;
        public static void CreatePools()
        {
            if (pools.Count == 0)
            {
                List<PoolData> essentialPools = Resources.Load<PoolDataTable>("Fate Games/ScriptableObjects/PoolDataTables/Essential Pool Data").PoolData;
                List<PoolData> gamePools = Resources.Load<PoolDataTable>("Fate Games/ScriptableObjects/PoolDataTables/Game Pool Data").PoolData;
                pools.AddRange(essentialPools);
                pools.AddRange(gamePools);
            }
            if (pools.Count == 0) return;
            poolDictionary = new Dictionary<string, Pool>();
            Transform container = new GameObject("Pool Container").transform;
            container.position = Vector3.up * 100 + Vector3.right * 100;
            foreach (PoolData poolData in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                Transform poolObj = new GameObject(poolData.tag + " Pool").transform;
                poolData.container = poolObj;
                poolObj.parent = container;
                poolObj.transform.localPosition = Vector3.zero;
                for (int i = 0; i < poolData.size; i++)
                {
                    GameObject obj = Object.Instantiate(poolData.prefab, poolObj);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                Pool pool = new Pool(objectPool, poolData.canActiveObjectsBeDequeued);
                if (!poolDictionary.ContainsKey(poolData.tag))
                    poolDictionary.Add(poolData.tag, pool);
            }
        }
        private static void ExtendPool(string tag)
        {
            PoolData poolData = pools.Find((pool) => tag == pool.tag);
            Pool pool = poolDictionary[tag];
            for (int i = 0; i < poolData.size; i++)
            {
                GameObject obj = Object.Instantiate(poolData.prefab, poolData.container);
                obj.SetActive(false);
                pool.Instances.Enqueue(obj);
            }
            poolData.size *= 2;
        }

        public static GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogError("Pool with tag " + tag + " doesn't exist.");
                return null;
            }
            GameObject objectToSpawn;
            Pool pool = poolDictionary[tag];
            objectToSpawn = pool.Instances.Dequeue();
            pool.Instances.Enqueue(objectToSpawn);
            if (!pool.CanActiveObjectsBeDequeued)
            {

                int i = pool.Instances.Count;
                while (i-- > 0 && objectToSpawn.activeSelf)
                {
                    objectToSpawn = pool.Instances.Dequeue();
                    pool.Instances.Enqueue(objectToSpawn);
                }
                if (objectToSpawn.activeSelf)
                {
                    ExtendPool(tag);
                    Debug.Log(tag + " pool is extended.");
                    return SpawnFromPool(tag, position, rotation);
                }
            }
            DOTween.Kill(objectToSpawn);
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);
            objectToSpawn.SetActive(true);
            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            if (pooledObj != null)
                pooledObj.OnObjectSpawn();
            return objectToSpawn;
        }
        [System.Serializable]
        public class PoolData
        {
            public string tag;
            public GameObject prefab;
            public int size;
            public bool canActiveObjectsBeDequeued = false;
            [HideInInspector] public Transform container;
        }
    }
}