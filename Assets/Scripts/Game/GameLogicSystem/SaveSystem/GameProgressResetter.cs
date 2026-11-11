using Foxberry;
using GeneralLogicEnemies;
using UnityEngine;

namespace GameLogic
{
    public static class GameProgressResetter
    {
        public static void Reset()
        {
            float music = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            int savedLanguage = PlayerPrefs.GetInt("GameLanguage", 0);

            SaveSystem.Instance?.DeleteSaveData();
            EnemyRegistry.Instance?.ResetAllEnemies();
            WorldProgress.ResetGameState();
            LevelStatsTracker.ResetAllStats();

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetFloat("MusicVolume", music);
            PlayerPrefs.SetFloat("SFXVolume", sfx);
            PlayerPrefs.SetInt("GameLanguage", savedLanguage);

            PlayerPrefs.Save();
        }
    }
}
