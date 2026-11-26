using UnityEngine;

namespace Foxberry
{
    public sealed class TutorialManager : MonoBehaviour
    {
        private const float PausedTimeScale = 0f;
        private const float NormalTimeScale = 1f;
        private readonly GamePauseScope _pause = new GamePauseScope();
        private static bool _isTutorialShown = false;
        [SerializeField]
        private GameObject _tutorialPanel;

        public static void ResetTutorial()
        {
            _isTutorialShown = false;
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

        private bool TryCloseTutorial()
        {
            if (_tutorialPanel == null || !_tutorialPanel.activeInHierarchy || !Input.anyKeyDown)
            {
                return false;
            }

            CloseTutorial();
            return true;
        }

        private void OnDisable() => _pause.Dispose();

        private void Start()
        {
            InitializeTutorial();
        }

        private void Update()
        {
            TryCloseTutorial();
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
            if (_tutorialPanel == null)
            {
                return;
            }

            _tutorialPanel.SetActive(true);
            PauseGame();
        }

        private void PauseGame()
        {
            _pause.Pause();
        }

        private void ResumeGame()
        {
            _pause.Dispose();
        }
    }
}
