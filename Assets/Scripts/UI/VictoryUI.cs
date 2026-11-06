using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChickenShooter.Core;

namespace ChickenShooter.UI
{
    public class VictoryUI : MonoBehaviour
    {
        public GameObject panel;
        public AudioClip victoryMusic;
        public Button mainMenuButton;

        void Start()
        {
            panel.SetActive(false);
            mainMenuButton.onClick.AddListener(QuitToMenu);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnVictory += Show;
            }
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnVictory -= Show;
            }
        }

        void Show()
        {
            Time.timeScale = 0f;
            panel.SetActive(true);
            AudioManager.Instance?.PlayMusic(victoryMusic);
        }

        void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
