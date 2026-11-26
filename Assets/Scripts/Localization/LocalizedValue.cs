using GameLogic;

namespace Foxberry
{
    internal static class LocalizedValue
    {
        public static T Select<T>(GameLanguage language, T russian, T english, T turkish)
        {
            return language switch
            {
                GameLanguage.English => english,
                GameLanguage.Turkish => turkish,
                _ => russian,
            };
        }
    }
}
