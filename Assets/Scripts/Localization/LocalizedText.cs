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
            LocalizationManager.OnLanguageChanged += UpdateText;
            UpdateText(LocalizationManager.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText(GameLanguage language)
        {
            if (_textComponent == null)
            {
                return;
            }

            switch (language)
            {
            case GameLanguage.Russian:
                _textComponent.text = _russianText;

                break;

            case GameLanguage.English:
                _textComponent.text = _englishText;

                break;

            case GameLanguage.Turkish:
                _textComponent.text = _turkishText;

                break;
            }
        }
    }
}
