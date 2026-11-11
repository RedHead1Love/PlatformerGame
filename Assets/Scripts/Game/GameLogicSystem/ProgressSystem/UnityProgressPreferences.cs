using UnityEngine;

namespace GameLogic
{
    internal sealed class UnityProgressPreferences : IProgressPreferences
    {
        public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }
}
