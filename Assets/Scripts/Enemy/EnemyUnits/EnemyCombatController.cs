using GeneralEnemyPatrolSystem;
using GeneralLogicEnemies;
using Shared.Damage;
using System.Collections;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(Animator))]
    public sealed class EnemyCombatController : Entity, IDamageable
    {
        private const float StrongAttackProbability = 0.5f;
        private const float AttackCooldownDuration = 2f;
        private const int RegularAttackAnimationCount = 4;
        private const float DamageAnimationDelay = 0.3f;
        private const float AttackCheckRadius = 1.5f;
        private const float HitAnimationDuration = 0.5f;
        private const string AnimationStateParameterName = "state";
        private const string PlayerLayerName = "Player";
        private const float BoxRotationAngle = 0f;
        private const float MovementThreshold = 0.01f;
        private const float MinimumDistanceToResetPlayer = 0.01f;
        private const float DirectionThreshold = 0f;
        private const float MaxDetectionRadiusMultiplier = 1.5f;

        [Header("Health Settings")]
        [SerializeField] private int _maxHealth = 2;
        [SerializeField] private int _currentHealth = 2;

        [Header("Attack Settings")]
        [SerializeField] private int _attackDamage = 1;
        [SerializeField] private int _strongAttackDamage = 2;
        [SerializeField] private float _attackCooldown = AttackCooldownDuration;

        [Header("Attack Box Settings")]
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        public int CurrentHealth => _currentHealth;

        public void Heal(int amount)
        {
            if (IsDead)
            {
                return;
            }

            _currentHealth += amount;

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        public void SetMaxHealth(int newMaxHealth)
        {
            _maxHealth = newMaxHealth;

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        private enum EnemyAnimationState
        {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Attack2 = 3,
            Kick = 4,
            DoubleAttack = 5,
            Dead = 6,
            Hit = 7
        }
    }
}
