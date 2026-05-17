using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMobileAdapter : MonoBehaviour
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

    // События для основного скрипта магазина
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

    private void SubscribeButtons()
    {
        // Навигационные кнопки
        if (_upButton != null)
            _upButton.onClick.AddListener(() => Navigate(Vector2.up));

        if (_downButton != null)
            _downButton.onClick.AddListener(() => Navigate(Vector2.down));

        if (_leftButton != null)
            _leftButton.onClick.AddListener(() => Navigate(Vector2.left));

        if (_rightButton != null)
            _rightButton.onClick.AddListener(() => Navigate(Vector2.right));

        // Действия
        if (_buyButton != null)
            _buyButton.onClick.AddListener(() => OnBuyRequested?.Invoke());

        if (_closeButton != null)
            _closeButton.onClick.AddListener(() => OnCloseRequested?.Invoke());
    }

    private void Navigate(Vector2 direction)
    {
        if (Time.time - _lastMoveTime < _moveDelay)
            return;

        bool moved = _navigation.TryMove(direction);

        if (moved)
        {
            _lastMoveTime = Time.time;

            // Уведомляем об изменениях
            var currentItem = _navigation.GetCurrentItem();
            var currentCurrency = _navigation.CurrentCurrency;

            if (currentItem != null)
                OnItemSelected?.Invoke(currentItem);
            else
                OnCurrencyChanged?.Invoke(currentCurrency);
        }
    }

    // Методы для добавления предметов
    public void AddItems(WalletManager.CoinType currency, List<IShopItem> items)
    {
        _navigation.AddItems(currency, items);
    }

    public IShopItem GetCurrentItem()
    {
        return _navigation.GetCurrentItem();
    }

    public WalletManager.CoinType GetCurrentCurrency()
    {
        return _navigation.CurrentCurrency;
    }

    public void ResetNavigation()
    {
        _navigation.Reset();
    }
}
