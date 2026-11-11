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
            LocalizationManager.OnLanguageChanged += UpdateImage;
            UpdateImage(LocalizationManager.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= UpdateImage;
        }

        private void UpdateImage(GameLanguage language)
        {
            if (_imageComponent == null)
            {
                return;
            }

            switch (language)
            {
            case GameLanguage.Russian:
                if (_russianSprite != null)
                {
                    _imageComponent.sprite = _russianSprite;
                }

                break;

            case GameLanguage.English:
                if (_englishSprite != null)
                {
                    _imageComponent.sprite = _englishSprite;
                }

                break;

            case GameLanguage.Turkish:
                if (_turkishSprite != null)
                {
                    _imageComponent.sprite = _turkishSprite;
                }

                break;
            }
        }
    }
}
