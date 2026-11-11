using System;
using System.Collections.Generic;

namespace GameLogic
{
    public interface IWalletManager
    {
        event Action<CoinType, int> OnCoinsChanged;

        void AddCoins(CoinType type, int amount);

        bool TrySpendCoins(CoinType type, int amount);

        int GetCoins(CoinType type);

        Dictionary<CoinType, int> GetAllCoins();

        void SaveWallet();

        void LoadWallet();

        void LoadFromSaveData(CoinData saveData);
    }
}
