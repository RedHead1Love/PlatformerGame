using UnityEngine;

namespace Foxberry
{
    public sealed class TutorialManager : MonoBehaviour
    {
        private const float PausedTimeScale = 0f;
        private const float NormalTimeScale = 1f;

        private static bool _isTutorialShown = false;

        [SerializeField]
        private GameObject _tutorialPanel;

        public static void ResetTutorial()
        {
            _isTutorialShown = false;
        }

        public void CloseTutorial()
        {
            _tutorialPanel.SetActive(false);

            ResumeGame();

            _isTutorialShown = true;
        }

        public void TryCloseTutorial()
        {
            if (_tutorialPanel.activeInHierarchy == false)
            {
                return;
            }

            if (Input.anyKeyDown)
            {
                CloseTutorial();
            }
        }

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
}
