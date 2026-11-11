using System.Collections.Generic;
using GameLogic;
using UnityEngine;

namespace DoorControl
{
    public sealed class KeyCollection : MonoBehaviour
    {
        private const float DefaultSpacing = 80f;

        private readonly Dictionary<KeyColor, bool> _collectedKeys = new Dictionary<KeyColor, bool>();

        [SerializeField]
        private RectTransform _keyDisplayPanel;
        [SerializeField]
        private GameObject _keyDisplayPrefab;
        [SerializeField]
        private List<KeySprite> _keySprites = new List<KeySprite>();
        [SerializeField]
        private float _spacing = DefaultSpacing;
        private KeyCollectionView _view;
        private bool _isInitialized;

        public void AddKey(KeyColor color)
        {
            EnsureInitialized();

            _collectedKeys[color] = true;

            _view.UpdateKeyDisplay(color, true);
            WorldProgress.AddKey(color);
        }

        public bool HasKey(KeyColor color)
        {
            EnsureInitialized();

            return _collectedKeys.ContainsKey(color) && _collectedKeys[color];
        }

        public bool UseKey(KeyColor color)
        {
            if (HasKey(color) == false)
            {
                return false;
            }

            _collectedKeys[color] = false;

            _view.UpdateKeyDisplay(color, false);
            WorldProgress.RemoveKey(color);

            return true;
        }

        public bool CanUseKey(KeyColor color)
        {
            return HasKey(color);
        }

        public void ResetAllKeys()
        {
            EnsureInitialized();

            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                _collectedKeys[color] = false;

                _view.UpdateKeyDisplay(color, false);
            }
        }

        public List<KeyColor> GetCollectedKeys()
        {
            EnsureInitialized();

            List<KeyColor> collectedKeys = new List<KeyColor>();

            foreach (KeyValuePair<KeyColor, bool> keyPair in _collectedKeys)
            {
                if (keyPair.Value)
                {
                    collectedKeys.Add(keyPair.Key);
                }
            }

            return collectedKeys;
        }

        public void LoadCollectedKeys(List<KeyColor> collectedKeys)
        {
            if (collectedKeys == null)
            {
                return;
            }

            EnsureInitialized();
            ResetAllKeys();

            foreach (KeyColor color in collectedKeys)
            {
                AddKey(color);
            }
        }

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

            _view = new KeyCollectionView(_keyDisplayPanel, _keyDisplayPrefab, _keySprites, _spacing);
            InitializeKeyStates();
            _view.Initialize();

            _isInitialized = true;
        }

        private void InitializeKeyStates()
        {
            _collectedKeys.Clear();

            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                _collectedKeys[color] = false;
            }
        }

        private void LoadSavedKeys()
        {
            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                if (WorldProgress.HasKey(color))
                {
                    _collectedKeys[color] = true;

                    _view.UpdateKeyDisplay(color, true);
                }
            }
        }

        private void EnsureInitialized()
        {
            if (_isInitialized == false)
            {
                InitializeKeySystem();
            }
        }
    }
}
