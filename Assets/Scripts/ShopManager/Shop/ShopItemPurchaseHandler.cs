using GameLogic;
using UnityEngine;

namespace ShopLogic
{
    public sealed class ShopItemPurchaseHandler : MonoBehaviour
    {
        private const float SoundVolume = 1f;

        [SerializeField] private AudioController _audioController;
        [SerializeField] private AudioClip _purchaseSuccessSound;
        [SerializeField] private AudioClip _purchaseFailSound;

        private void Awake()
        {
            if (_audioController == null)
            {
                _audioController = FindFirstObjectByType<AudioController>();
            }
        }

        public void TryPurchaseItem(ShopItemData itemData, ShopItemView view)
        {
            if (itemData == null || WalletManager.Instance == null)
            {
                return;
            }

            if (WalletManager.Instance.GetCoins(WalletManager.CoinType.Bronze) >= itemData.Price)
            {
                CompletePurchase(itemData, view);
            }
            else
            {
                PlayErrorSound();
            }
        }

        private void CompletePurchase(ShopItemData itemData, ShopItemView view)
        {
            WalletManager.Instance.SpendCoins(WalletManager.CoinType.Bronze, itemData.Price);
            
            if (!itemData.IsConsumable && ShopSaveManager.Instance != null)
            {
                ShopSaveManager.Instance.SavePurchasedItem(itemData.ItemId);
            }

            ApplyItemEffect();
            PlayPurchaseSound();
            
            view.RefreshState();
        }

        private void ApplyItemEffect()
        {
            ShopEffectsInitializer initializer = FindFirstObjectByType<ShopEffectsInitializer>();
            
            initializer?.InitializeShopEffects();
        }

        private void PlayPurchaseSound()
        {
            if (_audioController != null && _purchaseSuccessSound != null)
            {
                _audioController.PlayOneShotWithVolume(_purchaseSuccessSound, SoundVolume);
            }
        }

        private void PlayErrorSound()
        {
            if (_audioController != null && _purchaseFailSound != null)
            {
                _audioController.PlayOneShotWithVolume(_purchaseFailSound, SoundVolume);
            }
        }
    }
}
