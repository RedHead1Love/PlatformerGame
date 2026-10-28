using UnityEngine;

public static class BossDoorText
{
    public static string GetAllBossesDefeatedMessage()
    {
        return LocalizationManager.CurrentLanguage switch
        {
            LocalizationManager.Language.English => "The last boss is defeated! Head to the locked door.",
            LocalizationManager.Language.Turkish => "Son patron yenildi! Kilitli kapıya git.",
            _ => "Последний босс повержен! Направляйтесь к запертой двери."
        };
    }

    public static string GetBossStatusMessage(int aliveBossCount, int bossCount, KeyCode interactionKey)
    {
        int defeatedCount = bossCount - aliveBossCount;

        if (aliveBossCount > 0)
        {
            return LocalizationManager.CurrentLanguage switch
            {
                LocalizationManager.Language.English => $"({defeatedCount}/{bossCount}) bosses killed",
                LocalizationManager.Language.Turkish => $"({defeatedCount}/{bossCount}) patron öldürüldü",
                _ => $"({defeatedCount}/{bossCount}) боссов убито"
            };
        }

        string keyName = LocalizationManager.CurrentLanguage == LocalizationManager.Language.Russian ? "А (F)" : interactionKey.ToString();

        return LocalizationManager.CurrentLanguage switch
        {
            LocalizationManager.Language.English => $"All the guards are defeated, Press {keyName} to open the door",
            LocalizationManager.Language.Turkish => $"Tüm muhafızlar yenildi, Kapıyı açmak için {keyName} tuşuna basın",
            _ => $"Все надзиратели побеждены, Нажмите {keyName} чтобы открыть дверь"
        };
    }
}
