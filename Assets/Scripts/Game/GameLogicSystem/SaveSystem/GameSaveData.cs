using System;
using System.Collections.Generic;
using DoorControl;
using UnityEngine;

namespace GameLogic
{
    [Serializable]
    public sealed class GameSaveData
    {
        [SerializeField]
        private string SaveId;
        [SerializeField]
        private DateTime SaveTime;
        [SerializeField]
        private Vector3 PlayerPosition;
        [SerializeField]
        private int PlayerHealth;
        [SerializeField]
        private int PlayerArmor;
        [SerializeField]
        private List<KeyColor> CollectedKeys;
        [SerializeField]
        private List<string> KilledEnemies;
        [SerializeField]
        private List<string> CollectedHealthPickups;
        [SerializeField]
        private string CheckpointId;
        [SerializeField]
        private string SceneName;
        [SerializeField]
        private CoinData Coins;
        [SerializeField]
        private List<string> PurchasedItemIds;
        [SerializeField]
        private AbilitySaveData AbilityData;

        public GameSaveData()
        {
            SaveId = string.Empty;
            SaveTime = DateTime.Now;
            PlayerPosition = Vector3.zero;

            PlayerHealth = 0;
            PlayerArmor = 0;

            CollectedKeys = new List<KeyColor>();
            KilledEnemies = new List<string>();
            CollectedHealthPickups = new List<string>();

            CheckpointId = string.Empty;
            SceneName = string.Empty;

            Coins = new CoinData(0, 0, 0);

            PurchasedItemIds = new List<string>();
            AbilityData = new AbilitySaveData();
        }

        public string Id
        {
            get => SaveId;
            set => SaveId = value;
        }

        public DateTime Timestamp
        {
            get => SaveTime;
            set => SaveTime = value;
        }

        public Vector3 Position
        {
            get => PlayerPosition;
            set => PlayerPosition = value;
        }

        public int Health
        {
            get => PlayerHealth;
            set => PlayerHealth = value;
        }

        public int Armor
        {
            get => PlayerArmor;
            set => PlayerArmor = value;
        }

        public List<KeyColor> Keys
        {
            get => CollectedKeys;
            set => CollectedKeys = value;
        }

        public List<string> DefeatedEnemies
        {
            get => KilledEnemies;
            set => KilledEnemies = value;
        }

        public List<string> HealthPickups
        {
            get => CollectedHealthPickups;
            set => CollectedHealthPickups = value;
        }

        public string Checkpoint
        {
            get => CheckpointId;
            set => CheckpointId = value;
        }

        public string Scene
        {
            get => SceneName;
            set => SceneName = value;
        }

        public CoinData WalletBalance
        {
            get => Coins;
            set => Coins = value;
        }

        public List<string> Purchases
        {
            get => PurchasedItemIds;
            set => PurchasedItemIds = value;
        }

        public AbilitySaveData Abilities
        {
            get => AbilityData;
            set => AbilityData = value;
        }

        public void EnsureValidCollections()
        {
            CollectedKeys ??= new List<KeyColor>();
            KilledEnemies ??= new List<string>();
            CollectedHealthPickups ??= new List<string>();
            PurchasedItemIds ??= new List<string>();
            AbilityData ??= new AbilitySaveData();

            if (Coins.IsInitialized == false)
            {
                Coins = new CoinData(Coins.Bronze, Coins.Silver, Coins.Gold);
            }

            if (PlayerArmor < 0)
            {
                PlayerArmor = 0;
            }
        }
    }
}
