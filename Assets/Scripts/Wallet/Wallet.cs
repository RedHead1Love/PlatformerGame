namespace GameLogic
{
    public sealed class Wallet : IWallet
    {
        private const int MinimumCoins = 0;

        private int _bronzeCoins;
        private int _silverCoins;
        private int _goldCoins;

        public int GetCoins(WalletManager.CoinType type)
        {
            return type switch
            {
                WalletManager.CoinType.Bronze => _bronzeCoins,
                WalletManager.CoinType.Silver => _silverCoins,
                WalletManager.CoinType.Gold => _goldCoins,
                _ => MinimumCoins
            };
        }

        public void AddCoins(WalletManager.CoinType type, int amount)
        {
            if (amount <= MinimumCoins)
            {
                return;
            }

            switch (type)
            {
                case WalletManager.CoinType.Bronze:
                    _bronzeCoins += amount;
                    break;
                case WalletManager.CoinType.Silver:
                    _silverCoins += amount;
                    break;
                case WalletManager.CoinType.Gold:
                    _goldCoins += amount;
                    break;
            }
        }

        public bool SpendCoins(WalletManager.CoinType type, int amount)
        {
            if (amount <= MinimumCoins || GetCoins(type) < amount)
            {
                return false;
            }

            switch (type)
            {
                case WalletManager.CoinType.Bronze:
                    _bronzeCoins -= amount;
                    break;
                case WalletManager.CoinType.Silver:
                    _silverCoins -= amount;
                    break;
                case WalletManager.CoinType.Gold:
                    _goldCoins -= amount;
                    break;
            }

            return true;
        }

        public void Reset()
        {
            _bronzeCoins = MinimumCoins;
            _silverCoins = MinimumCoins;
            _goldCoins = MinimumCoins;
        }
    }
}
