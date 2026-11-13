using System.Collections.Generic;
using UnityEngine;

namespace ChickenShooter.Core.Pooling
{
    public class ObjectPool
    {
        readonly GameObject prefab;
        readonly Transform parent;
        readonly Stack<GameObject> inactive = new Stack<GameObject>();

        public ObjectPool(GameObject prefab, Transform parent, int prewarmCount = 0)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < prewarmCount; i++)
            {
                var instance = CreateInstance();
                instance.SetActive(false);
                inactive.Push(instance);
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject instance = inactive.Count > 0 ? inactive.Pop() : CreateInstance();

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);

            foreach (var poolable in instance.GetComponents<IPoolable>())
            {
                poolable.OnSpawned();
            }

            return instance;
        }

        public void Release(GameObject instance)
        {
            foreach (var poolable in instance.GetComponents<IPoolable>())
            {
                poolable.OnDespawned();
            }

            instance.SetActive(false);
            inactive.Push(instance);
        }

        GameObject CreateInstance()
        {
            var instance = Object.Instantiate(prefab, parent);
            var releaser = instance.GetComponent<PooledObject>();
            if (releaser == null) releaser = instance.AddComponent<PooledObject>();
            releaser.Pool = this;
            return instance;
        }
    }
}
