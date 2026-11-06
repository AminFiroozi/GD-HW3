using UnityEngine;
using UnityEngine.UI;
using ChickenShooter.Core;

namespace ChickenShooter.UI
{
    public class HudUI : MonoBehaviour
    {
        public Text roundText;
        public Text scoreText;
        public Text livesText;
        public Text missilesText;
        public Image foodBarFill;

        void OnEnable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            gm.OnRoundChanged += HandleRoundChanged;
            gm.OnScoreChanged += HandleScoreChanged;
            gm.OnLivesChanged += HandleLivesChanged;
            gm.OnSecondaryMissilesChanged += HandleMissilesChanged;
            gm.OnFoodProgressChanged += HandleFoodChanged;

            HandleRoundChanged(gm.LevelIndex, gm.RoundIndex);
            HandleScoreChanged(gm.Score);
            HandleLivesChanged(gm.Lives);
            HandleMissilesChanged(gm.SecondaryMissiles);
            HandleFoodChanged(gm.FoodProgress);
        }

        void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            gm.OnRoundChanged -= HandleRoundChanged;
            gm.OnScoreChanged -= HandleScoreChanged;
            gm.OnLivesChanged -= HandleLivesChanged;
            gm.OnSecondaryMissilesChanged -= HandleMissilesChanged;
            gm.OnFoodProgressChanged -= HandleFoodChanged;
        }

        void HandleRoundChanged(int level, int round)
        {
            if (roundText != null) roundText.text = $"Level {level} - Round {round}";
        }

        void HandleScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        void HandleLivesChanged(int lives)
        {
            if (livesText != null) livesText.text = $"Lives: {lives}";
        }

        void HandleMissilesChanged(int missiles)
        {
            if (missilesText != null) missilesText.text = $"Missiles: {missiles}";
        }

        void HandleFoodChanged(float progress)
        {
            if (foodBarFill != null) foodBarFill.fillAmount = Mathf.Clamp01(progress);
        }
    }
}
