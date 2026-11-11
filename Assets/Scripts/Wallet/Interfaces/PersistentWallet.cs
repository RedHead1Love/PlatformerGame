using System;
using System.Collections;
using GameLogic;
using UnityEngine;

namespace Foxberry
{
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

        [SerializeField]
        private CoinData _currentCoins;

        public event Action<CoinData> OnCoinsUpdated;

        public static PersistentWallet Instance { get; private set; }

        public CoinData CurrentCoins => _currentCoins;

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
            CoinData validCoins = new CoinData(savedCoins.Bronze, savedCoins.Silver, savedCoins.Gold);

            SetCoins(validCoins);
        }

        public void LoadFromSaveData(CoinData saveData)
        {
            LoadCoinsFromSave(saveData);
        }

        public void SaveCoins()
        {
            string json = JsonUtility.ToJson(_currentCoins);

            _preferences.SetString(SaveKey, json);
            _preferences.Save();
        }

        public void LoadCoins()
        {
            if (_preferences.HasKey(SaveKey) == false)
            {
                _currentCoins = new CoinData();

                return;
            }

            string json = _preferences.GetString(SaveKey);

            _currentCoins = JsonUtility.FromJson<CoinData>(json);

            _currentCoins = new CoinData(_currentCoins.Bronze, _currentCoins.Silver, _currentCoins.Gold);
        }

        private readonly IProgressPreferences _preferences = new UnityProgressPreferences();
        private IGameProgressSave _save;

        private void BindSave(IGameProgressSave save)
        {
            _save = save;
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void OnDestroy()
        {
            GameSaveConnections.Disconnect(BindSave);
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            StartCoroutine(SyncWithSaveData());
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                GameSaveConnections.Connect(BindSave);

                DontDestroyOnLoad(gameObject);
                LoadCoins();

                return;
            }

            Destroy(gameObject);
        }

        private IEnumerator SyncWithSaveData()
        {
            yield return new WaitForSeconds(SyncDelay);

            if (_save == null || _save.HasSave() == false)
            {
                yield break;
            }

            GameSaveData saveData = _save.CurrentSave;

            if (saveData == null || saveData.WalletBalance.IsInitialized == false)
            {
                yield break;
            }

            LoadCoinsFromSave(saveData.WalletBalance);
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
}
