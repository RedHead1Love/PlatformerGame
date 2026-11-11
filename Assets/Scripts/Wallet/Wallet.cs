using System;
using UnityEngine;

namespace GameLogic
{
    public sealed class Wallet : MonoBehaviour, IWallet
    {
        private int _coins;

        public event Action<int> OnCoinsChanged;

        public static Wallet Instance { get; private set; }

        public int Coins => _coins;

        public void AddCoins(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _coins += amount;

            OnCoinsChanged?.Invoke(_coins);
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount <= 0 || _coins < amount)
            {
                return false;
            }

            _coins -= amount;

            OnCoinsChanged?.Invoke(_coins);

            return true;
        }

        public int GetCoins()
        {
            return _coins;
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

                return;
            }

            Destroy(gameObject);
        }
    }
}
