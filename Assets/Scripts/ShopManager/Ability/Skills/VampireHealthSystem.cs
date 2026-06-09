using GeneralLogicEnemies;
using Player.Abilities;
using UnityEngine;

public sealed class VampireHealthSystem : MonoBehaviour, IVampireHealthSystem
{
    private const int DefaultHealthPerKill = 1;
    private const float EnemyCheckInterval = 2f;

    [SerializeField] private int _healthPerKill = DefaultHealthPerKill;
    [SerializeField] private bool _showHealEffect = true;

    private Hero _hero;
    private HealthManager _healthManager;
    private AbilityManager _abilityManager;
    private bool _isActive;
    private float _checkTimer;

    public int HealthPerKill => _healthPerKill;
    public bool IsActive => _isActive;

    private void Awake()
    {
        InitializeReferences();
    }

    private void Start()
    {
        CheckIfAbilityPurchased();
    }

    private void OnEnable()
    {
        if (_isActive)
        {
            SubscribeToAllEnemies();
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromAllEnemies();
    }

    private void Update()
    {
        if (_isActive == false)
        {
            return;
        }

        _checkTimer += Time.deltaTime;

        if (_checkTimer < EnemyCheckInterval)
        {
            return;
        }

        _checkTimer = 0f;

        SubscribeToAllEnemies();
    }

    public void Activate()
    {
        _isActive = true;

        RefreshAbilityManagerReference();

        if (_abilityManager != null)
        {
            _abilityManager.HasVampireAbility = true;
        }

        SubscribeToAllEnemies();
    }

    public void Deactivate()
    {
        _isActive = false;

        UnsubscribeFromAllEnemies();
    }

    private void InitializeReferences()
    {
        _hero = GetComponent<Hero>();
        _healthManager = GetComponent<HealthManager>();
        _abilityManager = _hero?.AbilityManager;
    }

    private void CheckIfAbilityPurchased()
    {
        RefreshAbilityManagerReference();

        _isActive = _abilityManager?.HasVampireAbility ?? false;

        if (_isActive)
        {
            SubscribeToAllEnemies();
        }
    }

    private void RefreshAbilityManagerReference()
    {
        if (_abilityManager == null && _hero != null)
        {
            _abilityManager = _hero.AbilityManager;
        }
    }

    private void SubscribeToAllEnemies()
    {
        Entity[] enemies = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (Entity enemy in enemies)
        {
            SubscribeToEnemy(enemy);
        }
    }

    private void UnsubscribeFromAllEnemies()
    {
        Entity[] enemies = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (Entity enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEntityDeath -= OnEnemyKilled;
            }
        }
    }

    private void SubscribeToEnemy(Entity enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.OnEntityDeath -= OnEnemyKilled;
        enemy.OnEntityDeath += OnEnemyKilled;
    }

    private void OnEnemyKilled(Entity enemy)
    {
        if (CanHealFromKill() == false)
        {
            return;
        }

        _healthManager.Heal(_healthPerKill);
    }

    private bool CanHealFromKill()
    {
        return _isActive &&
               _healthManager != null &&
               _hero != null &&
               _hero.IsAlive() &&
               _healthManager.IsFullHealth == false;
    }
}