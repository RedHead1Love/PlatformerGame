using UnityEngine;

public sealed class PlayerStateLoader : MonoBehaviour
{
    [SerializeField] private Hero _player;
    [SerializeField] private HealthBarUI _healthBarUI;

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
            SaveSystem.Instance.OnGameLoaded += HandleGameLoaded;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.OnGameLoaded -= HandleGameLoaded;
        }
    }

    private void LoadInitialPlayerState()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            HandleGameLoaded(SaveSystem.Instance.CurrentSave);

            return;
        }

        SetDefaultPlayerHealth();
    }

    private void HandleGameLoaded(GameSaveData saveData)
    {
        if (saveData == null || _player == null)
        {
            return;
        }

        ApplySavedPlayerState(saveData);
    }

    private void ApplySavedPlayerState(GameSaveData saveData)
    {
        _player.SetHealth(saveData.playerHealth);
        _healthBarUI?.SetHealth(saveData.playerHealth);
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