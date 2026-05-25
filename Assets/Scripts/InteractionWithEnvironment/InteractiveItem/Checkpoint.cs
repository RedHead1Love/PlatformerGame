using UnityEngine;

public sealed class Checkpoint : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private string _checkpointId;
    [SerializeField] private AudioClip _activationSound;

    private bool _isActivated;
    private SpriteRenderer _spriteRenderer;

    public string GetCheckpointId() => _checkpointId;
    public Vector3 GetSpawnPosition() => transform.position;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (SaveSystem.Instance?.CurrentSave?.CheckpointId == _checkpointId)
        {
            _isActivated = true;
        }

        UpdateVisuals();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActivated && other.CompareTag(PlayerTag))
        {
            Activate(other.transform.position);
        }
    }

    private void Activate(Vector3 playerPosition)
    {
        _isActivated = true;

        SaveSystem.Instance?.SaveGame(_checkpointId, playerPosition);

        if (_activationSound)
        {
            AudioSource.PlayClipAtPoint(_activationSound, transform.position);
        }

        UpdateVisuals();
    }

    private void UpdateVisuals() => _spriteRenderer.color = _isActivated ? Color.green : Color.white;
}
