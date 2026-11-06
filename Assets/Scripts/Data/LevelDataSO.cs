using UnityEngine;

namespace ChickenShooter.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ChickenShooter/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        public int levelIndex = 1;
        public string sceneName;
        public AudioClip backgroundMusic;
        public Sprite parallaxFar;
        public Sprite parallaxNear;
        public WaveDataSO[] rounds;
        public GameObject bossPrefab;
        public string nextLevelSceneName;
    }
}
