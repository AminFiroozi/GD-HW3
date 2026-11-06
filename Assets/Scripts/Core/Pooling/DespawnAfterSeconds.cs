using UnityEngine;

namespace ChickenShooter.Core.Pooling
{
    public class DespawnAfterSeconds : MonoBehaviour
    {
        public float seconds = 3f;

        float timer;

        void OnEnable()
        {
            timer = seconds;
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
        }
    }
}
