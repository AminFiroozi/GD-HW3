using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Player;

namespace ChickenShooter.Combat
{
    public class FoodDrop : MonoBehaviour, IPoolable
    {
        public float fallSpeed = 2.5f;
        public int scoreValue = 5;
        public float progressAmount = 0.1f;
        public AudioClip pickupSound;

        void Update()
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerShip>();
            if (player == null) return;

            GameManager.Instance?.AddScore(scoreValue);
            GameManager.Instance?.AddFoodProgress(progressAmount);
            AudioManager.Instance?.PlaySfx(pickupSound);
            PoolManager.Instance.Despawn(gameObject);
        }

        public void OnSpawned() { }
        public void OnDespawned() { }
    }
}
