using UnityEngine;

namespace GeneralLogicEnemies
{
    public sealed class EnemyRegistration : MonoBehaviour
    {
        private const string DefaultIdFormat = "{0}_{1:F3}_{2:F3}";
        [SerializeField]
        private string _enemyId;
        private Entity _enemyEntity;
        private bool _isRegistered;
        private bool _isDead;
        private bool _isQuitting;

        public void SetEnemyId(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "-1")
            {
                return;
            }

            _enemyId = id;
        }

        public string GetEnemyId()
        {
            GenerateEnemyIdIfEmpty();
            return _enemyId;
        }

        private void Start()
        {
            InitializeEnemyRegistration();
        }

        public void MarkEnemyKilled()
        {
            if (_isDead || _isRegistered == false || _isQuitting)
            {
                return;
            }

            _isDead = true;
            EnemyRegistry.Instance?.MarkEnemyKilled(_enemyId);
        }

        private void OnDestroy()
        {
            if (_isDead || _isRegistered == false || _isQuitting)
            {
                return;
            }

            EnemyRegistry.Instance?.RemoveEnemy(_enemyId);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void InitializeEnemyRegistration()
        {
            _enemyEntity = GetComponent<Entity>();
            if (_enemyEntity == null || EnemyRegistry.Instance == null)
            {
                return;
            }

            GenerateEnemyIdIfEmpty();
            string registeredId = EnemyRegistry.Instance.RegisterEnemy(_enemyEntity, _enemyId);
            if (string.IsNullOrWhiteSpace(registeredId))
            {
                return;
            }

            _enemyId = registeredId;
            _isRegistered = true;
        }

        private void GenerateEnemyIdIfEmpty()
        {
            if (string.IsNullOrWhiteSpace(_enemyId) == false && _enemyId != "-1")
            {
                return;
            }

            string cleanObjectName = gameObject.name.Replace("(Clone)", string.Empty).Trim();
            _enemyId = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                 DefaultIdFormat,
                 cleanObjectName,
                 transform.position.x,
                 transform.position.y);
        }
    }
}
