using System;

[Serializable]
public struct CoinData
{
    private const int MinimumCoinCount = 0;

    public int bronze;
    public int silver;
    public int gold;
    public bool isInitialized;

    public CoinData(
        int bronze = MinimumCoinCount,
        int silver = MinimumCoinCount,
        int gold = MinimumCoinCount)
    {
        this.bronze = Math.Max(MinimumCoinCount, bronze);
        this.silver = Math.Max(MinimumCoinCount, silver);
        this.gold = Math.Max(MinimumCoinCount, gold);
        isInitialized = true;
    }

    public static CoinData Empty => new CoinData
    {
        bronze = MinimumCoinCount,
        silver = MinimumCoinCount,
        gold = MinimumCoinCount,
        isInitialized = false
    };
}