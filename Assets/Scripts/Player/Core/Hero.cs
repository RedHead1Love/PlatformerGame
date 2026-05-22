using Player;
using Player.Abilities;
using Player.Input;
using Player.StateMachine;
using Shared.Damage;
using Shared.Sensors;
using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public sealed class Hero : MonoBehaviour, IDamageable
    {
        private const string PlayerChildName = "Player";
        private const float ZeroDirectionThreshold = 0.0f;
        private const float WallCheckDistance = 0.5f;

        [Header("Core Hero Parameters")]
        [SerializeField] private HeroData _heroData;

        [Header("Ground Detection")]
        [SerializeField] private Transform _groundCheckPoint;
        [SerializeField] private LayerMask _groundLayerMask;

        [Header("Attack Settings")]
        [SerializeField] private Transform _attackPoint;

        [Header("Wall Detection")]
        [SerializeField] private Transform _leftWallCheckPoint;
        [SerializeField] private Transform _rightWallCheckPoint;

        [Header("Passive Health Regeneration")]
        [SerializeField] private PassiveHealthRegeneration _passiveHealthRegen;

        private DamageService _damageService;
        private SpriteRenderer _spriteRenderer;
        private HealthManager _healthManager;
        private ArmorManager _armorManager;
        private Rigidbody2D _rigidbody;
        private bool _hasPerformedAirAttack;

        public AudioController AudioController { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public HeroStateMachine StateMachine { get; private set; }
        public AnimationService AnimationService { get; private set; }

        public HeroData Data => _heroData;
        public Rigidbody2D Rigidbody => _rigidbody;
        public Transform AttackPoint => _attackPoint;
        public float FacingDirection => _spriteRenderer.flipX ? -1f : 1f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _healthManager = GetComponent<HealthManager>();
            _armorManager = GetComponent<ArmorManager>();

            AudioController = FindFirstObjectByType<AudioController>();
            AbilityManager = new AbilityManager(this);
            StateMachine = new HeroStateMachine(this);
            AnimationService = new AnimationService(GetComponent<Animator>());
            _damageService = new DamageService(this, _healthManager);
        }

        private void Start()
        {
            StateMachine.Change<IdleState>();
        }

        private void Update()
        {
            StateMachine.Tick();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick();
        }

        public void SetHealth(int health)
        {
            _healthManager?.SetHealth(health);
        }

        public void OnAttackHit()
        {
            (StateMachine.Current as IDamageDealer)?.DealDamage();
        }

        public void OnAttackEnd()
        {
            (StateMachine.Current as IAnimationEndHandler)?.OnAnimationEnd();
        }

        public void TakeDamage(int amount)
        {
            _damageService.ApplyDamage(amount);
        }

        public void SetFacing(float direction)
        {
            if (Mathf.Abs(direction) > ZeroDirectionThreshold)
            {
                _spriteRenderer.flipX = direction < ZeroDirectionThreshold;
            }
        }

        public bool HasArmor()
        {
            return _armorManager?.HasArmor ?? false;
        }

        public void AddArmor(int amount)
        {
            _armorManager?.AddArmor(amount);
        }

        public bool IsAlive()
        {
            return _healthManager?.CurrentHealth > 0;
        }

        public bool NeedsHealing()
        {
            return _healthManager != null && _healthManager.CurrentHealth < _healthManager.MaxHealth;
        }

        public bool IsTouchingWall()
        {
            bool leftWallDetected = Physics2D.Raycast(_leftWallCheckPoint.position, Vector2.left, WallCheckDistance, _groundLayerMask);
            bool rightWallDetected = Physics2D.Raycast(_rightWallCheckPoint.position, Vector2.right, WallCheckDistance, _groundLayerMask);

            return leftWallDetected || rightWallDetected;
        }
    }
}
