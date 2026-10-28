using GameLogic;
using UnityEngine;

public sealed class ShopSelectionPresenter
{
    private readonly ShopUIManager _uiManager;
    private readonly ShopNavigationController _navigationController;

    public ShopSelectionPresenter(ShopUIManager view, ShopNavigationController navigation)
    {
        _uiManager = view;
        _navigationController = navigation;
    }

    public void UpdateUI()
    {
        if (_uiManager == null || _navigationController == null)
        {
            return;
        }

        WalletManager.CoinType currency = _navigationController.CurrentCurrency;

        _uiManager.SwitchCurrency(currency);
        _uiManager.UpdateItemSelection(currency, _navigationController.CurrentItemIndex);
    }

    public void UpdateDetailsPanel()
    {
        if (_uiManager == null || _navigationController == null)
        {
            return;
        }

        IShopItem item = _navigationController.GetCurrentItem();

        if (item != null)
        {
            ShopItemView view = _uiManager.GetItemViewById(_navigationController.CurrentCurrency, item.ItemId);
            Sprite icon = view != null ? view.ItemIcon : null;

            string itemName = ShopItemNames.GetItemName(item.ItemId);
            string itemDesc = ShopItemDescriptions.GetItemDescription(item.ItemId);

            bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;
            string boughtText = isEnglish ? "[Bought]" : "[Куплено]";

            string statusMessage = item.IsSold && item.ItemId != ShopItemIds.RestoreArmor
                ? $"<color=green>{boughtText}</color>\n" + itemDesc
                : itemDesc;

            _uiManager.UpdateRightPanel(item, icon, itemName, statusMessage);
        }
        else
        {
            _uiManager.UpdateRightPanel(null, null, null, GetCurrencyDescription(_navigationController.CurrentCurrency));
        }
    }

    public void ShowCannotPurchaseMessage(IShopItem item)
    {
        bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;

        if (item.IsSold && item.ItemId != ShopItemIds.RestoreArmor)
        {
            _uiManager?.ShowPurchaseMessage(isEnglish ? "<color=yellow>Item already bought</color>" : "<color=yellow>Этот предмет уже куплен</color>");
        }
        else
        {
            _uiManager?.ShowPurchaseMessage(isEnglish ? "<color=red>Not enough funds or locked</color>" : "<color=red>Недостаточно средств или покупка недоступна</color>");
        }
    }

    private string GetCurrencyDescription(WalletManager.CoinType coinType)
    {
        return string.Empty;
    }
}
