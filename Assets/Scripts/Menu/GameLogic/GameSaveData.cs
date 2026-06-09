using DoorControl;
using Player.Abilities;
using System;
using System.Collections.Generic;
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
        Coins = new CoinData(0, 0, 0);
        CollectedKeys = new List<KeyColor>();
        KilledEnemies = new List<string>();
        CollectedHealthPickups = new List<string>();
        PlayerArmor = 0;
        PurchasedItemIds = new List<string>();
        AbilityData = new AbilitySaveData();
    }
}
