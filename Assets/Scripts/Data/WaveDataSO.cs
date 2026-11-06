using UnityEngine;

namespace ChickenShooter.Data
{
    public enum WaveType
    {
        GridFormation,
        VFormation,
        AsteroidField
    }

    [CreateAssetMenu(fileName = "WaveData", menuName = "ChickenShooter/Wave Data")]
    public class WaveDataSO : ScriptableObject
    {
        public WaveType waveType;
        public GameObject enemyPrefab;
        public int enemyCount = 10;
        public float enemySpeedMultiplier = 1f;
        public float enemyFireRateMultiplier = 1f;
        public float entrySpeed = 4f;
        public float diveBombChance = 0.1f;
        public float diveBombIntervalSeconds = 3f;
    }
}
