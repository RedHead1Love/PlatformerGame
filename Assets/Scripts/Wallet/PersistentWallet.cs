using UnityEngine;

namespace GameLogic
{
    public sealed class PersistentWallet : WalletManager, IPersistentWallet
    {
        private const string BronzeKey = "Coins_Bronze";
        private const string SilverKey = "Coins_Silver";
        private const string GoldKey = "Coins_Gold";

        public new static PersistentWallet Instance { get; private set; }

        public CoinData CurrentCoins => new CoinData(
            GetCoins(CoinType.Bronze),
            GetCoins(CoinType.Silver),
            GetCoins(CoinType.Gold)
        );

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                WalletManager.Instance = this;
                _internalWallet = new Wallet();

                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public override void AddCoins(CoinType type, int amount)
        {
            base.AddCoins(type, amount);
            SaveCoins();
        }

        public override bool SpendCoins(CoinType type, int amount)
        {
            bool success = base.SpendCoins(type, amount);

            if (success)
            {
                SaveCoins();
            }

            return success;
        }

        public override void ResetWallets()
        {
            base.ResetWallets();
            SaveCoins();
        }

        public void SaveCoins()
        {
            PlayerPrefs.SetInt(BronzeKey, GetCoins(CoinType.Bronze));
            PlayerPrefs.SetInt(SilverKey, GetCoins(CoinType.Silver));
            PlayerPrefs.SetInt(GoldKey, GetCoins(CoinType.Gold));

            PlayerPrefs.Save();
        }

        public void LoadCoins()
        {
            _internalWallet?.Reset();

            if (PlayerPrefs.HasKey(BronzeKey)) base.AddCoins(CoinType.Bronze, PlayerPrefs.GetInt(BronzeKey));
            if (PlayerPrefs.HasKey(SilverKey)) base.AddCoins(CoinType.Silver, PlayerPrefs.GetInt(SilverKey));
            if (PlayerPrefs.HasKey(GoldKey)) base.AddCoins(CoinType.Gold, PlayerPrefs.GetInt(GoldKey));
        }

        public void LoadFromSaveData(CoinData data)
        {
            if (!data.IsInitialized)
            {
                return;
            }

            _internalWallet?.Reset();

            base.AddCoins(CoinType.Bronze, data.Bronze);
            base.AddCoins(CoinType.Silver, data.Silver);
            base.AddCoins(CoinType.Gold, data.Gold);
        }

        public void LoadCoinsFromSave(CoinData savedCoins)
        {
            LoadFromSaveData(savedCoins);
        }
    }
}
