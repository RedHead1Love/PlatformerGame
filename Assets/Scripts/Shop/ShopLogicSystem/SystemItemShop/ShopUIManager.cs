using System;
using System.Collections.Generic;
using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Foxberry.Shop
{
    public sealed class ShopUIManager : MonoBehaviour
    {
        private readonly Dictionary<CoinType, List<ShopItemView>> _itemViewsByCurrency = new Dictionary<CoinType, List<ShopItemView>>();
        [SerializeField]
        private GameObject _bronzeMenuPanel;
        [SerializeField]
        private GameObject _silverMenuPanel;
        [SerializeField]
        private GameObject _goldMenuPanel;
        [SerializeField]
        private GameObject _bronzeTabArea;
        [SerializeField]
        private GameObject _silverTabArea;
        [SerializeField]
        private GameObject _goldTabArea;
        [SerializeField]
        private Sprite _bronzeCoinSprite;
        [SerializeField]
        private Sprite _silverCoinSprite;
        [SerializeField]
        private Sprite _goldCoinSprite;
        [SerializeField]
        private Color _selectedCoinColor = Color.white;
        [SerializeField]
        private Color _unselectedCoinColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField]
        private Color _bronzeTextColor = new Color(0.8f, 0.5f, 0.2f);
        [SerializeField]
        private Color _silverTextColor = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField]
        private Color _goldTextColor = new Color(1f, 0.84f, 0f);
        [SerializeField]
        private TextMeshProUGUI _currentCurrencyText;
        [SerializeField]
        private Image _currentCurrencyIcon;
        [SerializeField]
        private TextMeshProUGUI _detailsNameText;
        [SerializeField]
        private GameObject _detailsTitleDivider;
        [SerializeField]
        private TextMeshProUGUI _detailsDescriptionText;
        [SerializeField]
        private Image _detailsItemIcon;
        [SerializeField]
        private GameObject _purchaseFrameObject;
        [SerializeField]
        private GameObject _priceTextObject;
        [SerializeField]
        private GameObject _priceIconObject;
        [SerializeField]
        private GameObject _buyButtonObject;
        [SerializeField]
        private TextMeshProUGUI _detailsPriceText;
        [SerializeField]
        private Image _detailsPriceCurrencyIcon;
        [SerializeField]
        private Button _buyButton;
        private CoinType _currentCurrency = CoinType.Bronze;
        private bool _isInitialized;
        private ShopItemDetailsView _details;

        public event Action<CoinType> OnCurrencyTabClicked;

        private ShopItemDetailsView Details => _details ??= new ShopItemDetailsView(_detailsNameText,
             _detailsTitleDivider,
             _detailsDescriptionText,
             _detailsItemIcon,
             _purchaseFrameObject,
             _priceTextObject,
             _priceIconObject,
             _buyButtonObject,
             _detailsPriceText,
             _detailsPriceCurrencyIcon,
             _buyButton,
             GetCurrencyColor,
             GetCurrencySprite);

        public Button BuyButton => _buyButton;

        public void UpdateRightPanel(IShopItem item, Sprite icon, string customName = null, string customMessage = null)
        {
            Details.UpdateRightPanel(item, icon, customName, customMessage);
        }

        public void ShowPurchaseMessage(string message) => Details.ShowPurchaseMessage(message);

        public void SwitchCurrency(CoinType currencyType)
        {
            Initialize();
            _currentCurrency = currencyType;
            UpdateCurrencyDisplay();
            ShowCurrencyMenu(currencyType);
        }

        public void ShowCurrencyMenu(CoinType currencyType)
        {
            HideAllMenus();
            GameObject menu = GetPanel(currencyType);
            if (menu != null)
            {
                menu.SetActive(true);
            }
        }

        public void UpdateItemSelection(CoinType currency, int selectedIndex)
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

        public ShopItemView GetItemView(CoinType currency, int index)
        {
            Initialize();
            if (_itemViewsByCurrency.TryGetValue(currency, out List<ShopItemView> itemViews) == false)
            {
                return null;
            }

            return index >= 0 && index < itemViews.Count
                ? itemViews[index]
                : null;
        }

        public ShopItemView GetItemViewById(CoinType currency, string itemId)
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

        public void RefreshItemViews()
        {
            _isInitialized = false;
            Initialize();
        }

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

        private void Start()
        {
            Initialize();
            SetupCurrencyClickHandlers();
        }

        private void SetupCurrencyClickHandlers()
        {
            BindCurrencyClick(_bronzeTabArea, CoinType.Bronze);
            BindCurrencyClick(_silverTabArea, CoinType.Silver);
            BindCurrencyClick(_goldTabArea, CoinType.Gold);
        }

        private void BindCurrencyClick(GameObject tabObject, CoinType currency)
        {
            if (tabObject == null)
            {
                return;
            }

            EventTrigger trigger = tabObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = tabObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry clickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            clickEntry.callback.AddListener((data) =>
            {
                PointerEventData pointerData = data as PointerEventData;
                if (pointerData != null && pointerData.button == PointerEventData.InputButton.Left)
                {
                    OnCurrencyTabClicked?.Invoke(currency);
                }
            });
            trigger.triggers.Add(clickEntry);
        }

        private void SetDetailsCurrencyIconActive(bool isActive)
        {
            if (_detailsPriceCurrencyIcon != null)
            {
                _detailsPriceCurrencyIcon.gameObject.SetActive(isActive);
            }
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _itemViewsByCurrency.Clear();
            foreach (CoinType currency in System.Enum.GetValues(typeof(CoinType)))
            {
                _itemViewsByCurrency[currency] = new List<ShopItemView>();
            }

            FindAllItemViews();
            _isInitialized = true;
        }

        private void FindAllItemViews()
        {
            FindItemViewsInPanel(_bronzeMenuPanel, CoinType.Bronze);
            FindItemViewsInPanel(_silverMenuPanel, CoinType.Silver);
            FindItemViewsInPanel(_goldMenuPanel, CoinType.Gold);
        }

        private void FindItemViewsInPanel(GameObject panel, CoinType currencyType)
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

        private void SetIndicatorColor(Image indicator, CoinType currency)
        {
            if (indicator != null)
            {
                indicator.color = _currentCurrency == currency
                    ? _selectedCoinColor
                    : _unselectedCoinColor;
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

        private GameObject GetPanel(CoinType currencyType)
        {
            return currencyType switch
            {
                CoinType.Bronze => _bronzeMenuPanel,
                CoinType.Silver => _silverMenuPanel,
                CoinType.Gold => _goldMenuPanel,
                _ => null,
            };
        }

        private Sprite GetCurrencySprite(CoinType coinType)
        {
            return coinType switch
            {
                CoinType.Bronze => _bronzeCoinSprite,
                CoinType.Silver => _silverCoinSprite,
                CoinType.Gold => _goldCoinSprite,
                _ => null,
            };
        }

        private Color GetCurrencyColor(CoinType coinType)
        {
            return coinType switch
            {
                CoinType.Bronze => _bronzeTextColor,
                CoinType.Silver => _silverTextColor,
                CoinType.Gold => _goldTextColor,
                _ => Color.white,
            };
        }
    }
}
