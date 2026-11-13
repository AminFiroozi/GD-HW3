using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChickenShooter.Input;

namespace ChickenShooter.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public GameObject panel;
        public Button resumeButton;
        public Button quitToMenuButton;

        PlayerControls controls;
        bool isPaused;

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            controls.Player.Enable();
            controls.Player.Pause.performed += OnPauseToggled;
        }

        void OnDisable()
        {
            controls.Player.Pause.performed -= OnPauseToggled;
            controls.Player.Disable();
        }

        void Start()
        {
            resumeButton.onClick.AddListener(Resume);
            quitToMenuButton.onClick.AddListener(QuitToMenu);
            SetPaused(false);
        }

        void OnPauseToggled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            SetPaused(!isPaused);
        }

        void Resume() => SetPaused(false);

        void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        void SetPaused(bool paused)
        {
            isPaused = paused;
            panel.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
        }
    }
}
