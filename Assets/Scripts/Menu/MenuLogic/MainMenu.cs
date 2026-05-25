using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class MainMenu : MonoBehaviour
{
    private const string FirstLevelName = "LevelDungeon";

    [Header("Menu Buttons")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        InitializeGameState();
        InitializeButtonListeners();
        UpdateContinueButtonVisibility();
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void InitializeGameState()
    {
        Time.timeScale = 1f;
    }

    private void InitializeButtonListeners()
    {
        _newGameButton?.onClick.AddListener(StartNewGame);
        _continueButton?.onClick.AddListener(ContinueGame);
        _exitButton?.onClick.AddListener(ExitGame);

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void UpdateContinueButtonVisibility()
    {
        if (_continueButton == null)
        {
            return;
        }

        bool hasSaveGame = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();

        _continueButton.gameObject.SetActive(hasSaveGame);
    }

    private void StartNewGame()
    {
        PlayButtonSound();
        ResetGameProgress();

        GameManager.Instance?.SetNewGameFlag();

        LoadFirstLevel();
    }

    private void ContinueGame()
    {
        PlayButtonSound();
        LoadSavedGameLevel();
    }

    private void ExitGame()
    {
        PlayButtonSound();
        QuitApplication();
    }

    private void PlayButtonSound()
    {
        if (_buttonClickSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }

    private void ResetGameProgress()
    {
        SaveSystem.Instance?.DeleteSaveData();
        EnemyManager.Instance?.ResetAllEnemies();
        GameStateManager.ResetGameState();

        ClearPlayerPrefs();
    }

    private void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(FirstLevelName);
    }

    private void LoadSavedGameLevel()
    {
        string savedSceneName = SaveSystem.Instance.CurrentSave.SceneName;
        string sceneToLoad = string.IsNullOrEmpty(savedSceneName) ? FirstLevelName : savedSceneName;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartNewGame();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && _continueButton.gameObject.activeSelf)
        {
            ContinueGame();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExitGame();
        }
    }
}
