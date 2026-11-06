using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;

namespace ChickenShooter.Combat
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        public float speed = 14f;
        public int damage = 1;
        public bool movesUp = true;
        public bool insta = false;

        void Update()
        {
            var baseDirection = movesUp ? Vector3.up : Vector3.down;
            var direction = transform.rotation * baseDirection;
            transform.position += direction * speed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(insta ? int.MaxValue : damage);
                PoolManager.Instance.Despawn(gameObject);
                return;
            }

            var player = other.GetComponent<ChickenShooter.Player.PlayerShip>();
            if (player != null && !movesUp)
            {
                player.TakeDamage();
                PoolManager.Instance.Despawn(gameObject);
            }
        }

        public void OnSpawned() { }

        public void OnDespawned() { }
    }
}
