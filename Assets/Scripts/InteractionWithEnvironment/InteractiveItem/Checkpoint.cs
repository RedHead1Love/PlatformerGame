using UnityEngine;

public sealed class Checkpoint : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private string _checkpointId;
    [SerializeField] private AudioClip _activationSound;

    private bool _isActivated;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        InitializeCheckpoint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActivated || !other.CompareTag(PlayerTag))
        {
            return;
        }

        ActivateCheckpoint(other.transform.position);
    }

    private void InitializeCheckpoint()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            GameSaveData saveData = SaveSystem.Instance.CurrentSave;
            _isActivated = saveData != null && saveData.checkpointId == _checkpointId;
        }

        UpdateVisualState();
    }

    private void ActivateCheckpoint(Vector3 playerPosition)
    {
        _isActivated = true;

        UpdateVisualState();
        PlayActivationEffects();

            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.SaveGame(_checkpointId, playerPosition);
            }

            GameStateManager.MarkGameSaved();
    }

    private void PlayActivationEffects()
    {
        if (_activationSound != null)
        {
            AudioSource.PlayClipAtPoint(_activationSound, transform.position);
        }
    }

    private void UpdateVisualState()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _isActivated ? Color.green : Color.white;
        }
    }

    public string GetCheckpointId() => _checkpointId;
    public bool IsActivated() => _isActivated;
    public Vector3 GetSpawnPosition() => transform.position;
}