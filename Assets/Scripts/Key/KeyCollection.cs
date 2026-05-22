using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoorControl
{
    public sealed class KeyCollection : MonoBehaviour
    {
        private const float DefaultSpacing = 80f;
        private const float CollectedAlpha = 1f;
        private const float UncollectedAlpha = 0.1f;

        [System.Serializable]
        public sealed class KeySprite
        {
            public KeyColor Color;
            public Sprite Sprite;
            public Sprite CollectedSprite;
        }

        [SerializeField] private RectTransform _keyDisplayPanel;
        [SerializeField] private GameObject _keyDisplayPrefab;
        [SerializeField] private List<KeySprite> _keySprites = new List<KeySprite>();
        [SerializeField] private float _spacing = DefaultSpacing;

        private Dictionary<KeyColor, bool> _collectedKeys = new Dictionary<KeyColor, bool>();
        private Dictionary<KeyColor, Image> _keyDisplayImages = new Dictionary<KeyColor, Image>();
        private Dictionary<KeyColor, Sprite> _keySpriteMap = new Dictionary<KeyColor, Sprite>();
        private Dictionary<KeyColor, Sprite> _collectedSpriteMap = new Dictionary<KeyColor, Sprite>();
        private bool _isInitialized;

        private void Start()
        {
            InitializeKeySystem();
            LoadSavedKeys();
        }

        private void InitializeKeySystem()
        {
            if (_isInitialized)
            {
                return;
            }

            InitializeSpriteMaps();
            InitializeCollectedKeysMap();
            CreateKeyDisplayUI();

            _isInitialized = true;
        }

        private void InitializeSpriteMaps()
        {
            _keySpriteMap.Clear();
            _collectedSpriteMap.Clear();

            foreach (var spriteData in _keySprites)
            {
                if (spriteData.Sprite != null)
                {
                    _keySpriteMap[spriteData.Color] = spriteData.Sprite;
                }

                if (spriteData.CollectedSprite != null)
                {
                    _collectedSpriteMap[spriteData.Color] = spriteData.CollectedSprite;
                }
            }
        }

        private void InitializeCollectedKeysMap()
        {
            _collectedKeys.Clear();

            foreach (var spriteData in _keySprites)
            {
                _collectedKeys[spriteData.Color] = false;
            }
        }

        private void CreateKeyDisplayUI()
        {
            if (_keyDisplayPanel == null || _keyDisplayPrefab == null)
            {
                return;
            }

            ClearExistingUI();

            int index = 0;

            foreach (var spriteData in _keySprites)
            {
                CreateSingleKeyDisplay(spriteData.Color, index);
                index++;
            }
        }

        private void ClearExistingUI()
        {
            foreach (Transform child in _keyDisplayPanel)
            {
                Destroy(child.gameObject);
            }

            _keyDisplayImages.Clear();
        }

        private void CreateSingleKeyDisplay(KeyColor color, int index)
        {
            GameObject displayObj = Instantiate(_keyDisplayPrefab, _keyDisplayPanel);
            RectTransform rectTransform = displayObj.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(index * _spacing, 0);
            }

            Image image = displayObj.GetComponent<Image>();

            if (image != null)
            {
                _keyDisplayImages[color] = image;
                UpdateKeyDisplay(color, false);
            }
        }

        public void AddKey(KeyColor color)
        {
            EnsureInitialized();

            _collectedKeys[color] = true;
            GameStateManager.SaveKey(color);
            UpdateKeyDisplay(color, true);
        }

        public void RemoveKey(KeyColor color)
        {
            EnsureInitialized();

            if (_collectedKeys.ContainsKey(color))
            {
                _collectedKeys[color] = false;
                GameStateManager.RemoveKey(color);
                UpdateKeyDisplay(color, false);
            }
        }

        public bool HasKey(KeyColor color)
        {
            EnsureInitialized();

            return _collectedKeys.TryGetValue(color, out bool isCollected) && isCollected;
        }

        public void LoadSavedKeys()
        {
            EnsureInitialized();

            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                if (GameStateManager.HasKey(color))
                {
                    _collectedKeys[color] = true;
                    UpdateKeyDisplay(color, true);
                }
            }
        }

        public void ResetAllKeys()
        {
            EnsureInitialized();

            List<KeyColor> keysToReset = new List<KeyColor>(_collectedKeys.Keys);

            foreach (KeyColor color in keysToReset)
            {
                _collectedKeys[color] = false;
                UpdateKeyDisplay(color, false);
            }
        }

        public void LoadCollectedKeys(List<KeyColor> collectedKeys)
        {
            EnsureInitialized();

            if (collectedKeys == null)
            {
                return;
            }

            ResetAllKeys();

            foreach (KeyColor color in collectedKeys)
            {
                AddKey(color);
            }
        }

        private void UpdateKeyDisplay(KeyColor color, bool isCollected)
        {
            if (!_keyDisplayImages.TryGetValue(color, out Image displayImage) || displayImage == null)
            {
                return;
            }

            UpdateDisplaySprite(displayImage, color, isCollected);
            UpdateDisplayAlpha(displayImage, isCollected);
        }

        private void UpdateDisplaySprite(Image image, KeyColor color, bool isCollected)
        {
            if (isCollected && _collectedSpriteMap.TryGetValue(color, out Sprite collectedSprite))
            {
                image.sprite = collectedSprite;
            }
            else if (_keySpriteMap.TryGetValue(color, out Sprite regularSprite))
            {
                image.sprite = regularSprite;
            }
        }

        private void UpdateDisplayAlpha(Image image, bool isCollected)
        {
            float targetAlpha = isCollected ? CollectedAlpha : UncollectedAlpha;
            UnityEngine.Color imageColor = image.color;
            imageColor.a = targetAlpha;
            image.color = imageColor;
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                InitializeKeySystem();
            }
        }
    }
}
