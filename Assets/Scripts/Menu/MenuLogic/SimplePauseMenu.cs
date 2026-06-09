using Player.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public sealed class SimplePauseMenu : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const float NormalTimeScale = 1f;
    private const float PausedTimeScale = 0f;

    [SerializeField] private IInputProvider _inputProvider;

    private bool _isPaused;
    private Rect _pauseWindowRect;

    private void Start()
    {
        InitializePauseWindow();
        FindInputProvider();
    }

    private void Update()
    {
        if (_inputProvider != null && _inputProvider.IsMenuPressed)
        {
            TogglePauseState();
        }
    }

    private void OnGUI()
    {
        if (_isPaused == false)
        {
            return;
        }

        _pauseWindowRect = GUI.Window(0, _pauseWindowRect, DrawPauseWindow, "Пауза");
    }

    private void OnDestroy()
    {
        ResumeGame();
    }

    private void FindInputProvider()
    {
        if (_inputProvider != null)
        {
            return;
        }

        _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();

        if (_inputProvider == null && YG2.envir.isDesktop)
        {
            _inputProvider = FindFirstObjectByType<OldInputProvider>();
        }

        if (_inputProvider == null && YG2.envir.isMobile)
        {
            _inputProvider = FindFirstObjectByType<JoystickInput>();
        }

        if (_inputProvider == null)
        {
            Debug.LogWarning("IInputProvider not found! Pause menu won't work.");
        }
    }

    private void InitializePauseWindow()
    {
        float windowWidth = 200f;
        float windowHeight = 150f;

        float centerX = Screen.width / 2f - windowWidth / 2f;
        float centerY = Screen.height / 2f - windowHeight / 2f;

        _pauseWindowRect = new Rect(centerX, centerY, windowWidth, windowHeight);
    }

    private void TogglePauseState()
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
        Time.timeScale = PausedTimeScale;
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = NormalTimeScale;
    }

    private void ExitToMainMenu()
    {
        ResumeGame();

        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void DrawPauseWindow(int windowID)
    {
        float topPadding = 20f;
        float dragAreaHeight = 20f;
        float dragAreaWidth = 10000f;
        float buttonHeight = 30f;
        float verticalSpacing = 10f;

        GUILayout.Space(topPadding);

        if (GUILayout.Button("Продолжить", GUILayout.Height(buttonHeight)))
        {
            ResumeGame();
        }

        GUILayout.Space(verticalSpacing);

        if (GUILayout.Button("Выход в меню", GUILayout.Height(buttonHeight)))
        {
            ExitToMainMenu();
        }

        GUI.DragWindow(new Rect(0f, 0f, dragAreaWidth, dragAreaHeight));
    }
}