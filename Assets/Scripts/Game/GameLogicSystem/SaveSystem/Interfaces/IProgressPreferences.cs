namespace GameLogic
{
    internal interface IProgressPreferences
    {
        int GetInt(string key, int defaultValue = 0);
        string GetString(string key, string defaultValue = "");
        bool HasKey(string key);
        void SetInt(string key, int value);
        void SetString(string key, string value);
        void DeleteKey(string key);
        void Save();
    }
}
