using System.Collections.Generic;
using UnityEngine;

namespace ShopLogic
{
    public sealed class ShopSaveManager : MonoBehaviour
    {
        private const string PurchasedItemsKey = "ShopItemKeys";
        private const char Separator = ',';

        public static ShopSaveManager Instance { get; private set; }

        private HashSet<string> _purchasedItems = new HashSet<string>();

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                
                DontDestroyOnLoad(gameObject);
                LoadPurchasedItems();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SavePurchasedItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || _purchasedItems.Contains(itemId))
            {
                return;
            }

            _purchasedItems.Add(itemId);
            
            SaveToPlayerPrefs();
        }

        public bool IsItemPurchased(string itemId)
        {
            return _purchasedItems.Contains(itemId);
        }

        private void LoadPurchasedItems()
        {
            string savedData = PlayerPrefs.GetString(PurchasedItemsKey, string.Empty);
            
            if (string.IsNullOrEmpty(savedData))
            {
                return;
            }

            string[] items = savedData.Split(Separator);
            
            foreach (string item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    _purchasedItems.Add(item);
                }
            }
        }

        private void SaveToPlayerPrefs()
        {
            string dataToSave = string.Join(Separator.ToString(), _purchasedItems);
            
            PlayerPrefs.SetString(PurchasedItemsKey, dataToSave);
            PlayerPrefs.Save();
        }
    }
}
