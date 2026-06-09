using GeneralLogicEnemies;
using UnityEngine;

public sealed class EnemyRegistration : MonoBehaviour
{
    private const string DefaultIdFormat = "{0}_{1:F3}_{2:F3}";
    private const float DeathSaveDelay = 0.1f;

    [SerializeField] private string _enemyId;

    private Entity _enemyEntity;
    private bool _isRegistered;
    private bool _isDead;
    private bool _isQuitting;

    private void Start()
    {
        InitializeEnemyRegistration();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEntityDeath();

        if (_isDead || _isRegistered == false || _isQuitting)
        {
            return;
        }

        EnemyManager.Instance?.RemoveEnemy(_enemyId);
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    public void SetEnemyId(string id)
    {
        if (string.IsNullOrEmpty(id))
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

    private void InitializeEnemyRegistration()
    {
        _enemyEntity = GetComponent<Entity>();

        if (_enemyEntity == null || EnemyManager.Instance == null)
        {
            return;
        }

        GenerateEnemyIdIfEmpty();

        _enemyEntity.OnEntityDeath -= OnEnemyDeath;
        _enemyEntity.OnEntityDeath += OnEnemyDeath;

        EnemyManager.Instance.RegisterEnemy(_enemyEntity, _enemyId);

        _isRegistered = true;
    }

    private void GenerateEnemyIdIfEmpty()
    {
        if (string.IsNullOrEmpty(_enemyId) == false)
        {
            return;
        }

        string cleanObjectName = gameObject.name.Replace("(Clone)", string.Empty).Trim();

        _enemyId = string.Format(
            DefaultIdFormat,
            cleanObjectName,
            transform.position.x,
            transform.position.y);
    }

    private void OnEnemyDeath(Entity enemy)
    {
        if (_isDead || _isQuitting)
        {
            return;
        }

        _isDead = true;

        Invoke(nameof(SaveEnemyDeath), DeathSaveDelay);
    }

    private void SaveEnemyDeath()
    {
        if (string.IsNullOrEmpty(_enemyId))
        {
            return;
        }

        EnemyManager.Instance?.MarkEnemyKilled(_enemyId);
        SaveSystem.Instance?.MarkEnemyKilled(_enemyId);
    }

    private void UnsubscribeFromEntityDeath()
    {
        if (_enemyEntity != null)
        {
            _enemyEntity.OnEntityDeath -= OnEnemyDeath;
        }
    }
}