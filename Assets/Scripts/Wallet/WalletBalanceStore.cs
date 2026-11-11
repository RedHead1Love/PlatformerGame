using System;
using System.Collections.Generic;

namespace GameLogic
{
    internal sealed class WalletBalanceStore
    {
        private readonly IProgressPreferences _preferences;

        public WalletBalanceStore(IProgressPreferences preferences)
        {
            _preferences = preferences;
        }

        public int Load(CoinType type) => Math.Max(0, _preferences.GetInt($"Coins_{type}"));

        public void Save(Dictionary<CoinType, int> coins)
        {
            foreach (KeyValuePair<CoinType, int> pair in coins)
            {
                _preferences.SetInt($"Coins_{pair.Key}", pair.Value);
            }

            _preferences.Save();
        }
    }
}
