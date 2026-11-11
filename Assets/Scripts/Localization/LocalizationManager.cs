using System;
using UnityEngine;
using YG;

namespace GameLogic
{
    public static class LocalizationManager
    {
        private const string LanguageKey = "Language";
        private const string ManualOverrideKey = "LangManualOverride";

        private static bool _isApplyingLanguage = false;

        public static event Action<GameLanguage> OnLanguageChanged;

        public static GameLanguage CurrentLanguage { get; private set; }

        public static void SetLanguage(GameLanguage newLanguage)
        {
            if (_isApplyingLanguage)
                return;

            CurrentLanguage = newLanguage;

            PlayerPrefs.SetInt(LanguageKey, (int)newLanguage);
            PlayerPrefs.SetInt(ManualOverrideKey, 1);
            PlayerPrefs.Save();

            ApplyToYG(newLanguage);
            OnLanguageChanged?.Invoke(CurrentLanguage);
        }

        public static string GetLocalizedVideoUrl(string fileNameRu, string fileNameEn, string fileNameTr = "")
        {
            string selectedFile = CurrentLanguage switch
            {
                GameLanguage.Russian => fileNameRu,
                GameLanguage.Turkish => string.IsNullOrEmpty(fileNameTr) ? fileNameEn : fileNameTr,
                _ => string.IsNullOrEmpty(fileNameEn) ? fileNameRu : fileNameEn,
            };

            if (string.IsNullOrEmpty(selectedFile))
            {
                selectedFile = fileNameRu;
            }

            return Application.streamingAssetsPath + "/" + selectedFile.Trim();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (PlayerPrefs.GetInt(ManualOverrideKey, 0) == 1)
            {
                int savedLang = PlayerPrefs.GetInt(LanguageKey, (int)GameLanguage.Russian);
                CurrentLanguage = (GameLanguage)savedLang;
            }
            else
            {
                CurrentLanguage = CodeToLanguage(YG2.lang);
            }

            YG2.onGetSDKData += OnSDKDataReceived;
        }

        private static void OnSDKDataReceived()
        {
            if (PlayerPrefs.GetInt(ManualOverrideKey, 0) == 1)
            {
                int savedLang = PlayerPrefs.GetInt(LanguageKey, (int)GameLanguage.Russian);
                CurrentLanguage = (GameLanguage)savedLang;

                ApplyToYG(CurrentLanguage);
                OnLanguageChanged?.Invoke(CurrentLanguage);
            }
            else
            {
                CurrentLanguage = CodeToLanguage(YG2.lang);

                PlayerPrefs.SetInt(LanguageKey, (int)CurrentLanguage);
                PlayerPrefs.Save();

                ApplyToYG(CurrentLanguage);
                OnLanguageChanged?.Invoke(CurrentLanguage);
            }
        }

        private static void ApplyToYG(GameLanguage lang)
        {
            _isApplyingLanguage = true;
            string code = LanguageToCode(lang);

            try
            {
                YG2.SwitchLanguage(code);
            }
            finally
            {
                _isApplyingLanguage = false;
            }
        }

        private static GameLanguage CodeToLanguage(string code)
        {
            return code switch
            {
                "ru" => GameLanguage.Russian,
                "tr" => GameLanguage.Turkish,
                _ => GameLanguage.English,
            };
        }

        private static string LanguageToCode(GameLanguage lang)
        {
            return lang switch
            {
                GameLanguage.Russian => "ru",
                GameLanguage.Turkish => "tr",
                _ => "en",
            };
        }
    }
}
