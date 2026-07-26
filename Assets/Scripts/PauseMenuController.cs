using Player.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public sealed class PauseMenuController : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _pausePanel;

    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button _continueButton;
    [SerializeField] private UnityEngine.UI.Button _exitButton;

    private IInputProvider _inputProvider;
    private bool _isPaused;

    private void Start()
    {
        FindInputProvider();
        SetupButtons();
        HidePauseMenu();

        ResumeGame();
    }

    private void Update()
    {
        if (_inputProvider != null && _inputProvider.IsMenuPressed)
        {
            TogglePause();
        }
    }

    private void SetupButtons()
    {
        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(ResumeGame);
        }

        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(ExitToMainMenu);
        }
    }

    private void FindInputProvider()
    {
        _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();

        if (_inputProvider == null && YG2.envir.isDesktop)
        {
            _inputProvider = FindFirstObjectByType<OldInputProvider>();
        }

        if (_inputProvider == null && YG2.envir.isMobile)
        {
            _inputProvider = FindFirstObjectByType<JoystickInput>();
        }
    }

    public void TogglePause()
    {
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        _isPaused = true;
        IsGamePaused = true;

        Time.timeScale = 0f;

        AudioListener.pause = true;

        ShowPauseMenu();
    }

    private void ResumeGame()
    {
        _isPaused = false;
        IsGamePaused = false;

        Time.timeScale = 1f;

        AudioListener.pause = false;

        HidePauseMenu();
    }

    private void ExitToMainMenu()
    {
        ResumeGame();

        SceneManager.LoadScene("MainMenu");
    }

    private void ShowPauseMenu()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
        }
    }

    private void HidePauseMenu()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        ResumeGame();
    }
}