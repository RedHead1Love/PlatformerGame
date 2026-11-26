using System;
using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foxberry.Shop
{
    internal sealed class ShopItemDetailsView
    {
        private readonly TextMeshProUGUI _detailsNameText;
        private readonly GameObject _detailsTitleDivider;
        private readonly TextMeshProUGUI _detailsDescriptionText;
        private readonly Image _detailsItemIcon;
        private readonly GameObject _purchaseFrameObject;
        private readonly GameObject _priceTextObject;
        private readonly GameObject _priceIconObject;
        private readonly GameObject _buyButtonObject;
        private readonly TextMeshProUGUI _detailsPriceText;
        private readonly Image _detailsPriceCurrencyIcon;
        private readonly Button _buyButton;
        private readonly Func<CoinType, Color> GetCurrencyColor;
        private readonly Func<CoinType, Sprite> GetCurrencySprite;

        public ShopItemDetailsView(TextMeshProUGUI detailsNameText,
             GameObject detailsTitleDivider,
             TextMeshProUGUI detailsDescriptionText,
             Image detailsItemIcon,
             GameObject purchaseFrameObject,
             GameObject priceTextObject,
             GameObject priceIconObject,
             GameObject buyButtonObject,
             TextMeshProUGUI detailsPriceText,
             Image detailsPriceCurrencyIcon,
             Button buyButton,
             Func<CoinType, Color> color,
             Func<CoinType, Sprite> sprite)
        {
            _detailsNameText = detailsNameText;
            _detailsTitleDivider = detailsTitleDivider;
            _detailsDescriptionText = detailsDescriptionText;
            _detailsItemIcon = detailsItemIcon;
            _purchaseFrameObject = purchaseFrameObject;
            _priceTextObject = priceTextObject;
            _priceIconObject = priceIconObject;
            _buyButtonObject = buyButtonObject;
            _detailsPriceText = detailsPriceText;
            _detailsPriceCurrencyIcon = detailsPriceCurrencyIcon;
            _buyButton = buyButton;
            GetCurrencyColor = color;
            GetCurrencySprite = sprite;
        }

        public void UpdateRightPanel(IShopItem item, Sprite icon, string customName = null, string customMessage = null)
        {
            if (item == null)
            {
                ClearRightPanel(customMessage);
                return;
            }

            if (_purchaseFrameObject != null)
            {
                _purchaseFrameObject.SetActive(true);
            }

            if (_priceTextObject != null)
            {
                _priceTextObject.SetActive(true);
            }

            if (_priceIconObject != null)
            {
                _priceIconObject.SetActive(true);
            }

            if (_buyButtonObject != null)
            {
                _buyButtonObject.SetActive(true);
            }

            if (_detailsNameText != null)
            {
                _detailsNameText.text = !string.IsNullOrEmpty(customName)
                    ? customName
                    : item.DisplayName;
            }

            if (_detailsTitleDivider != null)
            {
                _detailsTitleDivider.SetActive(true);
            }

            if (_detailsDescriptionText != null)
            {
                _detailsDescriptionText.text = customMessage ?? item.Description;
            }

            if (_detailsItemIcon != null)
            {
                _detailsItemIcon.sprite = icon;
                _detailsItemIcon.gameObject.SetActive(icon != null);
            }

            if (_detailsPriceText != null)
            {
                bool isEnglish = LocalizationManager.CurrentLanguage == GameLanguage.English;
                if (item.ItemId == ShopItemIds.ActivateLastChance && item.IsSold)
                {
                    _detailsPriceText.text = isEnglish
                        ? "Active"
                        : "Aктивно";
                    _detailsPriceText.color = Color.yellow;
                    if (_priceIconObject != null)
                    {
                        _priceIconObject.SetActive(false);
                    }
                }
                else if (item.IsSold && item.ItemId != ShopItemIds.RestoreArmor)
                {
                    _detailsPriceText.text = isEnglish
                        ? "Bought"
                        : "Куплено";
                    _detailsPriceText.color = Color.green;
                    if (_priceIconObject != null)
                    {
                        _priceIconObject.SetActive(false);
                    }
                }
                else
                {
                    _detailsPriceText.text = $"{item.Price}";
                    _detailsPriceText.color = GetCurrencyColor(item.CurrencyType);
                    if (_detailsPriceCurrencyIcon != null)
                    {
                        _detailsPriceCurrencyIcon.sprite = GetCurrencySprite(item.CurrencyType);
                        if (_priceIconObject != null)
                        {
                            _priceIconObject.SetActive(true);
                        }
                    }
                }
            }

            if (_buyButton != null)
            {
                _buyButton.interactable = item.CanBePurchased();
            }
        }

        public void ShowPurchaseMessage(string message)
        {
            if (_detailsDescriptionText != null)
            {
                _detailsDescriptionText.text = $"<color=green>✓ {message}</color>";
            }
        }

        private void ClearRightPanel(string message)
        {
            if (_purchaseFrameObject != null)
            {
                _purchaseFrameObject.SetActive(false);
            }

            if (_priceTextObject != null)
            {
                _priceTextObject.SetActive(false);
            }

            if (_priceIconObject != null)
            {
                _priceIconObject.SetActive(false);
            }

            if (_buyButtonObject != null)
            {
                _buyButtonObject.SetActive(false);
            }

            if (_detailsNameText != null)
            {
                _detailsNameText.text = string.Empty;
            }

            if (_detailsTitleDivider != null)
            {
                _detailsTitleDivider.SetActive(false);
            }

            if (_detailsDescriptionText != null)
            {
                _detailsDescriptionText.text = message ?? string.Empty;
            }

            if (_detailsItemIcon != null)
            {
                _detailsItemIcon.gameObject.SetActive(false);
            }
        }
    }
}
