using GameLogic;
using UnityEngine;

namespace Foxberry
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class LocalizedSprite : MonoBehaviour
    {
        [SerializeField]
        private Sprite _russianSprite;
        [SerializeField]
        private Sprite _englishSprite;
        [SerializeField]
        private Sprite _turkishSprite;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += UpdateSprite;
            UpdateSprite(LocalizationManager.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= UpdateSprite;
        }

        private void UpdateSprite(GameLanguage language)
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            switch (language)
            {
            case GameLanguage.Russian:
                if (_russianSprite != null)
                {
                    _spriteRenderer.sprite = _russianSprite;
                }

                break;

            case GameLanguage.English:
                if (_englishSprite != null)
                {
                    _spriteRenderer.sprite = _englishSprite;
                }

                break;

            case GameLanguage.Turkish:
                if (_turkishSprite != null)
                {
                    _spriteRenderer.sprite = _turkishSprite;
                }

                break;
            }
        }
    }
}
