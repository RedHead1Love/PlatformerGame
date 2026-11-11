using System;
using GameLogic;

namespace Foxberry
{
    public interface IPersistentWallet
    {
        event Action<CoinData> OnCoinsUpdated;

        CoinData CurrentCoins { get; }

        void AddCoins(string coinType, int amount);

        void LoadCoinsFromSave(CoinData savedCoins);

        void LoadFromSaveData(CoinData saveData);

        void SaveCoins();

        void LoadCoins();
    }
}
