using Foxberry;
using Player;
using UnityEngine;

namespace GameLogic
{
    public sealed class PlayerStateLoader : MonoBehaviour
    {
        [SerializeField]
        private Hero _player;
        [SerializeField]
        private HealthBarUI _healthBarUI;

        private void Start()
        {
            InitializeReferences();
            SubscribeToEvents();
            LoadInitialPlayerState();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void InitializeReferences()
        {
            if (_player == null)
            {
                _player = FindFirstObjectByType<Hero>();
            }

            if (_healthBarUI == null)
            {
                _healthBarUI = FindFirstObjectByType<HealthBarUI>();
            }
        }

        private void SubscribeToEvents()
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.OnGameLoaded += OnSaveLoaded;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.OnGameLoaded -= OnSaveLoaded;
            }
        }

        private void LoadInitialPlayerState()
        {
            if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
            {
                OnSaveLoaded(SaveSystem.Instance.CurrentSave);

                return;
            }

            SetDefaultPlayerHealth();
        }

        private void OnSaveLoaded(GameSaveData saveData)
        {
            if (saveData == null || _player == null)
            {
                return;
            }

            ApplySavedPlayerState(saveData);
        }

        private void ApplySavedPlayerState(GameSaveData saveData)
        {
            _player.SetHealth(saveData.Health);
            _healthBarUI?.SetHealth(saveData.Health);
        }

        private void SetDefaultPlayerHealth()
        {
            if (_player == null)
            {
                return;
            }

            _player.SetHealth(_player.Data.MaxLives);
        }
    }
}
