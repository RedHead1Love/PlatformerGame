using GameLogic;
using System.Collections.Generic;

public sealed class ShopCatalog
{
    private readonly List<ShopItemData> _items;

    public ShopCatalog(List<ShopItemData> items)
    {
        _items = items ?? new List<ShopItemData>();
    }

    public Dictionary<WalletManager.CoinType, List<ShopItemData>> GroupByCurrency()
    {
        var groups = new Dictionary<WalletManager.CoinType, List<ShopItemData>>();
        foreach (ShopItemData item in _items)
        {
            if (item == null)
            {
                continue;
            }
            if (!groups.TryGetValue(item.CurrencyType, out List<ShopItemData> group))
            {
                group = new List<ShopItemData>();
                groups.Add(item.CurrencyType, group);
            }
            group.Add(item);
        }
        return groups;
    }

    public ShopNavigationController CreateNavigation()
    {
        var navigation = new ShopNavigationController();
        foreach (var group in GroupByCurrency())
        {
            navigation.AddItems(group.Key, new List<IShopItem>(group.Value));
        }
        return navigation;
    }

    public void RestorePurchases(IEnumerable<string> purchasedIds)
    {
        if (purchasedIds == null)
        {
            return;
        }
        foreach (string id in purchasedIds)
        {
            if (id == ShopItemIds.ActivateLastChance || id == ShopItemIds.RestoreArmor)
            {
                continue;
            }
            ShopItemData item = _items.Find(candidate => candidate != null && candidate.ItemId == id);
            if (item != null)
            {
                item.IsSold = true;
            }
        }
    }
}
