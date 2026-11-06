using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Player;

namespace ChickenShooter.Combat
{
    public class WeaponGiftBox : MonoBehaviour, IPoolable
    {
        public float fallSpeed = 2.5f;
        public AudioClip pickupSound;

        void Update()
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerShip>();
            if (player == null) return;

            GameManager.Instance?.RaiseWeaponPower();
            AudioManager.Instance?.PlaySfx(pickupSound);
            PoolManager.Instance.Despawn(gameObject);
        }

        public void OnSpawned() { }
        public void OnDespawned() { }
    }
}
