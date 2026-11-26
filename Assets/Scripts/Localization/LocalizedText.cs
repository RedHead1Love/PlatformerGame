using GameLogic;
using TMPro;
using UnityEngine;

namespace Foxberry
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        [TextArea(2, 5)]
        [SerializeField]
        private string _russianText;
        [TextArea(2, 5)]
        [SerializeField]
        private string _englishText;
        [TextArea(2, 5)]
        [SerializeField]
        private string _turkishText;
        private TMP_Text _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
            OnLanguageChanged(LocalizationManager.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(GameLanguage language)
        {
            if (_textComponent == null)
            {
                return;
            }

            _textComponent.text = LocalizedValue.Select(language, _russianText, _englishText, _turkishText);
        }
    }
}
