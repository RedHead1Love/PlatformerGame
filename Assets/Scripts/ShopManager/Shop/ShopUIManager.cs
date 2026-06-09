using GameLogic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopUIManager : MonoBehaviour
{
    [Header("Currency Panels")]
    [SerializeField] private GameObject _bronzeMenuPanel;
    [SerializeField] private GameObject _silverMenuPanel;
    [SerializeField] private GameObject _goldMenuPanel;

    [Header("Currency Indicators")]
    [SerializeField] private Image _bronzeCoinIndicator;
    [SerializeField] private Image _silverCoinIndicator;
    [SerializeField] private Image _goldCoinIndicator;
    [SerializeField] private Color _selectedCoinColor = Color.white;
    [SerializeField] private Color _unselectedCoinColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI _currentCurrencyText;
    [SerializeField] private Image _currentCurrencyIcon;

    [Header("Description")]
    [SerializeField] private TextMeshProUGUI _selectedItemDescription;

    private readonly Dictionary<WalletManager.CoinType, List<ShopItemView>> _itemViewsByCurrency =
        new Dictionary<WalletManager.CoinType, List<ShopItemView>>();

    private WalletManager.CoinType _currentCurrency = WalletManager.CoinType.Bronze;
    private bool _isInitialized;

    private void Start()
    {
        Initialize();
    }

    public void SwitchCurrency(WalletManager.CoinType currencyType)
    {
        Initialize();

        _currentCurrency = currencyType;

        UpdateCurrencyDisplay();
        UpdateCoinIndicators();
        ShowCurrencyMenu(currencyType);
    }

    public void ShowCurrencyMenu(WalletManager.CoinType currencyType)
    {
        HideAllMenus();

        GameObject menu = GetPanel(currencyType);

        if (menu != null)
        {
            menu.SetActive(true);
        }
    }

    public void UpdateItemSelection(WalletManager.CoinType currency, int selectedIndex)
    {
        Initialize();
        ResetAllItemSelection();

        if (_itemViewsByCurrency.TryGetValue(currency, out List<ShopItemView> itemViews) == false)
        {
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= itemViews.Count)
        {
            return;
        }

        ShopItemView selectedView = itemViews[selectedIndex];

        if (selectedView == null)
        {
            return;
        }

        bool canPurchase = selectedView.ItemData != null && selectedView.ItemData.CanBePurchased();

        selectedView.SetSelected(true, canPurchase);
    }

    public ShopItemView GetItemView(WalletManager.CoinType currency, int index)
    {
        Initialize();

        if (_itemViewsByCurrency.TryGetValue(currency, out List<ShopItemView> itemViews) == false)
        {
            return null;
        }

        return index >= 0 && index < itemViews.Count ? itemViews[index] : null;
    }

    public ShopItemView GetItemViewById(WalletManager.CoinType currency, string itemId)
    {
        Initialize();

        if (_itemViewsByCurrency.TryGetValue(currency, out List<ShopItemView> itemViews) == false)
        {
            return null;
        }

        foreach (ShopItemView itemView in itemViews)
        {
            if (itemView != null && itemView.ItemData != null && itemView.ItemData.ItemId == itemId)
            {
                return itemView;
            }
        }

        return null;
    }

    public void UpdateDescription(string description)
    {
        if (_selectedItemDescription != null)
        {
            _selectedItemDescription.text = description;
        }
    }

    public void ShowPurchaseMessage(string message)
    {
        if (_selectedItemDescription != null)
        {
            _selectedItemDescription.text = $"<color=green>✓ {message}</color>";
        }
    }

    public void RefreshItemViews()
    {
        _isInitialized = false;

        Initialize();
    }

    public void LogCurrentState() { }

    public void RefreshLastChanceItems()
    {
        Initialize();

        foreach (List<ShopItemView> viewList in _itemViewsByCurrency.Values)
        {
            foreach (ShopItemView view in viewList)
            {
                if (view != null && view.ItemData != null && view.ItemData.ItemId == ShopItemIds.ActivateLastChance)
                {
                    view.UpdateView();
                }
            }
        }
    }

    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _itemViewsByCurrency.Clear();

        foreach (WalletManager.CoinType currency in System.Enum.GetValues(typeof(WalletManager.CoinType)))
        {
            _itemViewsByCurrency[currency] = new List<ShopItemView>();
        }

        FindAllItemViews();

        _isInitialized = true;
    }

    private void FindAllItemViews()
    {
        FindItemViewsInPanel(_bronzeMenuPanel, WalletManager.CoinType.Bronze);
        FindItemViewsInPanel(_silverMenuPanel, WalletManager.CoinType.Silver);
        FindItemViewsInPanel(_goldMenuPanel, WalletManager.CoinType.Gold);
    }

    private void FindItemViewsInPanel(GameObject panel, WalletManager.CoinType currencyType)
    {
        if (panel == null)
        {
            return;
        }

        ShopItemView[] itemViews = panel.GetComponentsInChildren<ShopItemView>(true);

        foreach (ShopItemView itemView in itemViews)
        {
            if (itemView != null)
            {
                _itemViewsByCurrency[currencyType].Add(itemView);
            }
        }
    }

    private void ResetAllItemSelection()
    {
        foreach (List<ShopItemView> itemViews in _itemViewsByCurrency.Values)
        {
            foreach (ShopItemView itemView in itemViews)
            {
                itemView?.SetSelected(false, true);
            }
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (WalletManager.Instance == null || _currentCurrencyText == null)
        {
            return;
        }

        _currentCurrencyText.text = WalletManager.Instance.GetCoins(_currentCurrency).ToString();

        if (_currentCurrencyIcon != null)
        {
            _currentCurrencyIcon.sprite = GetCurrencySprite(_currentCurrency);
        }
    }

    private void UpdateCoinIndicators()
    {
        SetIndicatorColor(_bronzeCoinIndicator, WalletManager.CoinType.Bronze);
        SetIndicatorColor(_silverCoinIndicator, WalletManager.CoinType.Silver);
        SetIndicatorColor(_goldCoinIndicator, WalletManager.CoinType.Gold);
    }

    private void SetIndicatorColor(Image indicator, WalletManager.CoinType currency)
    {
        if (indicator != null)
        {
            indicator.color = _currentCurrency == currency ? _selectedCoinColor : _unselectedCoinColor;
        }
    }

    private void HideAllMenus()
    {
        if (_bronzeMenuPanel != null)
        {
            _bronzeMenuPanel.SetActive(false);
        }

        if (_silverMenuPanel != null)
        {
            _silverMenuPanel.SetActive(false);
        }

        if (_goldMenuPanel != null)
        {
            _goldMenuPanel.SetActive(false);
        }
    }

    private GameObject GetPanel(WalletManager.CoinType currencyType)
    {
        return currencyType switch
        {
            WalletManager.CoinType.Bronze => _bronzeMenuPanel,
            WalletManager.CoinType.Silver => _silverMenuPanel,
            WalletManager.CoinType.Gold => _goldMenuPanel,
            _ => null
        };
    }

    private Sprite GetCurrencySprite(WalletManager.CoinType coinType)
    {
        return coinType switch
        {
            WalletManager.CoinType.Bronze => _bronzeCoinIndicator != null ? _bronzeCoinIndicator.sprite : null,
            WalletManager.CoinType.Silver => _silverCoinIndicator != null ? _silverCoinIndicator.sprite : null,
            WalletManager.CoinType.Gold => _goldCoinIndicator != null ? _goldCoinIndicator.sprite : null,
            _ => null
        };
    }
}