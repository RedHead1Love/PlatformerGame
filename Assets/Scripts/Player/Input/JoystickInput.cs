using PinePie.SimpleJoystick;
using Player.Input;
using UnityEngine;
using UnityEngine.UI;

public sealed class JoystickInput : MonoBehaviour, IInputProvider
{
    [SerializeField] private JoystickController _joystick;

    [Header("Gameplay Buttons")]
    [SerializeField] private Button _jumpButton;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _secondaryAttackButton;
    [SerializeField] private Button _slideButton;
    [SerializeField] private Button _liftButton;
    [SerializeField] private Button _dropButton;

    [Header("UI Buttons")]
    [SerializeField] private Button _mapButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _shopOrChestButton;

    private bool _isJumpPressed;
    private bool _isAttackPressed;
    private bool _isSecondaryAttackPressed;
    private bool _isSlidePressed;
    private bool _isLiftPressed;
    private bool _isDropPressed;
    private bool _isMapPressed;
    private bool _isMenuPressed;
    private bool _isOpenShopOrChestPressed;

    private bool _isInputBlocked;
    private bool _isShopOpen;

    public float HorizontalAxis
    {
        get
        {
            if (IsGameplayInputBlocked())
            {
                return 0f;
            }

            if (_joystick == null)
            {
                return 0f;
            }

            return _joystick.InputDirection.x;
        }
    }

    public bool IsJumpPressed => IsGameplayInputBlocked() == false && _isJumpPressed;
    public bool IsAttackPressed => IsGameplayInputBlocked() == false && _isAttackPressed;
    public bool IsSecondaryAttackPressed => IsGameplayInputBlocked() == false && _isSecondaryAttackPressed;
    public bool IsSlidePressed => IsGameplayInputBlocked() == false && _isSlidePressed;
    public bool IsLiftPressed => IsGameplayInputBlocked() == false && _isLiftPressed;
    public bool IsDropHeroPressed => IsGameplayInputBlocked() == false && _isDropPressed;
    public bool IsOpenMapPressed => IsGameplayInputBlocked() == false && _isMapPressed;
    public bool IsMenuPressed => _isInputBlocked == false && _isMenuPressed;
    public bool IsOpenShopOrChestPressed => _isInputBlocked == false && _isOpenShopOrChestPressed;

    private void Start()
    {
#if UNITY_WEBGL
    if (YG.YG2.envir.isMobile || YG.YG2.envir.isTablet)
    {
        gameObject.SetActive(true);
    }
#endif
    }

    private void OnEnable()
    {
        SubscribeButtons();
    }

    private void LateUpdate()
    {
        ResetFrameInput();
    }

    private void OnDisable()
    {
        UnsubscribeButtons();
    }

    private void SubscribeButtons()
    {
        if (_jumpButton != null)
        {
            _jumpButton.onClick.AddListener(OnJumpButtonClicked);
        }

        if (_attackButton != null)
        {
            _attackButton.onClick.AddListener(OnAttackButtonClicked);
        }

        if (_secondaryAttackButton != null)
        {
            _secondaryAttackButton.onClick.AddListener(OnSecondaryAttackButtonClicked);
        }

        if (_slideButton != null)
        {
            _slideButton.onClick.AddListener(OnSlideButtonClicked);
        }

        if (_liftButton != null)
        {
            _liftButton.onClick.AddListener(OnLiftButtonClicked);
        }

        if (_dropButton != null)
        {
            _dropButton.onClick.AddListener(OnDropButtonClicked);
        }

        if (_mapButton != null)
        {
            _mapButton.onClick.AddListener(OnMapButtonClicked);
        }

        if (_menuButton != null)
        {
            _menuButton.onClick.AddListener(OnMenuButtonClicked);
        }

        if (_shopOrChestButton != null)
        {
            _shopOrChestButton.onClick.AddListener(OnShopOrChestButtonClicked);
        }
    }

    private void UnsubscribeButtons()
    {
        if (_jumpButton != null)
        {
            _jumpButton.onClick.RemoveListener(OnJumpButtonClicked);
        }

        if (_attackButton != null)
        {
            _attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        }

        if (_secondaryAttackButton != null)
        {
            _secondaryAttackButton.onClick.RemoveListener(OnSecondaryAttackButtonClicked);
        }

        if (_slideButton != null)
        {
            _slideButton.onClick.RemoveListener(OnSlideButtonClicked);
        }

        if (_liftButton != null)
        {
            _liftButton.onClick.RemoveListener(OnLiftButtonClicked);
        }

        if (_dropButton != null)
        {
            _dropButton.onClick.RemoveListener(OnDropButtonClicked);
        }

        if (_mapButton != null)
        {
            _mapButton.onClick.RemoveListener(OnMapButtonClicked);
        }

        if (_menuButton != null)
        {
            _menuButton.onClick.RemoveListener(OnMenuButtonClicked);
        }

        if (_shopOrChestButton != null)
        {
            _shopOrChestButton.onClick.RemoveListener(OnShopOrChestButtonClicked);
        }
    }

    private void ResetFrameInput()
    {
        _isJumpPressed = false;
        _isAttackPressed = false;
        _isSecondaryAttackPressed = false;
        _isSlidePressed = false;
        _isLiftPressed = false;
        _isDropPressed = false;
        _isMapPressed = false;
        _isMenuPressed = false;
        _isOpenShopOrChestPressed = false;
    }

    private bool IsGameplayInputBlocked()
    {
        return _isInputBlocked || _isShopOpen;
    }

    private void OnJumpButtonClicked()
    {
        _isJumpPressed = true;
    }

    private void OnAttackButtonClicked()
    {
        _isAttackPressed = true;
    }

    private void OnSecondaryAttackButtonClicked()
    {
        _isSecondaryAttackPressed = true;
    }

    private void OnSlideButtonClicked()
    {
        _isSlidePressed = true;
    }

    private void OnLiftButtonClicked()
    {
        _isLiftPressed = true;
    }

    private void OnDropButtonClicked()
    {
        _isDropPressed = true;
    }

    private void OnMapButtonClicked()
    {
        _isMapPressed = true;
    }

    private void OnMenuButtonClicked()
    {
        _isMenuPressed = true;
    }

    private void OnShopOrChestButtonClicked()
    {
        _isOpenShopOrChestPressed = true;
    }

    public void BlockInput(bool isBlocked)
    {
        _isInputBlocked = isBlocked;
    }

    public void SetShopMode(bool isShopOpen)
    {
        _isShopOpen = isShopOpen;
    }
}