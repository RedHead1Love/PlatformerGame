using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoorControl
{
    internal sealed class KeyCollectionView
    {
        private const float CollectedAlpha = 1f;
        private const float UncollectedAlpha = 0.1f;

        private readonly RectTransform _keyDisplayPanel;
        private readonly GameObject _keyDisplayPrefab;
        private readonly List<KeySprite> _keySprites;
        private readonly float _spacing;
        private readonly Dictionary<KeyColor, Image> _keyDisplayImages = new Dictionary<KeyColor, Image>();
        private readonly Dictionary<KeyColor, Sprite> _keySpriteMap = new Dictionary<KeyColor, Sprite>();
        private readonly Dictionary<KeyColor, Sprite> _collectedSpriteMap = new Dictionary<KeyColor, Sprite>();

        public KeyCollectionView(RectTransform panel, GameObject prefab, List<KeySprite> sprites, float spacing)
        {
            _keyDisplayPanel = panel;
            _keyDisplayPrefab = prefab;
            _keySprites = sprites;
            _spacing = spacing;
        }

        public void Initialize()
        {
            InitializeSpriteMappings();
            CreateKeyDisplay();
        }

        public void UpdateKeyDisplay(KeyColor color, bool isCollected)
        {
            if (_keyDisplayImages.TryGetValue(color, out Image displayImage) == false || displayImage == null)
            {
                return;
            }

            UpdateDisplaySprite(displayImage, color, isCollected);
            UpdateDisplayAlpha(displayImage, isCollected);
        }

        private void InitializeSpriteMappings()
        {
            _keySpriteMap.Clear();
            _collectedSpriteMap.Clear();

            foreach (KeySprite keySprite in _keySprites)
            {
                if (keySprite == null)
                {
                    continue;
                }

                _keySpriteMap[keySprite.Color] = keySprite.Sprite;
                _collectedSpriteMap[keySprite.Color] =
                    keySprite.CollectedSprite != null ? keySprite.CollectedSprite : keySprite.Sprite;
            }
        }

        private void CreateKeyDisplay()
        {
            if (_keyDisplayPanel == null || _keyDisplayPrefab == null)
            {
                return;
            }

            ClearExistingDisplay();

            int keyIndex = 0;

            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                CreateKeyDisplayItem(color, keyIndex);

                keyIndex++;
            }
        }

        private void ClearExistingDisplay()
        {
            foreach (Transform child in _keyDisplayPanel)
            {
                Object.Destroy(child.gameObject);
            }

            _keyDisplayImages.Clear();
        }

        private void CreateKeyDisplayItem(KeyColor color, int index)
        {
            GameObject displayObject = Object.Instantiate(_keyDisplayPrefab, _keyDisplayPanel);

            ConfigureKeyDisplayObject(displayObject, color, index);

            Image keyImage = displayObject.GetComponent<Image>();

            if (keyImage == null)
            {
                return;
            }

            _keyDisplayImages[color] = keyImage;
            InitializeKeyDisplay(color, keyImage);
        }

        private void ConfigureKeyDisplayObject(GameObject displayObject, KeyColor color, int index)
        {
            displayObject.name = $"{color}Key";
            displayObject.SetActive(true);

            RectTransform rectTransform = displayObject.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchoredPosition = new Vector2(index * _spacing, 0f);
            rectTransform.localScale = Vector3.one;
        }

        private void InitializeKeyDisplay(KeyColor color, Image image)
        {
            image.enabled = true;

            if (_keySpriteMap.ContainsKey(color))
            {
                image.sprite = _keySpriteMap[color];
            }

            UpdateKeyDisplay(color, false);
        }

        private void UpdateDisplaySprite(Image image, KeyColor color, bool isCollected)
        {
            if (isCollected && _collectedSpriteMap.ContainsKey(color))
            {
                image.sprite = _collectedSpriteMap[color];

                return;
            }

            if (_keySpriteMap.ContainsKey(color))
            {
                image.sprite = _keySpriteMap[color];
            }
        }

        private void UpdateDisplayAlpha(Image image, bool isCollected)
        {
            Color imageColor = image.color;

            imageColor.a = isCollected ? CollectedAlpha : UncollectedAlpha;

            image.color = imageColor;
        }
    }
}
