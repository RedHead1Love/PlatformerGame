using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Foxberry
{
    [RequireComponent(typeof(Image))]
    public sealed class LocalizedImage : MonoBehaviour
    {
        [SerializeField]
        private Sprite _russianSprite;
        [SerializeField]
        private Sprite _englishSprite;
        [SerializeField]
        private Sprite _turkishSprite;
        private Image _imageComponent;

        private void Awake()
        {
            _imageComponent = GetComponent<Image>();
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
            if (_imageComponent == null)
            {
                return;
            }

            Sprite selected = LocalizedValue.Select(language, _russianSprite, _englishSprite, _turkishSprite);
            if (selected != null)
            {
                _imageComponent.sprite = selected;
            }
        }
    }
}
