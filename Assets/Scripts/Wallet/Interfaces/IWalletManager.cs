using System;

namespace GameLogic
{
    public interface IWalletManager : IWallet
    {
        event Action<WalletManager.CoinType, int> OnCoinsChanged;

        void ResetWallets();
    }
}
