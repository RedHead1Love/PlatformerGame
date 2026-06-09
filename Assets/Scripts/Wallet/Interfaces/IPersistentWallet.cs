namespace GameLogic
{
    public interface IPersistentWallet : IWalletManager
    {
        CoinData CurrentCoins { get; }

        void SaveCoins();
        void LoadCoins();
        void LoadFromSaveData(CoinData data);
    }
}
