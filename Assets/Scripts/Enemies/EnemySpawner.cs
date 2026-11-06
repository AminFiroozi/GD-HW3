using System.Collections.Generic;
using UnityEngine;
using ChickenShooter.Data;

namespace ChickenShooter.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject defaultEnemyPrefab;
        public Transform playerTransform;
        public float topSpawnY = 7f;
        public float formationTopY = 3.5f;
        public float formationRowSpacing = 1f;
        public int columns = 5;

        readonly List<Enemy> alive = new List<Enemy>();

        public System.Action OnWaveCleared;

        public void SpawnWave(WaveDataSO wave)
        {
            alive.Clear();

            var prefab = wave.enemyPrefab != null ? wave.enemyPrefab : defaultEnemyPrefab;
            if (prefab == null) return;

            int count = Mathf.Max(1, wave.enemyCount);
            float spacingX = 1.4f;
            float startX = -(columns - 1) * spacingX * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;

                var formationPos = new Vector3(startX + col * spacingX, formationTopY - row * formationRowSpacing, 0f);
                var spawnPos = new Vector3(formationPos.x, topSpawnY, 0f);

                var instance = Instantiate(prefab, spawnPos, Quaternion.identity);
                var enemy = instance.GetComponent<Enemy>();
                if (enemy == null) enemy = instance.AddComponent<Enemy>();

                enemy.entrySpeed = wave.entrySpeed;
                enemy.diveBombChance = wave.diveBombChance;
                enemy.diveBombCheckInterval = wave.diveBombIntervalSeconds;

                enemy.Init(spawnPos, formationPos, playerTransform);
                enemy.OnDied += HandleEnemyDied;

                alive.Add(enemy);
            }
        }

        void HandleEnemyDied(Enemy enemy)
        {
            alive.Remove(enemy);
            if (alive.Count == 0)
            {
                OnWaveCleared?.Invoke();
            }
        }
    }
}
