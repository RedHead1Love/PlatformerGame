using System.Collections.Generic;

namespace GameLogic
{
    public interface IWalletBalanceStore
    {
        int Load(CoinType type);

        void Save(Dictionary<CoinType, int> coins);
    }
}
