using DoorControl;
using Player;
using Player.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LevelLoader : MonoBehaviour
{
    private const string CommandUnlockMap = "1";
    private const string CommandUnlockDash = "2";
    private const string CommandUnlockAnatomy = "3";
    private const string CommandUnlockArmor = "4";
    private const string CommandUnlockSwampDamageBonus = "5";
    private const string CommandUnlockSkeletonDamageBonus = "8";
    private const string CommandUnlockDemonDamageBonus = "9";
    private const string CommandUnlockSpiderDamageBonus = "10";
    private const string CommandUnlockZombieDamageBonus = "11";
    private const string CommandUnlockPassiveHealthRegeneration = "12";
    private const string CommandUnlockRobocopRegeneration = "13";
    private const string CommandUnlockVampireAbility = "14";
    private const string CommandUnlockOnePunchManAbility = "15";
    private const string CommandUnlockBossDamageBonus = "16";

    private const float LoadDelay = 0.1f;

    private void Start()
    {
        StartCoroutine(LoadSavedGameDataCoroutine());
    }

    private IEnumerator LoadSavedGameDataCoroutine()
    {
        yield return new WaitForSeconds(LoadDelay);

        if (!CanLoadGame())
        {
            yield break;
        }

        GameSaveData saveData = SaveSystem.Instance.CurrentSave;
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (saveData.SceneName != currentSceneName)
        {
            yield break;
        }

        ApplySavedData(saveData);
    }

    private bool CanLoadGame()
    {
        return SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
    }

    private void ApplySavedData(GameSaveData saveData)
    {
        Hero player = FindFirstObjectByType<Hero>();

        if (player != null)
        {
            ApplyPlayerData(player, saveData);
        }

        ApplyGameWorldData(saveData);
    }

    private void ApplyPlayerData(Hero player, GameSaveData saveData)
    {
        int deathThreshold = 0;

        Vector3 spawnPosition = FindCheckpointPosition(saveData.CheckpointId);

        if (spawnPosition != Vector3.zero)
        {
            player.transform.position = spawnPosition;
        }

        if (saveData.PlayerHealth > deathThreshold)
        {
            player.SetHealth(saveData.PlayerHealth);
        }

        ApplyArmorData(player, saveData);
    }

    private void ApplyArmorData(Hero player, GameSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        ArmorManager armorManager = player.GetComponent<ArmorManager>();

        if (armorManager != null)
        {
            armorManager.LoadArmorFromSave(saveData.PlayerArmor);
        }
    }

    private void ApplyGameWorldData(GameSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        KeyCollection keyCollection = FindFirstObjectByType<KeyCollection>();

        if (keyCollection != null && saveData.CollectedKeys != null)
        {
            keyCollection.LoadCollectedKeys(saveData.CollectedKeys);
        }

        EnemyManager.Instance?.SyncWithSaveData();
    }

    private Vector3 FindCheckpointPosition(string checkpointId)
    {
        if (string.IsNullOrEmpty(checkpointId))
        {
            return Vector3.zero;
        }

        Checkpoint[] allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        foreach (Checkpoint checkpoint in allCheckpoints)
        {
            if (checkpoint.GetCheckpointId() == checkpointId)
            {
                return checkpoint.GetSpawnPosition();
            }
        }

        return Vector3.zero;
    }
}
