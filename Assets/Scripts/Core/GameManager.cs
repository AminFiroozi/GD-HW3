using System;
using UnityEngine;
using ChickenShooter.Data;

namespace ChickenShooter.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int LevelIndex { get; private set; } = 1;
        public int RoundIndex { get; private set; } = 1;
        public int Score { get; private set; }
        public int Lives { get; private set; } = 3;
        public int SecondaryMissiles { get; private set; } = 2;
        public int WeaponPowerLevel { get; private set; } = 1;
        public float FoodProgress { get; private set; }

        public const int MinWeaponPowerLevel = 1;
        public const int MaxWeaponPowerLevel = 3;
        public const int MaxLives = 3;
        public const float FoodProgressToMissile = 1f;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnLivesChanged;
        public event Action<int> OnWeaponPowerChanged;
        public event Action<int> OnSecondaryMissilesChanged;
        public event Action<float> OnFoodProgressChanged;
        public event Action<int, int> OnRoundChanged;
        public event Action OnGameOver;
        public event Action OnVictory;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public bool LoseLife()
        {
            Lives = Mathf.Max(0, Lives - 1);
            OnLivesChanged?.Invoke(Lives);
            DropWeaponPower();
            return Lives <= 0;
        }

        public void DropWeaponPower()
        {
            SetWeaponPowerLevel(WeaponPowerLevel - 1);
        }

        public void RaiseWeaponPower()
        {
            SetWeaponPowerLevel(WeaponPowerLevel + 1);
        }

        void SetWeaponPowerLevel(int level)
        {
            WeaponPowerLevel = Mathf.Clamp(level, MinWeaponPowerLevel, MaxWeaponPowerLevel);
            OnWeaponPowerChanged?.Invoke(WeaponPowerLevel);
        }

        public bool TryUseSecondaryMissile()
        {
            if (SecondaryMissiles <= 0) return false;
            SecondaryMissiles--;
            OnSecondaryMissilesChanged?.Invoke(SecondaryMissiles);
            return true;
        }

        public void AddFoodProgress(float amount)
        {
            FoodProgress += amount;
            if (FoodProgress >= FoodProgressToMissile)
            {
                FoodProgress -= FoodProgressToMissile;
                SecondaryMissiles++;
                OnSecondaryMissilesChanged?.Invoke(SecondaryMissiles);
            }
            OnFoodProgressChanged?.Invoke(FoodProgress);
        }

        public void SetRound(int levelIndex, int roundIndex)
        {
            LevelIndex = levelIndex;
            RoundIndex = roundIndex;
            OnRoundChanged?.Invoke(LevelIndex, RoundIndex);
        }

        public void ResetForNewGame()
        {
            LevelIndex = 1;
            RoundIndex = 1;
            Score = 0;
            Lives = MaxLives;
            SecondaryMissiles = 2;
            WeaponPowerLevel = MinWeaponPowerLevel;
            FoodProgress = 0f;
        }

        public void ResetForRetry()
        {
            RoundIndex = 1;
            Lives = MaxLives;
            WeaponPowerLevel = MinWeaponPowerLevel;
        }

        public void TriggerGameOver()
        {
            RoundIndex = 1;
            SaveSystem.Save(ToSaveData());
            OnGameOver?.Invoke();
        }

        public void TriggerVictory()
        {
            SaveSystem.Delete();
            OnVictory?.Invoke();
        }

        public void LoadFromSaveData(SaveData data)
        {
            LevelIndex = data.levelIndex;
            RoundIndex = data.roundIndex;
            Score = data.score;
            Lives = data.lives;
            SecondaryMissiles = data.secondaryMissiles;
            WeaponPowerLevel = data.weaponPowerLevel;
            FoodProgress = data.foodProgress;
        }

        public SaveData ToSaveData()
        {
            return new SaveData
            {
                levelIndex = LevelIndex,
                roundIndex = RoundIndex,
                score = Score,
                lives = Lives,
                secondaryMissiles = SecondaryMissiles,
                weaponPowerLevel = WeaponPowerLevel,
                foodProgress = FoodProgress
            };
        }
    }
}
