using GameLogic;
using Player;
using Player.Abilities;
using UnityEngine;

namespace Foxberry.Shop
{
    public sealed class ShopItemPurchaseHandler
    {
        private readonly Hero _hero;
        private readonly AbilityManager _abilityManager;

        private readonly ShopItemEffects _effects;
        private readonly System.Func<IWalletManager> _getWallet;
        private ShopManager _shopManager;

        public ShopItemPurchaseHandler(
            Hero hero = null,
            ArmorManager armorManager = null,
            ShopManager shopManager = null,
            System.Func<IWalletManager> getWallet = null)
        {
            _hero = hero;
            _abilityManager = _hero != null ? _hero.AbilityManager : null;
            _effects = new ShopItemEffects(hero, _abilityManager, armorManager ?? hero?.GetComponent<ArmorManager>());
            _getWallet = getWallet ?? (() => WalletManager.Instance);
            _shopManager = shopManager;
        }

        public bool TryPurchaseItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            ShopItemData item = FindShopItem(itemId);

            if (item == null || item.CanBePurchased() == false)
            {
                return false;
            }

            if (ProcessPayment(item) == false)
            {
                return false;
            }

            _effects.Apply(itemId);
            SavePurchase(itemId);

            return true;
        }

        public string GetItemDescription(string itemId) => ShopItemDescriptions.GetItemDescription(itemId);

        public string GetItemName(string itemId) => ShopItemNames.GetItemName(itemId);

        private bool ProcessPayment(ShopItemData item)
        {
            IWalletManager wallet = _getWallet();
            return wallet != null && wallet.TrySpendCoins(item.CurrencyType, item.Price);
        }

        private ShopItemData FindShopItem(string itemId)
        {
            if (_shopManager == null)
            {
                _shopManager = Object.FindFirstObjectByType<ShopManager>();
            }

            if (_shopManager == null || _shopManager.ShopItems == null)
            {
                return null;
            }

            return _shopManager.ShopItems.Find(item => item.ItemId == itemId);
        }

        private void SavePurchase(string itemId)
        {
            if (itemId == ShopItemIds.RestoreArmor)
            {
                int uses = PlayerPrefs.GetInt("ArmorPlates_Used", 0);

                PlayerPrefs.SetInt("ArmorPlates_Used", uses + 1);
                PlayerPrefs.Save();

                return;
            }

            if (itemId != ShopItemIds.ActivateLastChance)
            {
                SaveSystem.Instance?.MarkItemPurchased(itemId);
            }

            if (_abilityManager != null)
            {
                SaveSystem.Instance?.UpdateAbilityData(_abilityManager);
                ShopPurchaseHistory.Instance?.OnItemPurchased(itemId, _abilityManager);
            }

            SaveGameAfterPurchase();
        }

        private void SaveGameAfterPurchase()
        {
            if (SaveSystem.Instance == null || _hero == null)
            {
                return;
            }

            SaveSystem.Instance.SaveGame(string.Empty, _hero.transform.position);
        }
    }
}
