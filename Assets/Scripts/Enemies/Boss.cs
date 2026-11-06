using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;

namespace ChickenShooter.Enemies
{
    public enum BossPhase
    {
        Entry,
        ClusterEggs,
        DiveStrike,
        Enraged
    }

    public class Boss : MonoBehaviour, IDamageable
    {
        public int maxHealth = 60;
        public int scoreValue = 500;
        public Vector3 hoverPosition = new Vector3(0f, 4f, 0f);
        public float entrySpeed = 2f;
        public float enrageHealthFraction = 0.3f;

        public GameObject eggPrefab;
        public GameObject minionPrefab;

        public float clusterEggInterval = 2.5f;
        public int clusterEggCount = 6;
        public float diveStrikeInterval = 5f;
        public float diveSpeed = 5f;
        public float enragedFireMultiplier = 2f;
        public float minionSummonInterval = 6f;

        public System.Action<int, int> OnHealthChanged;
        public System.Action OnBossDied;

        int health;
        BossPhase phase = BossPhase.Entry;
        float phaseTimer;
        float minionTimer;
        Vector3 diveTarget;
        bool diving;
        Transform playerTransform;

        public void Init(Transform player)
        {
            health = maxHealth;
            playerTransform = player;
            phase = BossPhase.Entry;
            diveStrikeCountdown = diveStrikeInterval;
            OnHealthChanged?.Invoke(health, maxHealth);
        }

        void Update()
        {
            switch (phase)
            {
                case BossPhase.Entry:
                    TickEntry();
                    break;
                case BossPhase.ClusterEggs:
                    TickClusterEggs();
                    break;
                case BossPhase.DiveStrike:
                    TickDiveStrike();
                    break;
                case BossPhase.Enraged:
                    TickEnraged();
                    break;
            }
        }

        void TickEntry()
        {
            transform.position = Vector3.MoveTowards(transform.position, hoverPosition, entrySpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, hoverPosition) < 0.05f)
            {
                EnterPhase(BossPhase.ClusterEggs);
            }
        }

        void EnterPhase(BossPhase next)
        {
            phase = next;
            phaseTimer = 0f;
        }

        void TickClusterEggs()
        {
            Hover();
            phaseTimer -= Time.deltaTime;
            if (phaseTimer <= 0f)
            {
                FireEggCluster(clusterEggCount);
                phaseTimer = clusterEggInterval;
                diveStrikeCountdown -= clusterEggInterval;
                if (diveStrikeCountdown <= 0f)
                {
                    diveStrikeCountdown = diveStrikeInterval;
                    EnterPhase(BossPhase.DiveStrike);
                }
            }
        }

        float diveStrikeCountdown;

        void TickDiveStrike()
        {
            if (!diving)
            {
                diveTarget = playerTransform != null
                    ? new Vector3(playerTransform.position.x, hoverPosition.y - 2.5f, 0f)
                    : hoverPosition + Vector3.down * 2.5f;
                diving = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, diveTarget) < 0.1f)
            {
                FireEggCluster(2);
                transform.position = Vector3.MoveTowards(transform.position, hoverPosition, diveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, hoverPosition) < 0.1f)
                {
                    diving = false;
                    EnterPhase(BossPhase.ClusterEggs);
                }
            }
        }

        void TickEnraged()
        {
            Hover();
            phaseTimer -= Time.deltaTime;
            minionTimer -= Time.deltaTime;

            if (phaseTimer <= 0f)
            {
                FireEggCluster(clusterEggCount);
                phaseTimer = clusterEggInterval / enragedFireMultiplier;
            }

            if (minionTimer <= 0f)
            {
                SummonMinions();
                minionTimer = minionSummonInterval;
            }
        }

        void Hover()
        {
            var bob = Mathf.Sin(Time.time * 1.5f) * 0.3f;
            transform.position = new Vector3(hoverPosition.x, hoverPosition.y + bob, 0f);
        }

        void FireEggCluster(int count)
        {
            if (eggPrefab == null || PoolManager.Instance == null) return;

            float spread = 60f;
            for (int i = 0; i < count; i++)
            {
                float t = count <= 1 ? 0.5f : (float)i / (count - 1);
                float angle = Mathf.Lerp(-spread / 2f, spread / 2f, t);
                var rotation = Quaternion.Euler(0f, 0f, angle);
                var instance = PoolManager.Instance.Spawn(eggPrefab, transform.position, rotation);
                var bullet = instance.GetComponent<Combat.Bullet>();
                if (bullet != null) bullet.movesUp = false;
            }
        }

        void SummonMinions()
        {
            if (minionPrefab == null) return;

            for (int i = 0; i < 2; i++)
            {
                var spawnPos = transform.position + new Vector3(i == 0 ? -1.5f : 1.5f, -0.5f, 0f);
                var instance = Object.Instantiate(minionPrefab, spawnPos, Quaternion.identity);
                var minion = instance.GetComponent<Enemy>();
                if (minion != null)
                {
                    minion.Init(spawnPos, spawnPos + Vector3.down * 1.5f, playerTransform);
                }
            }
        }

        public void TakeDamage(int amount)
        {
            if (phase == BossPhase.Entry) return;

            health -= amount;
            health = Mathf.Max(0, health);
            OnHealthChanged?.Invoke(health, maxHealth);

            if (health <= 0)
            {
                Die();
                return;
            }

            if (phase != BossPhase.Enraged && health <= maxHealth * enrageHealthFraction)
            {
                EnterPhase(BossPhase.Enraged);
                minionTimer = minionSummonInterval;
            }
        }

        void Die()
        {
            GameManager.Instance?.AddScore(scoreValue);
            OnBossDied?.Invoke();
            Destroy(gameObject);
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
