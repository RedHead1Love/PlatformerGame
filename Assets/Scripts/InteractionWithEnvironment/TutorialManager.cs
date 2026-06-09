using UnityEngine;

public sealed class TutorialManager : MonoBehaviour
{
    private const float PausedTimeScale = 0f;
    private const float NormalTimeScale = 1f;

    [SerializeField] private GameObject _tutorialPanel;

    private static bool _isTutorialShown;

    private void Start()
    {
        InitializeTutorial();
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

    public void TryCloseTutorial()
    {
        if (_tutorialPanel != null && _tutorialPanel.activeInHierarchy)
        {
            CloseTutorial();
        }
    }

    private void InitializeTutorial()
    {
        if (_tutorialPanel == null)
        {
            ResumeGame();

            return;
        }

        if (_isTutorialShown == false)
        {
            ShowTutorial();
        }
        else
        {
            ResumeGame();
            _tutorialPanel.SetActive(false);
        }
    }

    private void ShowTutorial()
    {
        _tutorialPanel.SetActive(true);

        PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = PausedTimeScale;
    }

    private void ResumeGame()
    {
        Time.timeScale = NormalTimeScale;
    }
}