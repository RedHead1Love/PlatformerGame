using Cainos.LucidEditor;
using System.Collections;
using UnityEngine;

namespace DoorControl
{
    public sealed class Key : MonoBehaviour
    {
        private const float EffectDestroyDelay = 2f;
        private const float RotationSpeed = 360f;
        private const float ScaleMultiplier = 0.2f;
        private const float CollectDuration = 0.5f;

        [FoldoutGroup("Key Settings")]
        [SerializeField] private KeyColor _keyColor = KeyColor.WhiteColor;

        [FoldoutGroup("Collection Effects")]
        [SerializeField] private AudioClip _collectSound;

        [FoldoutGroup("Collection Effects")]
        [SerializeField] private float _collectSoundVolume = 0.5f;

        private bool _isCollected;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;

        public KeyColor Color => _keyColor;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();

            if (_collider != null)
            {
                _collider.isTrigger = true;
            }

            ApplyKeyColor();
        }

        private void ApplyKeyColor()
        {
            if (_spriteRenderer != null)
            {
                switch (_keyColor)
                {
                    case KeyColor.WhiteColor:
                        _spriteRenderer.color = UnityEngine.Color.white;
                        break;
                    case KeyColor.BlackColor:
                        _spriteRenderer.color = UnityEngine.Color.black;
                        break;
                    case KeyColor.YellowColor:
                        _spriteRenderer.color = UnityEngine.Color.yellow;
                        break;
                    case KeyColor.BlueColor:
                        _spriteRenderer.color = UnityEngine.Color.blue;
                        break;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCollected || !other.CompareTag("Player"))
            {
                return;
            }

            Collect(other.gameObject);
        }

        private void Collect(GameObject player)
        {
            _isCollected = true;

            if (_collider != null)
            {
                _collider.enabled = false;
            }

            AddKeyToPlayer(player);
            StartCoroutine(CollectAnimationCoroutine());
        }

        private void AddKeyToPlayer(GameObject player)
        {
            KeyCollection keyCollection = player.GetComponent<KeyCollection>();

            if (keyCollection == null)
            {
                keyCollection = player.GetComponentInParent<KeyCollection>();
            }

            if (keyCollection != null)
            {
                keyCollection.AddKey(_keyColor);
            }
        }

        private IEnumerator CollectAnimationCoroutine()
        {
            PlayCollectEffects();

            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < CollectDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / CollectDuration;

                transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
                transform.localScale = originalScale * (1 + progress * ScaleMultiplier);

                if (_spriteRenderer != null)
                {
                    UnityEngine.Color color = _spriteRenderer.color;
                    color.a = 1 - progress;
                    _spriteRenderer.color = color;
                }

                yield return null;
            }

            Destroy(gameObject);
        }

        private void PlayCollectEffects()
        {
            if (_collectSound != null)
            {
                AudioSource.PlayClipAtPoint(_collectSound, transform.position, _collectSoundVolume);
            }
        }
    }
}
