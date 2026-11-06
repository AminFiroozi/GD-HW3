using UnityEngine;

namespace ChickenShooter.Core.Pooling
{
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool Pool { get; set; }

        public void ReleaseToPool()
        {
            Pool?.Release(gameObject);
        }
    }
}
