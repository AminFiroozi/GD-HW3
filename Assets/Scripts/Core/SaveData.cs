using System;

namespace ChickenShooter.Core
{
    [Serializable]
    public class SaveData
    {
        public int levelIndex = 1;
        public int roundIndex = 1;
        public int score;
        public int lives = 3;
        public int secondaryMissiles = 2;
        public int weaponPowerLevel = 1;
        public float foodProgress;
    }
}
