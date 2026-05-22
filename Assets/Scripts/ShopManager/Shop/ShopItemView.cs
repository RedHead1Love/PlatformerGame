using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShopLogic
{
    public sealed class ShopItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private GameObject _soldOutOverlay;

        private ShopItemData _itemData;
        private ShopItemPurchaseHandler _purchaseHandler;

        public void Initialize(ShopItemData itemData, ShopItemPurchaseHandler purchaseHandler)
        {
            _itemData = itemData;
            _purchaseHandler = purchaseHandler;

            UpdateUI();

            _purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }

        private void OnDestroy()
        {
            _purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
        }

        public void RefreshState()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_itemData == null)
            {
                return;
            }

            _nameText.text = _itemData.ItemName;
            _priceText.text = _itemData.Price.ToString();
            _descriptionText.text = _itemData.Description;
            _iconImage.sprite = _itemData.Icon;

            bool isPurchased = !_itemData.IsConsumable && ShopSaveManager.Instance.IsItemPurchased(_itemData.ItemId);

            if (_soldOutOverlay != null)
            {
                _soldOutOverlay.SetActive(isPurchased);
            }

            _purchaseButton.interactable = !isPurchased;
        }

        private void OnPurchaseClicked()
        {
            _purchaseHandler?.TryPurchaseItem(_itemData, this);
        }
    }
}
