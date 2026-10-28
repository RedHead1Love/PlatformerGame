using System;

[Serializable]
public struct CoinData
{
    private const int MinimumCoinCount = 0;

    public int Bronze;
    public int Silver;
    public int Gold;
    public bool IsInitialized;

    public CoinData(
        int bronze = MinimumCoinCount,
        int silver = MinimumCoinCount,
        int gold = MinimumCoinCount)
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
        IsInitialized = false
    };
}
