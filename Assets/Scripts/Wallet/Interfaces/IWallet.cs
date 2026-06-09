namespace GameLogic
{
    public interface IWallet
    {
        int GetCoins(WalletManager.CoinType type);
        void AddCoins(WalletManager.CoinType type, int amount);
        bool SpendCoins(WalletManager.CoinType type, int amount);
    }
}
