using UnityEngine;

namespace GeneralLogicEnemies
{
    [RequireComponent(typeof(Entity))]
    public sealed class CoinDropSystem : MonoBehaviour, ICoinDropSystem
    {
        private const float DefaultDropRadius = 1f;
        private const float DefaultVerticalForce = 4f;
        private const float DefaultHorizontalForce = 2f;
        private const float DefaultTorqueForce = 100f;
        private const float DefaultBounciness = 0.3f;
        private const float SpawnVerticalOffset = 0.5f;
        private const int RandomRangeMaxOffset = 1;

        [Header("Coin Drop Settings")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int _minCoins = 1;
        [SerializeField] private int _maxCoins = 3;

        [Header("Physics Settings")]
        [SerializeField] private float _dropRadius = DefaultDropRadius;
        [SerializeField] private float _verticalForce = DefaultVerticalForce;
        [SerializeField] private float _horizontalForce = DefaultHorizontalForce;
        [SerializeField] private float _torqueForce = DefaultTorqueForce;
        [SerializeField] private LayerMask _groundLayer = 1 << 6;
        [SerializeField] private float _bounciness = DefaultBounciness;

        private Entity _entity;
        private bool _hasDroppedCoins;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (_entity != null)
            {
                _entity.OnEntityDeath -= HandleEntityDeath;
            }
        }

        public void InitializeDropSettings(GameObject coinPrefab, int minCoins, int maxCoins)
        {
            _coinPrefab = coinPrefab;
            _minCoins = Mathf.Max(0, minCoins);
            _maxCoins = Mathf.Max(_minCoins, maxCoins);
        }

        public void DropCoins()
        {
            if (_hasDroppedCoins || _coinPrefab == null)
            {
                return;
            }

            int coinCount = Random.Range(_minCoins, _maxCoins + RandomRangeMaxOffset);

            for (int i = 0; i < coinCount; i++)
            {
                SpawnCoin();
            }

            _hasDroppedCoins = true;
        }

        private void Initialize()
        {
            _entity = GetComponent<Entity>();

            if (_entity == null)
            {
                return;
            }

            _entity.OnEntityDeath -= HandleEntityDeath;
            _entity.OnEntityDeath += HandleEntityDeath;
        }

        private void HandleEntityDeath(Entity entity)
        {
            DropCoins();
        }

        private void SpawnCoin()
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-_dropRadius, _dropRadius),
                SpawnVerticalOffset,
                0f);

            GameObject coin = Instantiate(_coinPrefab, spawnPosition, Quaternion.identity);

            if (coin == null)
            {
                return;
            }

            SetupCoinPhysics(coin);
        }

        private void SetupCoinPhysics(GameObject coin)
        {
            Rigidbody2D coinRigidbody = coin.GetComponent<Rigidbody2D>();

            if (coinRigidbody == null)
            {
                coinRigidbody = coin.AddComponent<Rigidbody2D>();
                SetupDefaultRigidbody(coinRigidbody);
            }

            coinRigidbody.sharedMaterial = CreatePhysicsMaterial();

            ApplyRandomForces(coinRigidbody);
            AddPhysicsHandler(coin);
        }

        private PhysicsMaterial2D CreatePhysicsMaterial()
        {
            return new PhysicsMaterial2D("BouncyCoin")
            {
                bounciness = _bounciness,
                friction = 0.1f
            };
        }

        private void SetupDefaultRigidbody(Rigidbody2D coinRigidbody)
        {
            coinRigidbody.bodyType = RigidbodyType2D.Dynamic;
            coinRigidbody.gravityScale = 3f;
            coinRigidbody.mass = 0.1f;
            coinRigidbody.drag = 0.5f;
            coinRigidbody.angularDrag = 0.05f;
            coinRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            coinRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void ApplyRandomForces(Rigidbody2D coinRigidbody)
        {
            float horizontalDirection = Random.Range(-1f, 1f);
            Vector2 force = new Vector2(horizontalDirection * _horizontalForce, _verticalForce);

            coinRigidbody.AddForce(force, ForceMode2D.Impulse);

            float torque = Random.Range(-_torqueForce, _torqueForce);

            coinRigidbody.AddTorque(torque, ForceMode2D.Impulse);
        }

        private void AddPhysicsHandler(GameObject coin)
        {
            CoinPhysicsHandler physicsHandler = coin.GetComponent<CoinPhysicsHandler>();

            if (physicsHandler == null)
            {
                physicsHandler = coin.AddComponent<CoinPhysicsHandler>();
            }

            physicsHandler.Initialize(_groundLayer);
        }
    }
}