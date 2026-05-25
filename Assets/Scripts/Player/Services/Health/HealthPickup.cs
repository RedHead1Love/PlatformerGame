using Player;
using UnityEngine;

public sealed class HealthPickup : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const float DestroyDelay = 2f;
    private const float GizmoRadius = 0.5f;

    [Header("Health Settings")]
    [SerializeField] private bool _isFullHeal = true;
    [SerializeField] private string _pickupId;
    [SerializeField] private bool _requiresAnatomyAbility = true;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem _collectEffect;
    [SerializeField] private Color _lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color _collectedColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);

    private bool _isCollected = false;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Hero _playerHero;

    private bool _hasValidId = false;
    private bool _canPickup = false;

    private Color _originalColor;

    private void Start()
    {
        InitializeComponents();
        LoadCollectionState();
        FindPlayerHero();

        if (string.IsNullOrEmpty(_pickupId))
        {
            _pickupId = GenerateUniqueId();
            _hasValidId = true;
        }
        else
        {
            _hasValidId = true;
        }

        UpdatePickupState();
    }

    private void Update()
    {
        int frameCheckInterval = 60;
        int frameModuloZero = 0;

        if (!_isCollected && Time.frameCount % frameCheckInterval == frameModuloZero)
        {
            RefreshPickupState();
        }
    }

    private void InitializeComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    private void FindPlayerHero()
    {
        if (_playerHero == null)
        {
            _playerHero = FindFirstObjectByType<Hero>();
        }
    }

    private string GenerateUniqueId()
    {
        return $"Health_{transform.position.x}_{transform.position.y}";
    }

    private void LoadCollectionState()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.CurrentSave?.CollectedHealthPickups != null)
        {
            _isCollected = SaveSystem.Instance.CurrentSave.CollectedHealthPickups.Contains(_pickupId);
        }
    }

    private void UpdatePickupState()
    {
        if (_isCollected)
        {
            DisablePickup();
            return;
        }

        if (_requiresAnatomyAbility)
        {
            _canPickup = _playerHero != null && _playerHero.AbilityManager != null && _playerHero.AbilityManager.HasAnatomy;
        }
        else
        {
            _canPickup = true;
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _canPickup ? _originalColor : _lockedColor;
        }
    }

    private void DisablePickup()
    {
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = false;
        }
    }

    private void ApplyHealing(GameObject player)
    {
        HealthManager healthManager = player.GetComponent<HealthManager>() ?? FindFirstObjectByType<HealthManager>();

        if (healthManager != null)
        {
            if (_isFullHeal)
            {
                healthManager.FullHeal();
            }
            else
            {
                healthManager.Heal(2);
            }

            PlayHealSound(player);
        }
    }

    private void PlayHealSound(GameObject player)
    {
        AudioController audioController = player.GetComponent<AudioController>();

        audioController?.PlayHealSound();
    }

    private void PlayCollectEffects()
    {
        if (_collectEffect == null)
        {
            return;
        }

        ParticleSystem effectInstance = Instantiate(_collectEffect, transform.position, Quaternion.identity);

        effectInstance.Play();

        Destroy(effectInstance.gameObject, DestroyDelay);
    }

    public void OnAnatomyAbilityPurchased()
    {
        if (!_isCollected)
        {
            UpdatePickupState();
        }
    }

    public void RefreshPickupState()
    {
        FindPlayerHero();
        UpdatePickupState();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag) && !_isCollected && _canPickup)
        {
            ApplyHealing(other.gameObject);
            PlayCollectEffects();

            _isCollected = true;

            if (_hasValidId && SaveSystem.Instance != null && SaveSystem.Instance.CurrentSave?.CollectedHealthPickups != null)
            {
                SaveSystem.Instance.CurrentSave.CollectedHealthPickups.Add(_pickupId);
            }

            DisablePickup();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GizmoRadius);
    }
}
