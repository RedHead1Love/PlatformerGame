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
            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
            OnLanguageChanged(LocalizationManager.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(GameLanguage language)
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            Sprite selected = LocalizedValue.Select(language, _russianSprite, _englishSprite, _turkishSprite);
            if (selected != null)
            {
                _spriteRenderer.sprite = selected;
            }
        }
    }
}
