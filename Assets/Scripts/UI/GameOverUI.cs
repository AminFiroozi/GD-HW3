using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChickenShooter.Core;

namespace ChickenShooter.UI
{
    public class GameOverUI : MonoBehaviour
    {
        public GameObject panel;
        public Button retryButton;
        public Button quitToMenuButton;

        void Start()
        {
            panel.SetActive(false);

            retryButton.onClick.AddListener(Retry);
            quitToMenuButton.onClick.AddListener(QuitToMenu);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += Show;
            }
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver -= Show;
            }
        }

        void Show()
        {
            Time.timeScale = 0f;
            panel.SetActive(true);
        }

        void Retry()
        {
            Time.timeScale = 1f;
            GameManager.Instance.ResetForRetry();
            SceneManager.LoadScene($"Level{GameManager.Instance.LevelIndex}");
        }

        void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
