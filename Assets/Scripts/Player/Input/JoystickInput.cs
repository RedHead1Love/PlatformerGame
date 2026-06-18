using PinePie.SimpleJoystick;
using Player.Input;
using UnityEngine;
using UnityEngine.UI;

public class JoystickInput : MonoBehaviour, IInputProvider
{
    [SerializeField] private JoystickController _joystick;

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

    private bool _isJumpPressed = false;
    private bool _isAttackPressed = false;
    private bool _isSecondaryAttackPressed = false;
    private bool _isSlidePressed = false;
    private bool _isLiftPressed = false;
    private bool _isDropPressed = false;
    private bool _isOpenShopOrChestPressed = false;

    private bool _isInputBlocked = false;
    private bool _isShopOpen = false;

    private bool _isMapPressed = false;
    private bool _isMenuPressed = false;

    private void OnEnable()
    {
        if (_jumpButton != null)
            _jumpButton.onClick.AddListener(() => _isJumpPressed = true);

        if (_attackButton != null)
            _attackButton.onClick.AddListener(() => _isAttackPressed = true);

        if (_secondaryAttackButton != null)
            _secondaryAttackButton.onClick.AddListener(() => _isSecondaryAttackPressed = true);

        if (_slideButton != null)
            _slideButton.onClick.AddListener(() => _isSlidePressed = true);

        if (_liftButton != null)
            _liftButton.onClick.AddListener(() => _isLiftPressed = true);

        if (_dropButton != null)
            _dropButton.onClick.AddListener(() => _isDropPressed = true);

        if (_mapButton != null)
            _mapButton.onClick.AddListener(() => _isMapPressed = true);

        if (_menuButton != null)
            _menuButton.onClick.AddListener(() => _isMenuPressed = true);

        if (_shopOrChestButton != null)
            _shopOrChestButton.onClick.AddListener(() => _isOpenShopOrChestPressed = true);

    }

    private void LateUpdate()
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

    public float HorizontalAxis
    {
        get
        {
            if (_isInputBlocked || _isShopOpen) return 0f;
            if (_joystick != null) return _joystick.InputDirection.x;
            return 0f;
        }
    }

    public bool IsJumpPressed => (!_isInputBlocked && !_isShopOpen) && _isJumpPressed;
    public bool IsAttackPressed => (!_isInputBlocked && !_isShopOpen) && _isAttackPressed;
    public bool IsSecondaryAttackPressed => (!_isInputBlocked && !_isShopOpen) && _isSecondaryAttackPressed;
    public bool IsSlidePressed => (!_isInputBlocked && !_isShopOpen) && _isSlidePressed;
    public bool IsLiftPressed => (!_isInputBlocked && !_isShopOpen) && _isLiftPressed;
    public bool IsDropHeroPressed => (!_isInputBlocked && !_isShopOpen) && _isDropPressed;
    public bool IsOpenMapPressed => (!_isInputBlocked && !_isShopOpen) && _isMapPressed;
    public bool IsMenuPressed => (!_isInputBlocked) && _isMenuPressed;
    public bool IsOpenShopOrChestPressed => (!_isInputBlocked) && _isOpenShopOrChestPressed;

    public void BlockInput(bool block) => _isInputBlocked = block;
    public void SetShopMode(bool isShopOpen) => _isShopOpen = isShopOpen;

    private void OnDestroy()
    {
        if (_jumpButton != null) _jumpButton.onClick.RemoveAllListeners();
        if (_attackButton != null) _attackButton.onClick.RemoveAllListeners();
        if (_secondaryAttackButton != null) _secondaryAttackButton.onClick.RemoveAllListeners();
        if (_slideButton != null) _slideButton.onClick.RemoveAllListeners();
        if (_liftButton != null) _liftButton.onClick.RemoveAllListeners();
        if (_dropButton != null) _dropButton.onClick.RemoveAllListeners();
        if (_mapButton != null) _mapButton.onClick.RemoveAllListeners();
        if (_menuButton != null) _menuButton.onClick.RemoveAllListeners();
        if (_shopOrChestButton != null) _shopOrChestButton.onClick.RemoveAllListeners();
    }
}
