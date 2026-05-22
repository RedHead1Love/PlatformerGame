using System.Collections;
using UnityEngine;

namespace GameLogic
{
    public sealed class Coin : MonoBehaviour, ICoin
    {
        private const float DefaultFloatHeight = 0.1f;
        private const float DefaultFloatSpeed = 2f;
        private const float DefaultCollectableDelay = 1f;
        private const float GizmoRadius = 0.3f;
        private const float RetryDelay = 0.1f;
        private const string PlayerTag = "Player";

        [Header("Coin Settings")]
        [SerializeField] private WalletManager.CoinType _coinType = WalletManager.CoinType.Bronze;
        [SerializeField] private int _coinValue = 1;
        [SerializeField] private float _collectableDelay = DefaultCollectableDelay;
        [SerializeField] private AudioClip _collectSound;

        [Header("Visual Effects")]
        [SerializeField] private float _floatHeight = DefaultFloatHeight;
        [SerializeField] private float _floatSpeed = DefaultFloatSpeed;

        private bool _isCollectable;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _originalPosition;
        private Rigidbody2D _rigidbody;

        public WalletManager.CoinType CoinType => _coinType;
        public int CoinValue => _coinValue;
        public bool IsCollectable => _isCollectable;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _originalPosition = transform.position;
        }

        private void Start()
        {
            Invoke(nameof(EnableCollection), _collectableDelay);
        }

        public void EnableCollection()
        {
            if (_isCollectable)
            {
                return;
            }

            if (_rigidbody != null && _rigidbody.bodyType != RigidbodyType2D.Kinematic)
            {
                if (_rigidbody.velocity.magnitude < RetryDelay)
                {
                    _isCollectable = true;
                    _originalPosition = transform.position;
                }
                else
                {
                    Invoke(nameof(EnableCollection), RetryDelay);
                    return;
                }
            }
            else
            {
                _isCollectable = true;
                _originalPosition = transform.position;
            }

            StartCoroutine(FloatAnimationCoroutine());
        }

        private IEnumerator FloatAnimationCoroutine()
        {
            while (true)
            {
                float yOffset = Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;

                transform.position = _originalPosition + new Vector3(0, yOffset, 0);

                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isCollectable || !other.CompareTag(PlayerTag))
            {
                return;
            }

            Collect();
        }

        public void Collect()
        {
            if (WalletManager.Instance != null)
            {
                WalletManager.Instance.AddCoins(_coinType, _coinValue);
            }

            if (_collectSound != null)
            {
                AudioSource.PlayClipAtPoint(_collectSound, transform.position);
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            if (_isCollectable)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, GizmoRadius);
            }
        }
    }
}
