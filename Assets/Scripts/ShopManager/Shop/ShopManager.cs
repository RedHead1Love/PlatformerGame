using System.Collections.Generic;
using UnityEngine;

namespace ShopLogic
{
    [RequireComponent(typeof(ShopItemPurchaseHandler))]
    public sealed class ShopManager : MonoBehaviour
    {
        [Header("Shop Content")]
        [SerializeField] private List<ShopItemData> _availableItems;

        [Header("UI References")]
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private GameObject _shopItemPrefab;

        private ShopItemPurchaseHandler _purchaseHandler;
        private readonly List<ShopItemView> _activeViews = new List<ShopItemView>();

        private void Awake()
        {
            _purchaseHandler = GetComponent<ShopItemPurchaseHandler>();
        }

        private void Start()
        {
            PopulateShop();
        }
    }

    private void HandleShopInput(Vector2 input)
    {
        if (_navigationController.TryMove(input))
        {
            OnNavigationChanged();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space) ||
            UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            TryPurchaseSelectedItem();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void OnNavigationChanged()
    {
        UpdateUI();
        UpdateDescription();
    }

    private void UpdateUI()
    {
        var currency = _navigationController.CurrentCurrency;

        _uiManager.SwitchCurrency(currency);

        _uiManager.UpdateItemSelection(currency, _navigationController.CurrentItemIndex);
    }

        private void PopulateShop()
        {
            if (_shopItemPrefab == null || _itemsContainer == null)
            {
                return;
            }

            ClearExistingItems();

            foreach (ShopItemData itemData in _availableItems)
            {
                CreateShopItem(itemData);
            }
        }

        private void ClearExistingItems()
        {
            foreach (Transform child in _itemsContainer)
            {
                Destroy(child.gameObject);
            }

            _activeViews.Clear();
        }

        private void CreateShopItem(ShopItemData itemData)
        {
            GameObject itemObject = Instantiate(_shopItemPrefab, _itemsContainer);
            ShopItemView view = itemObject.GetComponent<ShopItemView>();

            if (view != null)
            {
                view.Initialize(itemData, _purchaseHandler);
                _activeViews.Add(view);
            }
        }

        public void RefreshAllItems()
        {
            foreach (ShopItemView view in _activeViews)
            {
                view.RefreshState();
            }
        }
    }

    public void NavigateUp() => HandleShopInput(Vector2.up);
    public void NavigateDown() => HandleShopInput(Vector2.down);
    public void NavigateLeft() => HandleShopInput(Vector2.left);
    public void NavigateRight() => HandleShopInput(Vector2.right);
    public void ConfirmPurchase() => TryPurchaseSelectedItem();
}
}
