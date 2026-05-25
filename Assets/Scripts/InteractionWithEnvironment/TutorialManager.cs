using Player.Input;
using UnityEngine;

public sealed class TutorialManager : MonoBehaviour
{
    private const KeyCode CloseTutorialKey = KeyCode.V;
    private const float PausedTimeScale = 0f;
    private const float NormalTimeScale = 1f;

    [SerializeField] private GameObject _tutorialPanel;

    private static bool _isTutorialShown = false;

    private void Start()
    {
        InitializeTutorial();
    }

    private void InitializeTutorial()
    {
        if (!_isTutorialShown)
        {
            ShowTutorial();
        }
        else
        {
            ResumeGame();

            if (_tutorialPanel != null)
            {
                _tutorialPanel.SetActive(false);
            }
        }
    }

    private void ShowTutorial()
    {
        if (_tutorialPanel != null)
        {
            _tutorialPanel.SetActive(true);
        }

        PauseGame();
    }

    public void CloseTutorial()
    {
        if (_tutorialPanel != null)
        {
            _tutorialPanel.SetActive(false);
        }

        ResumeGame();

        _isTutorialShown = true;
    }

    public static void ResetTutorial()
    {
        _isTutorialShown = false;
    }

    private void PauseGame()
    {
        Time.timeScale = PausedTimeScale;
    }

    private void ResumeGame()
    {
        Time.timeScale = NormalTimeScale;
    }

    public void TryCloseTutorial()
    {
        if (_tutorialPanel.activeInHierarchy)
        {
            CloseTutorial();
        }
    }
}
}
