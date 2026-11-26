using Foxberry;
using GeneralLogicEnemies;
using UnityEngine;

namespace GameLogic
{
    public static class GameProgressResetter
    {
        public static void Reset()
        {
            Reset(GameServices.Progress);
        }

        public static void Reset(IWorldProgress progress)
        {
            if (progress == null)
            {
                throw new System.ArgumentNullException(nameof(progress));
            }
            float music = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            int savedLanguage = PlayerPrefs.GetInt("GameLanguage", 0);
            int manualOverride = PlayerPrefs.GetInt("LangManualOverride", 0);
            SaveSystem.Instance?.DeleteSaveData();
            EnemyRegistry.Instance?.ResetAllEnemies();
            progress.ResetGameState();
            LevelStatsTracker.ResetAllStats();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetFloat("MusicVolume", music);
            PlayerPrefs.SetFloat("SFXVolume", sfx);
            PlayerPrefs.SetInt("GameLanguage", savedLanguage);
            PlayerPrefs.SetInt("LangManualOverride", manualOverride);
            PlayerPrefs.Save();
        }
    }
}
