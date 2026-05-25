using System;
using UnityEngine;

namespace GameLogic
{
    public class WalletManager : MonoBehaviour, IWalletManager
    {
        public enum CoinType
        {
            Bronze,
            Silver,
            Gold
        }

        public static WalletManager Instance { get; protected set; }

        public event Action<CoinType, int> OnCoinsChanged;

        protected Wallet _internalWallet;

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                _internalWallet = new Wallet();

                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public virtual int GetCoins(CoinType type)
        {
            return _internalWallet?.GetCoins(type) ?? 0;
        }

        public virtual void AddCoins(CoinType type, int amount)
        {
            if (_internalWallet == null)
            {
                return;
            }

            _internalWallet.AddCoins(type, amount);
            NotifyCoinsChanged(type);
        }

        public virtual bool SpendCoins(CoinType type, int amount)
        {
            if (_internalWallet != null && _internalWallet.SpendCoins(type, amount))
            {
                NotifyCoinsChanged(type);
                return true;
            }

            return false;
        }

        public virtual void ResetWallets()
        {
            _internalWallet?.Reset();

            NotifyCoinsChanged(CoinType.Bronze);
            NotifyCoinsChanged(CoinType.Silver);
            NotifyCoinsChanged(CoinType.Gold);
        }

        public virtual void LoadFromSaveData(CoinData data)
        {
        }

        protected void NotifyCoinsChanged(CoinType type)
        {
            OnCoinsChanged?.Invoke(type, GetCoins(type));
        }
    }
}
