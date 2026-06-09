using GeneralLogicEnemies;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyDeathHandler : MonoBehaviour, IEnemyDeathHandler
{
    [Header("Death Settings")]
    [SerializeField] private bool _disableColliderOnDeath = true;
    [SerializeField] private bool _disablePhysicsOnDeath = true;
    [SerializeField] private bool _disableAIOnDeath = true;

    private Entity _entity;
    private EnemyRegistration _registration;
    private Collider2D[] _colliders;
    private Rigidbody2D _rigidbody;
    private MonoBehaviour[] _componentsToDisable;

    private void Awake()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        if (_entity != null)
        {
            _entity.OnEntityDeath += OnEntityDeath;
        }
    }

    private void OnDisable()
    {
        if (_entity != null)
        {
            _entity.OnEntityDeath -= OnEntityDeath;
        }
    }

    public void DisableColliderOnDeath(bool disable)
    {
        _disableColliderOnDeath = disable;
    }

    public void DisablePhysicsOnDeath(bool disable)
    {
        _disablePhysicsOnDeath = disable;
    }

    public void DisableAIOnDeath(bool disable)
    {
        _disableAIOnDeath = disable;
    }

    private void InitializeComponents()
    {
        _entity = GetComponent<Entity>();
        _registration = GetComponent<EnemyRegistration>();
        _colliders = GetComponents<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _componentsToDisable = CollectComponentsToDisable();
    }

    private MonoBehaviour[] CollectComponentsToDisable()
    {
        MonoBehaviour[] allComponents = GetComponents<MonoBehaviour>();
        List<MonoBehaviour> result = new List<MonoBehaviour>();

        foreach (MonoBehaviour component in allComponents)
        {
            if (component == null)
            {
                continue;
            }

            if (component == this ||
                component == _entity ||
                component == _registration ||
                component is EnemyDeathHandler ||
                component is EnemyRegistration ||
                component is CoinDropSystem)
            {
                continue;
            }

            result.Add(component);
        }

        return result.ToArray();
    }

    private void OnEntityDeath(Entity entity)
    {
        DisableComponents();
    }

    private void DisableComponents()
    {
        DisableCollidersIfNeeded();
        DisablePhysicsIfNeeded();
        DisableAIIfNeeded();
    }

    private void DisableCollidersIfNeeded()
    {
        if (_disableColliderOnDeath == false || _colliders == null)
        {
            return;
        }

        foreach (Collider2D enemyCollider in _colliders)
        {
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
        }
    }

    private void DisablePhysicsIfNeeded()
    {
        if (_disablePhysicsOnDeath == false || _rigidbody == null)
        {
            return;
        }

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.simulated = false;
    }

    private void DisableAIIfNeeded()
    {
        if (_disableAIOnDeath == false || _componentsToDisable == null)
        {
            return;
        }

        foreach (MonoBehaviour component in _componentsToDisable)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }
}