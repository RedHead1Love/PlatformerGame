using DoorControl;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public sealed class GameSaveData
{
    public string SaveId;
    public DateTime SaveTime;
    public Vector3 PlayerPosition;

    public int PlayerHealth;
    public int PlayerArmor;

    public List<KeyColor> CollectedKeys = new List<KeyColor>();
    public List<string> KilledEnemies = new List<string>();
    public List<string> CollectedHealthPickups = new List<string>();

    public string CheckpointId;
    public string SceneName;

    public CoinData Coins;

    public List<string> PurchasedItemIds = new List<string>();
    public AbilitySaveData AbilityData = new AbilitySaveData();

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
