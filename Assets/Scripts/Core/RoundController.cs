using UnityEngine;
using UnityEngine.SceneManagement;
using ChickenShooter.Data;
using ChickenShooter.Enemies;
using ChickenShooter.UI;

namespace ChickenShooter.Core
{
    public class RoundController : MonoBehaviour
    {
        public LevelDataSO levelData;
        public EnemySpawner spawner;
        public Transform playerTransform;
        public BossHealthBarUI bossHealthBar;
        public float sceneTransitionDelay = 2f;

        Boss activeBoss;

        void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.LevelIndex != levelData.levelIndex)
            {
                GameManager.Instance.SetRound(levelData.levelIndex, 1);
            }

            spawner.OnWaveCleared += HandleWaveCleared;
            SpawnCurrentRound();
        }

        void SpawnCurrentRound()
        {
            int roundIndex = GameManager.Instance != null ? GameManager.Instance.RoundIndex : 1;
            int arrayIndex = roundIndex - 1;

            if (arrayIndex < 0 || arrayIndex >= levelData.rounds.Length)
            {
                return;
            }

            spawner.SpawnWave(levelData.rounds[arrayIndex]);
        }

        void HandleWaveCleared()
        {
            int currentRound = GameManager.Instance != null ? GameManager.Instance.RoundIndex : 1;
            int nextRound = currentRound + 1;

            if (currentRound >= levelData.rounds.Length)
            {
                SpawnBoss();
                return;
            }

            GameManager.Instance.SetRound(levelData.levelIndex, nextRound);
            SaveSystem.Save(GameManager.Instance.ToSaveData());
            SpawnCurrentRound();
        }

        void SpawnBoss()
        {
            if (levelData.bossPrefab == null) return;

            var instance = Instantiate(levelData.bossPrefab, new Vector3(0f, 8f, 0f), Quaternion.identity);
            activeBoss = instance.GetComponent<Boss>();
            if (activeBoss == null) return;

            activeBoss.Init(playerTransform);
            activeBoss.OnBossDied += HandleBossDefeated;

            if (bossHealthBar != null)
            {
                bossHealthBar.Bind(activeBoss);
            }
        }

        void HandleBossDefeated()
        {
            if (!string.IsNullOrEmpty(levelData.nextLevelSceneName))
            {
                GameManager.Instance.SetRound(levelData.levelIndex + 1, 1);
                SaveSystem.Save(GameManager.Instance.ToSaveData());
                Invoke(nameof(LoadNextLevel), sceneTransitionDelay);
            }
            else
            {
                GameManager.Instance.TriggerVictory();
            }
        }

        void LoadNextLevel()
        {
            SceneManager.LoadScene(levelData.nextLevelSceneName);
        }
    }
}
