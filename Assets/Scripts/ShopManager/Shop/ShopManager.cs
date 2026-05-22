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
}
