using UnityEngine;

namespace ShopLogic
{
    [CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item Data")]
    public sealed class ShopItemData : ScriptableObject
    {
        [SerializeField] private string _itemId;
        [SerializeField] private string _itemName;
        [SerializeField] private string _description;
        [SerializeField] private int _price;
        [SerializeField] private Sprite _icon;
        [SerializeField] private bool _isConsumable;

        public string ItemId => _itemId;
        public string ItemName => _itemName;
        public string Description => _description;
        public int Price => _price;
        public Sprite Icon => _icon;
        public bool IsConsumable => _isConsumable;
    }
}
