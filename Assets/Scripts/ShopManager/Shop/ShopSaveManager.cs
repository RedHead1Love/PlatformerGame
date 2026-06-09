using Player.Abilities;
using UnityEngine;

public sealed class ShopSaveManager : MonoBehaviour
{
    private const string ShopItemKeyPrefix = "ShopItem_";
    private const string ShopItemKeySuffix = "_Purchased";
    private const string ShopItemKeysList = "ShopItemKeys";
    private const int PurchasedValue = 1;
    private const int NotPurchasedValue = 0;

    public static ShopSaveManager Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    public void OnItemPurchased(string itemId, AbilityManager abilityManager)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        PlayerPrefs.SetInt(GetPurchaseKey(itemId), PurchasedValue);
        AppendItemKey(itemId);
        PlayerPrefs.Save();

        if (abilityManager != null && SaveSystem.Instance != null)
        {
            SaveSystem.Instance.UpdateAbilityData(abilityManager);
        }
    }

    public bool IsItemPurchased(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        if (PlayerPrefs.HasKey(GetPurchaseKey(itemId)))
        {
            return PlayerPrefs.GetInt(GetPurchaseKey(itemId), NotPurchasedValue) == PurchasedValue;
        }

        return SaveSystem.Instance != null &&
               SaveSystem.Instance.CurrentSave?.purchasedItemIds != null &&
               SaveSystem.Instance.CurrentSave.purchasedItemIds.Contains(itemId);
    }

    public void LoadAllPurchases()
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
            PlayerPrefs.SetInt(GetPurchaseKey(itemId), PurchasedValue);
            AppendItemKey(itemId);
        }

        PlayerPrefs.Save();
    }

    public void ResetAllPurchases()
    {
        string keysRaw = PlayerPrefs.GetString(ShopItemKeysList, string.Empty);
        string[] keys = keysRaw.Split(',');

        foreach (string key in keys)
        {
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            PlayerPrefs.DeleteKey(GetPurchaseKey(key));
        }

        PlayerPrefs.DeleteKey(ShopItemKeysList);
        PlayerPrefs.Save();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

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
        string currentKeys = PlayerPrefs.GetString(ShopItemKeysList, string.Empty);

        if (currentKeys.Contains(itemId))
        {
            return;
        }

        string separator = string.IsNullOrEmpty(currentKeys) ? string.Empty : ",";

        PlayerPrefs.SetString(ShopItemKeysList, $"{currentKeys}{separator}{itemId}");
    }
}