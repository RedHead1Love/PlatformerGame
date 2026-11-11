using GameLogic;
using Player.Abilities;
using UnityEngine;

namespace Foxberry.Shop
{
    public sealed class ShopPurchaseHistory : MonoBehaviour
    {
        private const string ShopItemKeyPrefix = "ShopItem_";
        private const string ShopItemKeySuffix = "_Purchased";
        private const string ShopItemKeysList = "ShopItemKeys";
        private const int PurchasedValue = 1;
        private const int NotPurchasedValue = 0;

        public static ShopPurchaseHistory Instance { get; private set; }

        public void OnItemPurchased(string itemId, AbilityManager abilityManager)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            _preferences.SetInt(GetPurchaseKey(itemId), PurchasedValue);
            AppendItemKey(itemId);
            _preferences.Save();

            if (abilityManager != null && _save != null)
            {
                _save.UpdateAbilityData(abilityManager);
            }
        }

        public bool IsItemPurchased(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            if (_preferences.HasKey(GetPurchaseKey(itemId)))
            {
                return _preferences.GetInt(GetPurchaseKey(itemId), NotPurchasedValue) == PurchasedValue;
            }

            return _save != null && _save.CurrentSave?.Purchases != null &&
                   _save.CurrentSave.Purchases.Contains(itemId);
        }

        public void LoadAllPurchases()
        {
            if (_save == null || _save.CurrentSave == null)
            {
                return;
            }

            if (_save.CurrentSave.Purchases == null)
            {
                return;
            }

            foreach (string itemId in _save.CurrentSave.Purchases)
            {
                _preferences.SetInt(GetPurchaseKey(itemId), PurchasedValue);
                AppendItemKey(itemId);
            }

            _preferences.Save();
        }

        public void ResetAllPurchases()
        {
            string keysRaw = _preferences.GetString(ShopItemKeysList, string.Empty);
            string[] keys = keysRaw.Split(',');

            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                _preferences.DeleteKey(GetPurchaseKey(key));
            }

            _preferences.DeleteKey(ShopItemKeysList);
            _preferences.Save();
        }

        private readonly IProgressPreferences _preferences = new UnityProgressPreferences();
        private IGameProgressSave _save;

        private void OnDestroy()
        {
            GameSaveConnections.Disconnect(BindSave);
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void BindSave(IGameProgressSave save)
        {
            _save = save;
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                GameSaveConnections.Connect(BindSave);

                DontDestroyOnLoad(gameObject);

                return;
            }

            Destroy(gameObject);
        }

        private string GetPurchaseKey(string itemId)
        {
            return $"{ShopItemKeyPrefix}{itemId}{ShopItemKeySuffix}";
        }

        private void AppendItemKey(string itemId)
        {
            string currentKeys = _preferences.GetString(ShopItemKeysList, string.Empty);

            if (System.Array.IndexOf(currentKeys.Split(','), itemId) >= 0)
            {
                return;
            }

            string separator = string.IsNullOrEmpty(currentKeys) ? string.Empty : ",";

            _preferences.SetString(ShopItemKeysList, $"{currentKeys}{separator}{itemId}");
        }
    }
}
