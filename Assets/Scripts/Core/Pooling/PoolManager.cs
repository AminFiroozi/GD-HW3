using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChickenShooter.Core.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        readonly Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        void OnDestroy()
        {
            if (Instance == this) SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (child.activeSelf) Despawn(child);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GetOrCreatePool(prefab).Get(position, rotation);
        }

        public void Despawn(GameObject instance)
        {
            var pooled = instance.GetComponent<PooledObject>();
            if (pooled != null)
            {
                pooled.ReleaseToPool();
            }
            else
            {
                Destroy(instance);
            }
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (pools.ContainsKey(prefab)) return;
            pools[prefab] = new ObjectPool(prefab, transform, count);
        }

        ObjectPool GetOrCreatePool(GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool(prefab, transform);
                pools[prefab] = pool;
            }

            return pool;
        }
    }
}
