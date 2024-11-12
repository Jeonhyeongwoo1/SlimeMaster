using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace SlimeMaster.InGame.Manager
{
    public class PoolManager
    {
        private Dictionary<string, IObjectPool<GameObject>> _poolDict = new ();
        
        public GameObject GetObject(string name, GameObject prefab = null)
        {
            if (_poolDict.TryGetValue(name, out var objectPool))
            {
                return objectPool.Get();
            }
            
            const int poolCount = 5;
            var pool = CreatePool(prefab, poolCount);
            _poolDict.TryAdd(name, pool);

            return pool.Get();
        }

        public void ReleaseObject(string name, GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (_poolDict.TryGetValue(name, out var objectPool))
            {
                if (obj.activeSelf)
                {
                    objectPool.Release(obj);
                    obj.SetActive(false);
                }
            }
        }
        
        private IObjectPool<GameObject> CreatePool(GameObject prefab, int poolCount)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Object.Instantiate(prefab);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnDestroy: Object.Destroy,
                maxSize: poolCount
            );
        }
    }
}