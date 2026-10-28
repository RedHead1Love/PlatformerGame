using GameLogic;
using Player.Input;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public sealed class ShopManager : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";

    [SerializeField] private ShopUIManager _uiManager;

    private ShopItemPurchaseHandler _purchaseHandler;

    [SerializeField] private Button _closeButton;

    [SerializeField] private AggregatedInputProvider _inputProvider;
    private IInputProvider _input;
    private readonly List<ShopItemView> _boundViews = new List<ShopItemView>();

    [SerializeField] private List<ShopItemData> _shopItems = new List<ShopItemData>();

    [SerializeField] private MiniMapController _miniMapController;

    private ShopNavigationController _navigationController;
    private bool _isShopOpen;

    private ShopSelectionPresenter _presenter;

    public bool IsShopOpen => _isShopOpen;

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
        UnbindItemViews();
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
        }

        if (_uiManager != null)
        {
            _uiManager.OnCurrencyTabClicked -= HandleCurrencyTabClick;

            if (_uiManager.BuyButton != null)
            {
                _uiManager.BuyButton.onClick.RemoveListener(ConfirmPurchase);
            }
        }
    }

    public void OpenShop()
    {
        _isShopOpen = true;

        gameObject.SetActive(true);

        EnsureInitialized();

        _miniMapController?.CloseMiniMap();

        _navigationController.Reset();

        SetShopInputMode(true);
        _presenter.UpdateUI();
        _presenter.UpdateDetailsPanel();
    }

    public void CloseShop()
    {
        _isShopOpen = false;

        SetShopInputMode(false);

        gameObject.SetActive(false);
    }

    public void NavigateUp() => HandleShopInput(Vector2.up);
    public void NavigateDown() => HandleShopInput(Vector2.down);
    public void NavigateLeft() => HandleShopInput(Vector2.left);
    public void NavigateRight() => HandleShopInput(Vector2.right);
    public void ConfirmPurchase() => TryPurchaseSelectedItem();

    private void Initialize()
    {
        if (_miniMapController == null)
        {
            _miniMapController = FindFirstObjectByType<MiniMapController>();
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
            _closeButton.onClick.AddListener(CloseShop);
        }

        if (_uiManager == null)
        {
            _uiManager = GetComponentInChildren<ShopUIManager>(true);
        }

        if (_uiManager != null && _uiManager.BuyButton != null)
        {
            _uiManager.BuyButton.onClick.RemoveListener(ConfirmPurchase);
            _uiManager.BuyButton.onClick.AddListener(ConfirmPurchase);
        }

        Hero hero = FindFirstObjectByType<Hero>();
        ArmorManager armorManager = FindFirstObjectByType<ArmorManager>();

        _purchaseHandler = new ShopItemPurchaseHandler(hero, armorManager, this);

        if (_uiManager == null)
        {
            _uiManager = GetComponentInChildren<ShopUIManager>(true);
        }

        if (_uiManager != null)
        {
            _uiManager.OnCurrencyTabClicked -= HandleCurrencyTabClick;
            _uiManager.OnCurrencyTabClicked += HandleCurrencyTabClick;

            if (_uiManager.BuyButton != null)
            {
                _uiManager.BuyButton.onClick.RemoveListener(ConfirmPurchase);
                _uiManager.BuyButton.onClick.AddListener(ConfirmPurchase);
            }
        }

        FindInputProvider();
        InitializeNavigation();
        InitializeItemViews();
    }

    private void EnsureInitialized()
    {
        if (_miniMapController == null)
        {
            _miniMapController = FindFirstObjectByType<MiniMapController>();
        }

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

        _presenter = new ShopSelectionPresenter(_uiManager, _navigationController);
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
        new ShopCatalog(_shopItems).RestorePurchases(SaveSystem.Instance?.CurrentSave?.PurchasedItemIds);
        _uiManager?.RefreshItemViews();
    }

    private void InitializeNavigation()
    {
        _navigationController = new ShopCatalog(_shopItems).CreateNavigation();
        _presenter = new ShopSelectionPresenter(_uiManager, _navigationController);
    }

    private void InitializeItemViews()
    {
        UnbindItemViews();
        if (_uiManager == null)
        {
            return;
        }

        var itemsByCurrency = new ShopCatalog(_shopItems).GroupByCurrency();

        foreach (KeyValuePair<WalletManager.CoinType, List<ShopItemData>> kvp in itemsByCurrency)
        {
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                ShopItemView itemView = _uiManager.GetItemView(kvp.Key, i);

                if (itemView != null)
                {
                    itemView.Initialize(kvp.Value[i]);

                    itemView.OnPointerSelected -= HandlePointerSelection;
                    itemView.OnPointerSelected += HandlePointerSelection;
                    _boundViews.Add(itemView);
                }
            }
        }
    }

    private void UnbindItemViews()
    {
        foreach (ShopItemView view in _boundViews)
        {
            if (view != null)
            {
                view.OnPointerSelected -= HandlePointerSelection;
            }
        }
        _boundViews.Clear();
    }

    private void HandlePointerSelection(IShopItem selectedItem)
    {
        if (_navigationController == null || selectedItem == null)
        {
            return;
        }

        if (_navigationController.TrySetSelectedItem(selectedItem))
        {
            OnNavigationChanged();
        }
    }

    private void HandleCurrencyTabClick(WalletManager.CoinType currency)
    {
        if (_navigationController != null && _navigationController.TrySetCurrency(currency))
        {
            OnNavigationChanged();
        }
    }

    private void HandleShopInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw(HorizontalAxisName), Input.GetAxisRaw(VerticalAxisName));
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
        _presenter.UpdateUI();
        _presenter.UpdateDetailsPanel();
    }

    private void TryPurchaseSelectedItem()
    {
        IShopItem item = _navigationController?.GetCurrentItem();
        if (item == null) return;

        if (item.CanBePurchased() == false)
        {
            _presenter.ShowCannotPurchaseMessage(item);
            return;
        }

        bool success = _purchaseHandler.TryPurchaseItem(item.ItemId);
        bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;

        if (success == false)
        {
            _uiManager?.ShowPurchaseMessage(isEnglish ? "<color=red>Failed to purchase</color>" : "<color=red>Не удалось купить предмет</color>");
            return;
        }

        item.Purchase();

        _uiManager?.ShowPurchaseMessage(isEnglish ? "Purchase successful" : "Покупка успешна");
        _uiManager?.RefreshLastChanceItems();

        _presenter.UpdateUI();
        _presenter.UpdateDetailsPanel();
        SaveGameImmediately();
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

    private void SetShopInputMode(bool isShopOpen)
    {
        if (_inputProvider == null)
        {
            _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();
        }

        _inputProvider?.SetShopMode(isShopOpen);
    }
}
