using GameLogic;
using UnityEngine;

namespace DoorControl
{
    public static class BossDoorText
    {
        public static string GetAllBossesDefeatedMessage()
        {
            return LocalizationManager.CurrentLanguage switch
            {
                GameLanguage.English => "The last boss is defeated! Head to the locked door.",
                GameLanguage.Turkish => "Son patron yenildi! Kilitli kapıya git.",
                _ => "Последний босс повержен! Направляйтесь к запертой двери.",
            };
        }

        public static string GetBossStatusMessage(int aliveBossCount, int bossCount, KeyCode interactionKey)
        {
            int defeatedCount = bossCount - aliveBossCount;

            if (aliveBossCount > 0)
            {
                return LocalizationManager.CurrentLanguage switch
            {
                    GameLanguage.English => $"({defeatedCount}/{bossCount}) bosses killed",
                    GameLanguage.Turkish => $"({defeatedCount}/{bossCount}) patron öldürüldü",
                    _ => $"({defeatedCount}/{bossCount}) боссов убито",
                };
            }

            string keyName =
                LocalizationManager.CurrentLanguage == GameLanguage.Russian ? "А (F)" : interactionKey.ToString();

            return LocalizationManager.CurrentLanguage switch
            {
                GameLanguage.English => $"All the guards are defeated, Press {keyName} to open the door",
                GameLanguage.Turkish => $"Tüm muhafızlar yenildi, Kapıyı açmak için {keyName} tuşuna basın",
                _ => $"Все надзиратели побеждены, Нажмите {keyName} чтобы открыть дверь",
            };
        }
    }
}
