using UnityEngine;

public sealed class LevelStatsTracker : MonoBehaviour
{
    public static LevelStatsTracker Instance { get; private set; }

    public float TimeInSeconds { get; private set; }
    public int TotalDamageTaken { get; private set; }
    public int TotalDeaths { get; private set; }
    public int TotalScore { get; private set; }

    private bool _isTracking = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        HealthManager.OnDamageTakenGlobal += AddDamage;
        Hero.OnHeroDiedGlobal += AddDeath;
        EnemyManager.OnEnemyKilledGlobal += AddScore;
        BossDoor.OnBossDoorOpenedGlobal += CompleteLevel;
    }

    private void OnDisable()
    {
        HealthManager.OnDamageTakenGlobal -= AddDamage;
        Hero.OnHeroDiedGlobal -= AddDeath;
        EnemyManager.OnEnemyKilledGlobal -= AddScore;
        BossDoor.OnBossDoorOpenedGlobal -= CompleteLevel;
    }

    private void Update()
    {
        if (_isTracking)
        {
            TimeInSeconds += Time.deltaTime;
        }
    }

    private void AddDamage(int amount)
    {
        if (_isTracking) TotalDamageTaken += amount;
    }

    private void AddDeath()
    {
        if (_isTracking) TotalDeaths++;
    }

    private void AddScore()
    {
        if (_isTracking) TotalScore += 100; 
    }

    private void CompleteLevel()
    {
        if (_isTracking == false) return;

        _isTracking = false;
        Debug.Log("Уровень завершен. Таймер остановлен.");
    }
}