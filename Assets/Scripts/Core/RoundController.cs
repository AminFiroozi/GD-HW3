using UnityEngine;
using ChickenShooter.Data;
using ChickenShooter.Enemies;

namespace ChickenShooter.Core
{
    public class RoundController : MonoBehaviour
    {
        public LevelDataSO levelData;
        public EnemySpawner spawner;

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
            int nextRound = (GameManager.Instance != null ? GameManager.Instance.RoundIndex : 1) + 1;

            if (nextRound > levelData.rounds.Length)
            {
                return;
            }

            GameManager.Instance.SetRound(levelData.levelIndex, nextRound);
            SaveSystem.Save(GameManager.Instance.ToSaveData());
            SpawnCurrentRound();
        }
    }
}
