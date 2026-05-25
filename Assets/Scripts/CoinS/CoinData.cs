using System;

[Serializable]
public struct CoinData
{
    private const int MinimumCountBronzeCoin = 0;
    private const int MinimumCountSilverCoin = 0;
    private const int MinimumCountGoldCoin = 0;

    public int Bronze;
    public int Silver;
    public int Gold;
    public bool IsInitialized;

    public CoinData(int bronze = MinimumCountBronzeCoin, int silver = MinimumCountSilverCoin, int gold = MinimumCountGoldCoin)
    {
        Bronze = bronze;
        Silver = silver;
        Gold = gold;
        IsInitialized = true;
    }

    public static CoinData Empty => new CoinData(MinimumCountBronzeCoin, MinimumCountSilverCoin, MinimumCountGoldCoin) { IsInitialized = false };
}
