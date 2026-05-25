using DoorControl;
using Player.Input;
using UnityEngine;
using YG;

public sealed class Door : MonoBehaviour, IOpenable
{
    [Header("Input")]
    [SerializeField] private IInputProvider _inputProvider;

    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _spriteOpened;
    [SerializeField] private Sprite _spriteClosed;

    [Header("Interaction")]
    
    public Vector2 triggerSize = new Vector2(1.5f, 1.5f);
    public LayerMask playerLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.F;
    [SerializeField] private Vector2 _triggerSize = new Vector2(1.5f, 1.5f);
    [SerializeField] private LayerMask _playerLayer;

    [Header("Key Settings")]
    [SerializeField] private bool _requiresKey = false;
    [SerializeField] private KeyColor _requiredKeyColor = KeyColor.WhiteColor;

    [Header("Sounds")]
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;

    [Header("Sound Settings")]
    [SerializeField] private float _soundRange = 5f;
    [SerializeField] private bool _checkPlayerDistance = true;

    private bool _isOpened;
    private Transform _playerTransform;
    private Animator _animator;
    private KeyCollection _playerKeyCollection;
    private AudioController _audioController;

    public bool IsClosed => !_isOpened;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        _audioController = FindFirstObjectByType<AudioController>();

        if (_playerTransform != null)
        {
            _playerKeyCollection = _playerTransform.GetComponent<KeyCollection>();

            if (_playerKeyCollection == null)
            {
                _playerKeyCollection = _playerTransform.GetComponentInParent<KeyCollection>();
            }
        }
    }

    private void Start()
    {
        if (animator != null)
        {
            animator.Play(isOpened ? "Opened" : "Closed");
        }

        ApplyVisualState(isOpened);
        FindInputProvider();
    }

    private void Update()
    {
        if (_inputProvider.IsOpenShopOrChestPressed &&
            Physics2D.OverlapBox(transform.position, triggerSize, 0, playerLayer))
        {
            TryToggle();
        }
    }

    private void TryToggle()
    public void Open()
    {
        if (_isOpened)
        {
            return;
        }

        if (_requiresKey)
        {
            if (_playerKeyCollection == null || !_playerKeyCollection.HasKey(_requiredKeyColor))
            {
                return;
            }

            _playerKeyCollection.RemoveKey(_requiredKeyColor);
        }

        _isOpened = true;

        Collider2D doorCollider = GetComponent<Collider2D>();

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        ApplyVisualState(true);
        PlaySound(_openSound);
    }

    private void ApplyVisualState(bool opened)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = opened ? _spriteOpened : _spriteClosed;
        }
    }

    private void FindInputProvider()
    {
        if (_inputProvider == null)
        {
            if (YG2.envir.isDesktop)
            {
                _inputProvider = FindObjectOfType<OldInputProvider>();
            }
            else if (YG2.envir.isMobile)
            {
                _inputProvider = FindObjectOfType<JoystickInput>();
            }
        }

        if (_inputProvider == null)
            Debug.LogWarning("IInputProvider not found! Map toggle won't work with key.");
    }
    private void ApplyVisualState(bool opened)
    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (!_checkPlayerDistance || IsPlayerInRange())
        {
            if (_audioController != null)
            {
                _audioController.PlayOneShot(clip);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (_playerTransform == null)
        {
            return true;
        }

        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        return distance <= _soundRange;
    }
}
