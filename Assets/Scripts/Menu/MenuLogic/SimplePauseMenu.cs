using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SimplePauseMenu : MonoBehaviour
{
    private const KeyCode PauseKey = KeyCode.Escape;
    private const string MainMenuSceneName = "MainMenu";
    private const float NormalTimeScale = 1f;
    private const float PausedTimeScale = 0f;

    private bool _isPaused = false;
    private Rect _pauseWindowRect;

    private void Start()
    {
        InitializePauseWindow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(PauseKey))
        {
            TogglePauseState();
        }
    }

    private void OnGUI()
    {
        if (!_isPaused)
        {
            return;
        }

        _pauseWindowRect = GUI.Window(0, _pauseWindowRect, DrawPauseWindow, "Пауза");
    }

    private void OnDestroy()
    {
        ResumeGame();
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

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
