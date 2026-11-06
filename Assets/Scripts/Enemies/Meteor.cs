using UnityEngine;
using ChickenShooter.Core;

namespace ChickenShooter.Enemies
{
    public class Meteor : MonoBehaviour, IDamageable
    {
        public int maxHealth = 2;
        public int scoreValue = 5;
        public float driftSpeed = 2f;
        public float driftSidewaysSpeed = 0f;

        public System.Action<Meteor> OnDestroyed;

        int health;

        public void Init(int startingHealth, float speed, float sideways)
        {
            health = startingHealth;
            maxHealth = startingHealth;
            driftSpeed = speed;
            driftSidewaysSpeed = sideways;
        }

        void Update()
        {
            transform.position += new Vector3(driftSidewaysSpeed, -driftSpeed, 0f) * Time.deltaTime;

            if (transform.position.y < -8f)
            {
                OnDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                GameManager.Instance?.AddScore(scoreValue);
                OnDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<Player.PlayerShip>();
            if (player != null)
            {
                player.TakeDamage();
            }
        }
    }
}
