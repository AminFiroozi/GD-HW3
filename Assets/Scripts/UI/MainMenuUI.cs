using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChickenShooter.Core;

namespace ChickenShooter.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public Button newGameButton;
        public Button continueButton;
        public Slider volumeSlider;
        public Button quitButton;
        public AudioClip menuMusic;

        const string FirstLevelScene = "Level1";

        void Start()
        {
            AudioManager.Instance?.PlayMusic(menuMusic);

            continueButton.interactable = SaveSystem.HasSave();

            newGameButton.onClick.AddListener(OnNewGame);
            continueButton.onClick.AddListener(OnContinue);
            quitButton.onClick.AddListener(OnQuit);

            if (volumeSlider != null)
            {
                volumeSlider.value = AudioListener.volume;
                volumeSlider.onValueChanged.AddListener(v => AudioListener.volume = v);
            }
        }

        void OnNewGame()
        {
            GameManager.Instance.ResetForNewGame();
            SceneManager.LoadScene(FirstLevelScene);
        }

        void OnContinue()
        {
            var data = SaveSystem.Load();
            if (data != null)
            {
                GameManager.Instance.LoadFromSaveData(data);
            }
            SceneManager.LoadScene($"Level{(data != null ? data.levelIndex : 1)}");
        }

        void OnQuit()
        {
            Application.Quit();
        }
    }
}
