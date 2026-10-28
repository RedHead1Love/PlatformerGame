using DoorControl;
using Player.Input;
using UnityEngine;
using UnityEngine.Serialization;
using YG;

public sealed class Door : MonoBehaviour, IOpenable
{
    private const float BoxRotationAngle = 0f;

    [SerializeField] private IInputProvider _inputProvider;

    [FormerlySerializedAs("spriteRenderer")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [FormerlySerializedAs("spriteOpened")]
    [SerializeField] private Sprite _spriteOpened;
    [FormerlySerializedAs("spriteClosed")]
    [SerializeField] private Sprite _spriteClosed;

    [SerializeField] private GameObject _interactionHint;
    [SerializeField] private GameObject _missingKeyHint;

    [FormerlySerializedAs("triggerSize")]
    [SerializeField] private Vector2 _triggerSize = new Vector2(1.5f, 1.5f);
    [FormerlySerializedAs("playerLayer")]
    [SerializeField] private LayerMask _playerLayer;

    [FormerlySerializedAs("requiresKey")]
    [SerializeField] private bool _requiresKey = false;
    [FormerlySerializedAs("requiredKeyColor")]
    [SerializeField] private KeyColor _requiredKeyColor = KeyColor.WhiteColor;

    [FormerlySerializedAs("openSound")]
    [SerializeField] private AudioClip _openSound;
    [FormerlySerializedAs("closeSound")]
    [SerializeField] private AudioClip _closeSound;

    [FormerlySerializedAs("soundRange")]
    [SerializeField] private float _soundRange = 5f;
    [FormerlySerializedAs("checkPlayerDistance")]
    [SerializeField] private bool _checkPlayerDistance = true;

    private bool _isOpened;
    private bool _wasPlayerInside;
    private Transform _player;
    private Animator _animator;
    private KeyCollection _playerKeyCollection;
    private AudioController _audioController;
    private Collider2D _doorCollider;

    public bool IsClosed => _isOpened == false;
    public bool RequiresKey => _requiresKey;

    void IOpenable.Open()
    {
        Open();
    }

    private void Awake()
    {
        InitializeComponents();
        InitializePlayerReferences();
        HideAllHints();
    }

    private void Start()
    {
        ApplyInitialState();
        FindInputProvider();
    }

    private void Update()
    {
        bool isInside = IsPlayerInsideInteractionArea();

        if (isInside != _wasPlayerInside)
        {
            _wasPlayerInside = isInside;
            UpdateInteractionHint(isInside);
        }

        if (_inputProvider == null)
        {
            return;
        }

        if (_inputProvider.IsOpenShopOrChestPressed && isInside)
        {
            TryToggle();
        }
    }

    public void Open()
    {
        if (_isOpened)
        {
            return;
        }

        _isOpened = true;

        HideAllHints();
        SetAnimatorState(true);
        SetColliderEnabled(false);
        ApplyVisualState(true);
        PlayOpenSound();
    }

    public void Close()
    {
        if (_isOpened == false)
        {
            return;
        }

        _isOpened = false;

        if (_wasPlayerInside && _interactionHint != null)
        {
            _interactionHint.SetActive(true);
        }

        SetAnimatorState(false);
        SetColliderEnabled(true);
        ApplyVisualState(false);
        PlayCloseSound();
    }

    private void UpdateInteractionHint(bool isInside)
    {
        if (_isOpened) return;

        if (_interactionHint != null)
        {
            _interactionHint.SetActive(isInside);
        }

        if (isInside == false && _missingKeyHint != null)
        {
            _missingKeyHint.SetActive(false);
        }
    }

    private void HideAllHints()
    {
        if (_interactionHint != null) _interactionHint.SetActive(false);
        if (_missingKeyHint != null) _missingKeyHint.SetActive(false);
    }

    private void InitializeComponents()
    {
        _animator = GetComponent<Animator>();
        _doorCollider = GetComponent<Collider2D>();
        _audioController = FindFirstObjectByType<AudioController>();
    }

    private void InitializePlayerReferences()
    {
        if (Hero.Instance != null)
        {
            _player = Hero.Instance.transform;
            _playerKeyCollection = Hero.Instance.GetComponent<KeyCollection>();

            if (_playerKeyCollection == null)
            {
                _playerKeyCollection = Hero.Instance.GetComponentInParent<KeyCollection>();
            }
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerKeyCollection = playerObject.GetComponent<KeyCollection>();

            if (_playerKeyCollection == null)
            {
                _playerKeyCollection = playerObject.GetComponentInParent<KeyCollection>();
            }
        }
    }

    private void ApplyInitialState()
    {
        if (_animator != null)
        {
            _animator.Play(_isOpened ? "Opened" : "Closed");
        }

        SetColliderEnabled(_isOpened == false);
        ApplyVisualState(_isOpened);
    }

    private void FindInputProvider()
    {
        if (_inputProvider != null) return;

        _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();

        if (_inputProvider == null && YG2.envir.isDesktop)
            _inputProvider = FindFirstObjectByType<LegacyInputProvider>();

        if (_inputProvider == null && YG2.envir.isMobile)
            _inputProvider = FindFirstObjectByType<JoystickInput>();

        if (_inputProvider == null)
            Debug.LogWarning("IInputProvider не найден");
    }

    private bool IsPlayerInsideInteractionArea()
    {
        return Physics2D.OverlapBox(transform.position, _triggerSize, BoxRotationAngle, _playerLayer) != null;
    }

    private void TryToggle()
    {
        if (_isOpened)
        {
            Close();
            return;
        }

        TryOpen();
    }

    private void TryOpen()
    {
        if (_requiresKey == false)
        {
            Open();
            return;
        }

        if (_playerKeyCollection != null && _playerKeyCollection.HasKey(_requiredKeyColor))
        {
            Open();
        }
        else
        {
            if (_interactionHint != null) _interactionHint.SetActive(false);
            if (_missingKeyHint != null) _missingKeyHint.SetActive(true);
        }
    }

    private void SetAnimatorState(bool opened)
    {
        if (_animator != null) _animator.SetBool("IsOpened", opened);
    }

    private void SetColliderEnabled(bool enabled)
    {
        if (_doorCollider != null) _doorCollider.enabled = enabled;
    }

    private void ApplyVisualState(bool opened)
    {
        if (_spriteRenderer != null) _spriteRenderer.sprite = opened ? _spriteOpened : _spriteClosed;
    }

    private void PlayOpenSound() => PlaySound(_openSound);
    private void PlayCloseSound() => PlaySound(_closeSound);

    private void PlaySound(AudioClip sound)
    {
        if (sound == null || ShouldPlaySound() == false) return;

        if (_audioController != null)
            _audioController.PlayOneShot(sound);
        else
            AudioSource.PlayClipAtPoint(sound, transform.position);
    }

    private bool ShouldPlaySound() => _checkPlayerDistance == false || IsPlayerInRange();

    private bool IsPlayerInRange()
    {
        if (_player == null) return true;
        return Vector2.Distance(transform.position, _player.position) <= _soundRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _triggerSize);
    }
}
