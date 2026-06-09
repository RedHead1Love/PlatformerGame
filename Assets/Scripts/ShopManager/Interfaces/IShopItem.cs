namespace ShopLogic
{
    public interface IShopItem
    {
        string ItemId { get; }
        string ItemName { get; }
        int Price { get; }
        bool IsPurchased { get; }

        void Purchase();
    }
}
