using GameLogic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopMobileAdapter : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button _upButton;
    [SerializeField] private Button _downButton;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("Action Buttons")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _closeButton;

    [Header("Settings")]
    [SerializeField] private float _moveDelay = 0.2f;

    [SerializeField] private ShopNavigationController _navigation;

    private float _lastMoveTime;

    public System.Action<IShopItem> OnItemSelected;
    public System.Action<WalletManager.CoinType> OnCurrencyChanged;
    public System.Action OnBuyRequested;
    public System.Action OnCloseRequested;

    public ShopNavigationController Navigation => _navigation;

    private void Awake()
    {
        _navigation = new ShopNavigationController(_moveDelay);
    }

    private void Start()
    {
        SubscribeButtons();
    }

    private void OnDestroy()
    {
        UnsubscribeButtons();
    }

    private void SubscribeButtons()
    {
        _upButton?.onClick.AddListener(NavigateUp);
        _downButton?.onClick.AddListener(NavigateDown);
        _leftButton?.onClick.AddListener(NavigateLeft);
        _rightButton?.onClick.AddListener(NavigateRight);
        _buyButton?.onClick.AddListener(RequestBuy);
        _closeButton?.onClick.AddListener(RequestClose);
    }

    private void UnsubscribeButtons()
    {
        _upButton?.onClick.RemoveListener(NavigateUp);
        _downButton?.onClick.RemoveListener(NavigateDown);
        _leftButton?.onClick.RemoveListener(NavigateLeft);
        _rightButton?.onClick.RemoveListener(NavigateRight);
        _buyButton?.onClick.RemoveListener(RequestBuy);
        _closeButton?.onClick.RemoveListener(RequestClose);
    }

    private void NavigateUp() => Navigate(Vector2.up);
    private void NavigateDown() => Navigate(Vector2.down);
    private void NavigateLeft() => Navigate(Vector2.left);
    private void NavigateRight() => Navigate(Vector2.right);

    private void RequestBuy()
    {
        OnBuyRequested?.Invoke();
    }

    private void RequestClose()
    {
        OnCloseRequested?.Invoke();
    }

    private void Navigate(Vector2 direction)
    {
        if (Time.time - _lastMoveTime < _moveDelay)
        {
            return;
        }

        if (_navigation.TryMove(direction) == false)
        {
            return;
        }

        _lastMoveTime = Time.time;

        IShopItem currentItem = _navigation.GetCurrentItem();

        if (currentItem != null)
        {
            OnItemSelected?.Invoke(currentItem);
        }
        else
        {
            OnCurrencyChanged?.Invoke(_navigation.CurrentCurrency);
        }
    }
}