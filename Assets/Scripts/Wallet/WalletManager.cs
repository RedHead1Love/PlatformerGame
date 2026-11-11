using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public sealed class WalletManager : MonoBehaviour, IWalletManager
    {
        private readonly WalletBalanceStore _storage = new WalletBalanceStore(new UnityProgressPreferences());
        private const int DefaultCoinAmount = 0;
        private const int MinimumCoinAmount = 0;

        private readonly Dictionary<CoinType, int> _coins = new Dictionary<CoinType, int>();

        public event Action<CoinType, int> OnCoinsChanged;

        public static WalletManager Instance { get; private set; }

        public void AddCoins(CoinType type, int amount)
        {
            if (amount <= MinimumCoinAmount)
            {
                return;
            }

            EnsureCoinTypeExists(type);

            _coins[type] += amount;

            NotifyCoinChanged(type);
            SaveWallet();
        }

        public bool TrySpendCoins(CoinType type, int amount)
        {
            if (amount <= MinimumCoinAmount)
            {
                return false;
            }

            EnsureCoinTypeExists(type);

            if (_coins[type] < amount)
            {
                return false;
            }

            _coins[type] -= amount;

            NotifyCoinChanged(type);
            SaveWallet();

            return true;
        }

        public int GetCoins(CoinType type)
        {
            EnsureCoinTypeExists(type);

            return _coins[type];
        }

        public Dictionary<CoinType, int> GetAllCoins()
        {
            return new Dictionary<CoinType, int>(_coins);
        }

        public void SaveWallet()
        {
            _storage.Save(_coins);
        }

        public void LoadWallet()
        {
            foreach (CoinType type in Enum.GetValues(typeof(CoinType)))
            {
                _coins[type] =
                    _storage.Load(type);

                NotifyCoinChanged(type);
            }
        }

        public void LoadFromSaveData(CoinData saveData)
        {
            if (saveData.IsInitialized == false)
            {
                return;
            }

            SetCoins(CoinType.Bronze, saveData.Bronze);
            SetCoins(CoinType.Silver, saveData.Silver);
            SetCoins(CoinType.Gold, saveData.Gold);

            SaveWallet();
        }

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

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
                InitializeWallet();

                return;
            }

            Destroy(gameObject);
        }

        private void InitializeWallet()
        {
            foreach (CoinType type in Enum.GetValues(typeof(CoinType)))
            {
                _coins[type] = DefaultCoinAmount;
            }

            LoadWallet();
        }

        private void SetCoins(CoinType type, int amount)
        {
            _coins[type] = Mathf.Max(MinimumCoinAmount, amount);

            NotifyCoinChanged(type);
        }

        private void EnsureCoinTypeExists(CoinType type)
        {
            if (_coins.ContainsKey(type))
            {
                return;
            }

            _coins[type] = MinimumCoinAmount;
        }

        private void NotifyCoinChanged(CoinType type)
        {
            OnCoinsChanged?.Invoke(type, _coins[type]);
        }
    }
}
