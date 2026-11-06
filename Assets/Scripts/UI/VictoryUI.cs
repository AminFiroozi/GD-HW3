using UnityEngine;
using ChickenShooter.Core;

namespace ChickenShooter.UI
{
    public class VictoryUI : MonoBehaviour
    {
        public GameObject panel;
        public AudioClip victoryMusic;

        void Start()
        {
            panel.SetActive(false);

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
    }
}
