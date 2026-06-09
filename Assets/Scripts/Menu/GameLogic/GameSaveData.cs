using DoorControl;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GameSaveData
{
    public string saveId;
    public DateTime saveTime;
    public Vector3 playerPosition;

    public int playerHealth;
    public int playerArmor;

    public List<KeyColor> collectedKeys = new List<KeyColor>();
    public List<string> killedEnemies = new List<string>();
    public List<string> collectedHealthPickups = new List<string>();

    public string checkpointId;
    public string sceneName;

    public CoinData coins;

    public List<string> purchasedItemIds = new List<string>();
    public AbilitySaveData abilityData = new AbilitySaveData();

    public GameSaveData()
    {
        saveId = string.Empty;
        saveTime = DateTime.Now;
        playerPosition = Vector3.zero;

        playerHealth = 0;
        playerArmor = 0;

        collectedKeys = new List<KeyColor>();
        killedEnemies = new List<string>();
        collectedHealthPickups = new List<string>();

        checkpointId = string.Empty;
        sceneName = string.Empty;

        coins = new CoinData(0, 0, 0);

        purchasedItemIds = new List<string>();
        abilityData = new AbilitySaveData();
    }

    public void EnsureValidCollections()
    {
        collectedKeys ??= new List<KeyColor>();
        killedEnemies ??= new List<string>();
        collectedHealthPickups ??= new List<string>();
        purchasedItemIds ??= new List<string>();
        abilityData ??= new AbilitySaveData();

        if (coins.isInitialized == false)
        {
            coins = new CoinData(coins.bronze, coins.silver, coins.gold);
        }

        if (playerArmor < 0)
        {
            playerArmor = 0;
        }
    }
}