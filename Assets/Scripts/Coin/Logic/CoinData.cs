using System;
using UnityEngine.Serialization;

namespace GameLogic
{
    [Serializable]
    public struct CoinData
    {
        [FormerlySerializedAs("bronze")]
        public int Bronze;
        [FormerlySerializedAs("silver")]
        public int Silver;
        [FormerlySerializedAs("gold")]
        public int Gold;
        [FormerlySerializedAs("isInitialized")]
        public bool IsInitialized;

        private const int MinimumCoinCount = 0;

        public CoinData(int bronze = MinimumCoinCount, int silver = MinimumCoinCount, int gold = MinimumCoinCount)
        {
            this.Bronze = Math.Max(MinimumCoinCount, bronze);
            this.Silver = Math.Max(MinimumCoinCount, silver);
            this.Gold = Math.Max(MinimumCoinCount, gold);
            IsInitialized = true;
        }

        public static CoinData Empty => new CoinData
        {
            Bronze = MinimumCoinCount,
            Silver = MinimumCoinCount,
            Gold = MinimumCoinCount,
            IsInitialized = false,
        };
    }
}
