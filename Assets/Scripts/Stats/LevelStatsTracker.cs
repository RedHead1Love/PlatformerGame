using DoorControl;
using GeneralLogicEnemies;
using Player;
using UnityEngine;
using YG;

namespace Foxberry
{
    public sealed class LevelStatsTracker : MonoBehaviour
    {
        private static float _persistentTime = 0f;
        private static int _persistentDamage = 0;
        private static int _persistentDeaths = 0;
        private static int _persistentScore = 0;
        private bool _isTracking = true;

        public static LevelStatsTracker Instance { get; private set; }

        public float TimeInSeconds => _persistentTime;

        public int TotalDamageTaken => _persistentDamage;

        public int TotalDeaths => _persistentDeaths;

        public int TotalScore => _persistentScore;

        public static void ResetAllStats()
        {
            _persistentTime = 0f;
            _persistentDamage = 0;
            _persistentDeaths = 0;
            _persistentScore = 0;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            HealthManager.OnDamageTakenGlobal += OnDamageTaken;
            Hero.OnHeroDiedGlobal += OnHeroDied;
            EnemyRegistry.OnEnemyKilledGlobal += OnEnemyKilled;
            BossDoor.OnBossDoorOpenedGlobal += OnBossDoorOpened;
        }

        private void OnDisable()
        {
            HealthManager.OnDamageTakenGlobal -= OnDamageTaken;
            Hero.OnHeroDiedGlobal -= OnHeroDied;
            EnemyRegistry.OnEnemyKilledGlobal -= OnEnemyKilled;
            BossDoor.OnBossDoorOpenedGlobal -= OnBossDoorOpened;
        }

        private void Update()
        {
            if (_isTracking)
            {
                _persistentTime += Time.deltaTime;
            }
        }

        private void OnDamageTaken(int amount)
        {
            if (_isTracking)
            {
                _persistentDamage += amount;
            }
        }

        private void OnHeroDied()
        {
            if (_isTracking)
            {
                _persistentDeaths++;
            }
        }

        private void OnEnemyKilled()
        {
            if (_isTracking)
            {
                _persistentScore += 100;
            }
        }

        private void OnBossDoorOpened()
        {
            if (_isTracking == false)
            {
                return;
            }

            _isTracking = false;
            StartCoroutine(SendLeaderboardsRoutine());
        }

        private System.Collections.IEnumerator SendLeaderboardsRoutine()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YG2.SetLeaderboard("ScoreBoard", _persistentScore);
#endif
            yield return null;
            ResetAllStats();
        }
    }
}
