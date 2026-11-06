using System.Collections.Generic;
using UnityEngine;
using ChickenShooter.Data;

namespace ChickenShooter.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject defaultEnemyPrefab;
        public GameObject defaultMeteorPrefab;
        public Transform playerTransform;
        public float topSpawnY = 7f;
        public float formationTopY = 3.5f;
        public float formationRowSpacing = 1f;
        public int columns = 5;

        readonly List<Enemy> aliveEnemies = new List<Enemy>();
        readonly List<Meteor> aliveMeteors = new List<Meteor>();

        public System.Action OnWaveCleared;

        public void SpawnWave(WaveDataSO wave)
        {
            aliveEnemies.Clear();
            aliveMeteors.Clear();

            if (wave.waveType == WaveType.AsteroidField)
            {
                SpawnMeteorField(wave);
            }
            else
            {
                SpawnFormation(wave);
            }
        }

        void SpawnFormation(WaveDataSO wave)
        {
            var prefab = wave.enemyPrefab != null ? wave.enemyPrefab : defaultEnemyPrefab;
            if (prefab == null) return;

            int count = Mathf.Max(1, wave.enemyCount);
            float spacingX = 1.4f;
            int centerCol = (columns - 1) / 2;
            float startX = -(columns - 1) * spacingX * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;

                float y = formationTopY - row * formationRowSpacing;
                if (wave.waveType == WaveType.VFormation)
                {
                    y -= Mathf.Abs(col - centerCol) * 0.6f;
                }

                var formationPos = new Vector3(startX + col * spacingX, y, 0f);
                var spawnPos = new Vector3(formationPos.x, topSpawnY, 0f);

                var instance = Instantiate(prefab, spawnPos, Quaternion.identity);
                var enemy = instance.GetComponent<Enemy>();
                if (enemy == null) enemy = instance.AddComponent<Enemy>();

                enemy.entrySpeed = wave.entrySpeed;
                enemy.diveBombChance = wave.diveBombChance;
                enemy.diveBombCheckInterval = wave.diveBombIntervalSeconds;

                enemy.Init(spawnPos, formationPos, playerTransform);
                enemy.OnDied += HandleEnemyDied;

                aliveEnemies.Add(enemy);
            }
        }

        void SpawnMeteorField(WaveDataSO wave)
        {
            var prefab = wave.enemyPrefab != null ? wave.enemyPrefab : defaultMeteorPrefab;
            if (prefab == null) return;

            int count = Mathf.Max(1, wave.enemyCount);

            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-4f, 4f);
                float y = topSpawnY + Random.Range(0f, 6f);
                var spawnPos = new Vector3(x, y, 0f);

                var instance = Instantiate(prefab, spawnPos, Quaternion.identity);
                var meteor = instance.GetComponent<Meteor>();
                if (meteor == null) meteor = instance.AddComponent<Meteor>();

                meteor.Init(2, wave.entrySpeed * wave.enemySpeedMultiplier, Random.Range(-0.5f, 0.5f));
                meteor.OnDestroyed += HandleMeteorDestroyed;

                aliveMeteors.Add(meteor);
            }
        }

        void HandleEnemyDied(Enemy enemy)
        {
            aliveEnemies.Remove(enemy);
            CheckWaveCleared();
        }

        void HandleMeteorDestroyed(Meteor meteor)
        {
            aliveMeteors.Remove(meteor);
            CheckWaveCleared();
        }

        void CheckWaveCleared()
        {
            if (aliveEnemies.Count == 0 && aliveMeteors.Count == 0)
            {
                OnWaveCleared?.Invoke();
            }
        }
    }
}
