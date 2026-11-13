using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.VFX;

namespace ChickenShooter.Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public int maxHealth = 1;
        public int scoreValue = 10;
        public float entrySpeed = 4f;
        public float eggDropIntervalMin = 2f;
        public float eggDropIntervalMax = 5f;
        public float diveBombChance = 0.1f;
        public float diveBombCheckInterval = 3f;
        public float diveBombSpeed = 6f;
        public GameObject eggPrefab;
        public GameObject foodPrefab;
        public GameObject giftBoxPrefab;
        public float foodDropChance = 0.3f;
        public float giftBoxDropChance = 0.12f;
        public AudioClip eggDropSound;
        public AudioClip deathSound;
        public GameObject explosionPrefab;

        public System.Action<Enemy> OnDied;

        int health;
        Vector3 formationTarget;
        bool inFormation;
        bool diving;
        float eggTimer;
        float diveCheckTimer;
        Transform playerTransform;
        Vector3 diveDirection;

        const float DiveExitY = 7f;

        public void Init(Vector3 spawnPos, Vector3 formationPos, Transform player)
        {
            transform.position = spawnPos;
            formationTarget = formationPos;
            playerTransform = player;
            health = maxHealth;
            ResetEggTimer();
            diveCheckTimer = diveBombCheckInterval;
        }

        void Update()
        {
            if (!inFormation)
            {
                MoveToFormation();
                return;
            }

            if (diving)
            {
                DiveTowardsPlayer();
                return;
            }

            TickEggTimer();
            TickDiveCheck();
        }

        void MoveToFormation()
        {
            transform.position = Vector3.MoveTowards(transform.position, formationTarget, entrySpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, formationTarget) < 0.05f)
            {
                inFormation = true;
            }
        }

        void TickEggTimer()
        {
            eggTimer -= Time.deltaTime;
            if (eggTimer <= 0f)
            {
                DropEgg();
                ResetEggTimer();
            }
        }

        void ResetEggTimer()
        {
            eggTimer = Random.Range(eggDropIntervalMin, eggDropIntervalMax);
        }

        void DropEgg()
        {
            if (eggPrefab == null || PoolManager.Instance == null) return;
            var instance = PoolManager.Instance.Spawn(eggPrefab, transform.position, Quaternion.identity);
            var bullet = instance.GetComponent<Combat.Bullet>();
            if (bullet != null) bullet.movesUp = false;
            AudioManager.Instance?.PlaySfx(eggDropSound);
        }

        void TickDiveCheck()
        {
            diveCheckTimer -= Time.deltaTime;
            if (diveCheckTimer <= 0f)
            {
                diveCheckTimer = diveBombCheckInterval;
                if (Random.value < diveBombChance)
                {
                    StartDive();
                }
            }
        }

        void StartDive()
        {
            diving = true;
            diveDirection = Vector3.down;

            if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
            {
                var toPlayer = playerTransform.position - transform.position;
                if (toPlayer.y < -0.5f) diveDirection = toPlayer.normalized;
            }
        }

        void DiveTowardsPlayer()
        {
            transform.position += diveDirection * diveBombSpeed * Time.deltaTime;

            if (transform.position.y < -DiveExitY)
            {
                transform.position = new Vector3(formationTarget.x, DiveExitY, transform.position.z);
                diving = false;
                inFormation = false;
            }
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            GetComponent<DamageFlash>()?.Flash();

            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            GameManager.Instance?.AddScore(scoreValue);
            AudioManager.Instance?.PlaySfx(deathSound);
            MaybeDrop(foodPrefab, foodDropChance);
            MaybeDrop(giftBoxPrefab, giftBoxDropChance);

            if (explosionPrefab != null && PoolManager.Instance != null)
            {
                PoolManager.Instance.Spawn(explosionPrefab, transform.position, Quaternion.identity);
            }
            ScreenShake.Instance?.Shake(0.15f);

            OnDied?.Invoke(this);
            Destroy(gameObject);
        }

        void MaybeDrop(GameObject prefab, float chance)
        {
            if (prefab == null || PoolManager.Instance == null) return;
            if (Random.value <= chance)
            {
                PoolManager.Instance.Spawn(prefab, transform.position, Quaternion.identity);
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
