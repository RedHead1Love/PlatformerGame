using DoorControl;
using Player.Input;
using UnityEngine;
using YG;

public sealed class Door : MonoBehaviour, IOpenable
{
    private const float BoxRotationAngle = 0f;

    [Header("Input")]
    [SerializeField] private IInputProvider _inputProvider;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteOpened;
    public Sprite spriteClosed;

    [Header("Interaction")]
    public Vector2 triggerSize = new Vector2(1.5f, 1.5f);
    public LayerMask playerLayer;

    [Header("Key Settings")]
    public bool requiresKey = false;
    public KeyColor requiredKeyColor = KeyColor.WhiteColor;

    [Header("Sounds")]
    public AudioClip openSound;
    public AudioClip closeSound;

    [Header("Sound Settings")]
    public float soundRange = 5f;
    public bool checkPlayerDistance = true;

    private bool isOpened;
    private Transform player;
    private Animator animator;
    private KeyCollection playerKeyCollection;
    private AudioController audioController;
    private Collider2D doorCollider;

    public bool IsClosed => isOpened == false;

    void IOpenable.Open()
    {
        Open();
    }

    private void Awake()
    {
        InitializeComponents();
        InitializePlayerReferences();
    }

    private void Start()
    {
        ApplyInitialState();
        FindInputProvider();
    }

    private void Update()
    {
        if (_inputProvider == null)
        {
            return;
        }

        if (_inputProvider.IsOpenShopOrChestPressed && IsPlayerInsideInteractionArea())
        {
            TryToggle();
        }
    }

    public void Open()
    {
        if (isOpened)
        {
            return;
        }

        isOpened = true;

        SetAnimatorState(true);
        SetColliderEnabled(false);
        ApplyVisualState(true);
        PlayOpenSound();
    }

    public void Close()
    {
        if (isOpened == false)
        {
            return;
        }

        isOpened = false;

        SetAnimatorState(false);
        SetColliderEnabled(true);
        ApplyVisualState(false);
        PlayCloseSound();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
        audioController = FindFirstObjectByType<AudioController>();
    }

    private void InitializePlayerReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        player = playerObject.transform;
        playerKeyCollection = playerObject.GetComponent<KeyCollection>();

        if (playerKeyCollection == null)
        {
            playerKeyCollection = playerObject.GetComponentInParent<KeyCollection>();
        }
    }

    private void ApplyInitialState()
    {
        if (animator != null)
        {
            animator.Play(isOpened ? "Opened" : "Closed");
        }

        SetColliderEnabled(isOpened == false);
        ApplyVisualState(isOpened);
    }

    private void FindInputProvider()
    {
        if (_inputProvider != null)
        {
            return;
        }

        _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();

        if (_inputProvider == null && YG2.envir.isDesktop)
        {
            _inputProvider = FindFirstObjectByType<OldInputProvider>();
        }

        if (_inputProvider == null && YG2.envir.isMobile)
        {
            _inputProvider = FindFirstObjectByType<JoystickInput>();
        }

        if (_inputProvider == null)
        {
            Debug.LogWarning("IInputProvider не найден");
        }
    }

    private bool IsPlayerInsideInteractionArea()
    {
        return Physics2D.OverlapBox(transform.position, triggerSize, BoxRotationAngle, playerLayer) != null;
    }

    private void TryToggle()
    {
        if (isOpened)
        {
            Close();

            return;
        }

        TryOpen();
    }

    private void TryOpen()
    {
        if (requiresKey == false)
        {
            Open();

            return;
        }

        if (playerKeyCollection != null && playerKeyCollection.HasKey(requiredKeyColor))
        {
            Open();
        }
    }

    private void SetAnimatorState(bool opened)
    {
        if (animator != null)
        {
            animator.SetBool("IsOpened", opened);
        }
    }

    private void SetColliderEnabled(bool enabled)
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = enabled;
        }
    }

    private void ApplyVisualState(bool opened)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = opened ? spriteOpened : spriteClosed;
        }
    }

    private void PlayOpenSound()
    {
        PlaySound(openSound);
    }

    private void PlayCloseSound()
    {
        PlaySound(closeSound);
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound == null || ShouldPlaySound() == false)
        {
            return;
        }

        if (audioController != null)
        {
            audioController.PlayOneShot(sound);
        }
        else
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }

    private bool ShouldPlaySound()
    {
        return checkPlayerDistance == false || IsPlayerInRange();
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            return true;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        return distance <= soundRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, triggerSize);
    }
}