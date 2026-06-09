using GameLogic;
using Player.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopManager : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";

    [Header("UI Manager")]
    [SerializeField] private ShopUIManager _uiManager;

    [Header("Purchase Handler")]
    [SerializeField] private ShopItemPurchaseHandler _purchaseHandler;

    [Header("Close Button")]
    [SerializeField] private Button _closeButton;

    [Header("Input")]
    [SerializeField] private AggregatedInputProvider _inputProvider;
    private IInputProvider _input;

    [Header("Shop Items")]
    [SerializeField] private List<ShopItemData> _shopItems = new List<ShopItemData>();

    private ShopNavigationController _navigationController;
    private bool _isShopOpen;

    public List<ShopItemData> ShopItems => _shopItems;

    private void Start()
    {
        Initialize();
        LoadShopPurchases();
        InitializeShopSaveManager();
    }

    private void Update()
    {
        if (_isShopOpen == false)
        {
            return;
        }

        HandleShopInput();
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
        }
    }

    public void OpenShop()
    {
        _isShopOpen = true;

        gameObject.SetActive(true);

        EnsureInitialized();

        _navigationController.Reset();

        SetShopInputMode(true);
        UpdateUI();
        UpdateDescription();
    }

    public void CloseShop()
    {
        _isShopOpen = false;

        SetShopInputMode(false);

        gameObject.SetActive(false);
    }

    public void NavigateUp()
    {
        HandleShopInput(Vector2.up);
    }

    public void NavigateDown()
    {
        HandleShopInput(Vector2.down);
    }

    public void NavigateLeft()
    {
        HandleShopInput(Vector2.left);
    }

    public void NavigateRight()
    {
        HandleShopInput(Vector2.right);
    }

    public void ConfirmPurchase()
    {
        TryPurchaseSelectedItem();
    }

    private void Initialize()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
            _closeButton.onClick.AddListener(CloseShop);
        }

        if (_uiManager == null)
        {
            _uiManager = GetComponentInChildren<ShopUIManager>(true);
        }

        Hero hero = FindFirstObjectByType<Hero>();
        ArmorManager armorManager = FindFirstObjectByType<ArmorManager>();

        _purchaseHandler = new ShopItemPurchaseHandler(hero, armorManager, this);

        FindInputProvider();
        InitializeNavigation();
        InitializeItemViews();
    }

    private void EnsureInitialized()
    {
        if (_navigationController == null)
        {
            InitializeNavigation();
        }

        if (_uiManager == null)
        {
            _uiManager = GetComponentInChildren<ShopUIManager>(true);
        }

        if (_purchaseHandler == null)
        {
            Hero hero = FindFirstObjectByType<Hero>();
            ArmorManager armorManager = FindFirstObjectByType<ArmorManager>();

            _purchaseHandler = new ShopItemPurchaseHandler(hero, armorManager, this);
        }

        FindInputProvider();
    }

    private void FindInputProvider()
    {
        if (_inputProvider == null)
        {
            _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();
        }

        _input = _inputProvider;
    }

    private void InitializeShopSaveManager()
    {
        if (ShopSaveManager.Instance == null)
        {
            GameObject managerObject = new GameObject("ShopSaveManager");

            managerObject.AddComponent<ShopSaveManager>();
            DontDestroyOnLoad(managerObject);
        }

        ShopSaveManager.Instance?.LoadAllPurchases();
    }

    private void LoadShopPurchases()
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.CurrentSave == null)
        {
            return;
        }

        if (SaveSystem.Instance.CurrentSave.purchasedItemIds == null)
        {
            return;
        }

        foreach (string itemId in SaveSystem.Instance.CurrentSave.purchasedItemIds)
        {
            ShopItemData item = _shopItems.Find(shopItem => shopItem.ItemId == itemId);

            if (item != null && itemId != ShopItemIds.ActivateLastChance && itemId != ShopItemIds.RestoreArmor)
            {
                item.IsSold = true;
            }
        }

        _uiManager?.RefreshItemViews();
    }

    private void InitializeNavigation()
    {
        Dictionary<WalletManager.CoinType, List<IShopItem>> itemsByCurrency =
            new Dictionary<WalletManager.CoinType, List<IShopItem>>();

        foreach (ShopItemData item in _shopItems)
        {
            if (itemsByCurrency.ContainsKey(item.CurrencyType) == false)
            {
                itemsByCurrency[item.CurrencyType] = new List<IShopItem>();
            }

            itemsByCurrency[item.CurrencyType].Add(item);
        }

        _navigationController = new ShopNavigationController();

        foreach (KeyValuePair<WalletManager.CoinType, List<IShopItem>> pair in itemsByCurrency)
        {
            _navigationController.AddItems(pair.Key, pair.Value);
        }
    }

    private void InitializeItemViews()
    {
        if (_uiManager == null)
        {
            return;
        }

        foreach (ShopItemData item in _shopItems)
        {
            ShopItemView itemView = _uiManager.GetItemViewById(item.CurrencyType, item.ItemId);

            itemView?.Initialize(item);
        }
    }

    private void HandleShopInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw(HorizontalAxisName),
            Input.GetAxisRaw(VerticalAxisName));

        HandleShopInput(input);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            TryPurchaseSelectedItem();
        }

        if ((_input != null && _input.IsMenuPressed) || Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void HandleShopInput(Vector2 input)
    {
        if (_navigationController == null)
        {
            return;
        }

        if (_navigationController.TryMove(input))
        {
            OnNavigationChanged();
        }
    }

    private void OnNavigationChanged()
    {
        UpdateUI();
        UpdateDescription();
    }

    private void UpdateUI()
    {
        if (_uiManager == null || _navigationController == null)
        {
            return;
        }

        WalletManager.CoinType currency = _navigationController.CurrentCurrency;

        _uiManager.SwitchCurrency(currency);
        _uiManager.UpdateItemSelection(currency, _navigationController.CurrentItemIndex);
    }

    private void UpdateDescription()
    {
        if (_uiManager == null || _navigationController == null)
        {
            return;
        }

        IShopItem item = _navigationController.GetCurrentItem();

        if (item != null)
        {
            string status = item.IsSold && item.ItemId != ShopItemIds.RestoreArmor
                ? "<color=green>[Куплено]</color>\n"
                : string.Empty;

            _uiManager.UpdateDescription(status + item.Description);

            return;
        }

        _uiManager.UpdateDescription(GetCurrencyDescription(_navigationController.CurrentCurrency));
    }

    private void TryPurchaseSelectedItem()
    {
        IShopItem item = _navigationController?.GetCurrentItem();

        if (item == null)
        {
            return;
        }

        if (item.CanBePurchased() == false)
        {
            ShowCannotPurchaseMessage(item);

            return;
        }

        bool success = _purchaseHandler.TryPurchaseItem(item.ItemId);

        if (success == false)
        {
            _uiManager?.ShowPurchaseMessage("<color=red>Не удалось купить предмет</color>");

            return;
        }

        item.Purchase();

        _uiManager?.ShowPurchaseMessage("Покупка успешна");
        _uiManager?.RefreshLastChanceItems();

        UpdateUI();
        UpdateDescription();
        SaveGameImmediately();
    }

    private void ShowCannotPurchaseMessage(IShopItem item)
    {
        if (item.IsSold && item.ItemId != ShopItemIds.RestoreArmor)
        {
            _uiManager?.ShowPurchaseMessage("<color=yellow>этот предмет уже куплен</color>");
        }
        else
        {
            _uiManager?.ShowPurchaseMessage("<color=red>Недостаточно средств или покупка недоступна</color>");
        }
    }

    private void SaveGameImmediately()
    {
        Hero hero = FindFirstObjectByType<Hero>();

        if (hero == null || SaveSystem.Instance == null)
        {
            return;
        }

        if (hero.AbilityManager != null)
        {
            SaveSystem.Instance.UpdateAbilityData(hero.AbilityManager);
        }

        SaveSystem.Instance.SaveGame(string.Empty, hero.transform.position);
    }

    private string GetCurrencyDescription(WalletManager.CoinType coinType)
    {
        return coinType switch
        {
            WalletManager.CoinType.Bronze => "Бронзовые монеты - базовая валюта\n↓ Выбрать предмет",
            WalletManager.CoinType.Silver => "Серебряные монеты - редкая валюта\n↓ Выбрать предмет",
            WalletManager.CoinType.Gold => "Золотые монеты - ценная валюта\n↓ Выбрать предмет",
            _ => string.Empty
        };
    }

    private void SetShopInputMode(bool isShopOpen)
    {
        if (_inputProvider == null)
        {
            _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();
        }

        _inputProvider?.SetShopMode(isShopOpen);
    }
}