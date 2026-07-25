using UnityEngine;

namespace Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class TutorialTrigger : MonoBehaviour
    {
        [Header("Tutorial Settings")]
        [SerializeField] private GameObject _tutorialPanel;
        [SerializeField] private string _playerTag = "Player";

        [Header("Behavior")]
        [SerializeField] private bool _triggerOnlyOnce = true;
        [SerializeField] private bool _pauseGameOnTrigger = false;

        private bool _hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasTriggered && _triggerOnlyOnce)
            {
                return;
            }

            if (other.CompareTag(_playerTag))
            {
                ActivateTutorial();
            }
        }

        private void ActivateTutorial()
        {
            _hasTriggered = true;

            if (_tutorialPanel != null)
            {
                _tutorialPanel.SetActive(true);

                if (_pauseGameOnTrigger)
                {
                    Time.timeScale = 0f;
                }
            }
        }
    }
}