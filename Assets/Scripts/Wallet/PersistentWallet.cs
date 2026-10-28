using System;
using System.Collections;
using GameLogic;
using UnityEngine;

public sealed class PersistentWallet : MonoBehaviour, IPersistentWallet
{
    private const string BronzeCoinCommand = "1";
    private const string SilverCoinCommand = "2";
    private const string GoldCoinCommand = "3";

    private const string BronzeCoinName = "bronze";
    private const string SilverCoinName = "silver";
    private const string GoldCoinName = "gold";

    private const string SaveKey = "PlayerCoins";
    private const float SyncDelay = 0.5f;
    private const int MinimumCoinAmount = 0;

    [SerializeField] private CoinData _currentCoins;

    public static PersistentWallet Instance { get; private set; }

    public CoinData CurrentCoins => _currentCoins;

    public event Action<CoinData> OnCoinsUpdated;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        StartCoroutine(SyncWithSaveData());
    }

    public void AddCoins(string coinType, int amount)
    {
        if (amount <= MinimumCoinAmount || string.IsNullOrWhiteSpace(coinType))
        {
            return;
        }

        CoinData newCoins = _currentCoins;

        switch (coinType.Trim().ToLowerInvariant())
        {
            case BronzeCoinCommand:
            case BronzeCoinName:
                newCoins.Bronze += amount;

                break;

            case SilverCoinCommand:
            case SilverCoinName:
                newCoins.Silver += amount;

                break;

            case GoldCoinCommand:
            case GoldCoinName:
                newCoins.Gold += amount;

                break;

            default:
                return;
        }

        newCoins.IsInitialized = true;

        SetCoins(newCoins);
    }

    public void LoadCoinsFromSave(CoinData savedCoins)
    {
        CoinData validCoins = new CoinData(
            savedCoins.Bronze,
            savedCoins.Silver,
            savedCoins.Gold);

        SetCoins(validCoins);
    }

    public void LoadFromSaveData(CoinData saveData)
    {
        LoadCoinsFromSave(saveData);
    }

    public void SaveCoins()
    {
        string json = JsonUtility.ToJson(_currentCoins);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        if (PlayerPrefs.HasKey(SaveKey) == false)
        {
            _currentCoins = new CoinData();

            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);

        _currentCoins = JsonUtility.FromJson<CoinData>(json);

        _currentCoins = new CoinData(
            _currentCoins.Bronze,
            _currentCoins.Silver,
            _currentCoins.Gold);
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            LoadCoins();

            return;
        }

        Destroy(gameObject);
    }

    private IEnumerator SyncWithSaveData()
    {
        yield return new WaitForSeconds(SyncDelay);

        if (SaveSystem.Instance == null || SaveSystem.Instance.HasSave() == false)
        {
            yield break;
        }

        GameSaveData saveData = SaveSystem.Instance.CurrentSave;

        if (saveData == null || saveData.Coins.IsInitialized == false)
        {
            yield break;
        }

        LoadCoinsFromSave(saveData.Coins);
    }

    private void SetCoins(CoinData coins)
    {
        _currentCoins = coins;

        SaveCoins();
        SyncWalletManager();
        OnCoinsUpdated?.Invoke(_currentCoins);
    }

    private void SyncWalletManager()
    {
        if (WalletManager.Instance == null)
        {
            return;
        }

        WalletManager.Instance.LoadFromSaveData(_currentCoins);
    }
}
