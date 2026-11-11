using System.Collections;
using AudioSystem;
using Foxberry;
using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;
using UnityEngine.Video;

namespace MiddleBossLogic
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class BossGolem : Entity, IDamageable
    {
        private const float DestroyDelay = 1f;
        private const float DefaultSoundEffectVolume = 1f;

        private static readonly int StateParameterHash = Animator.StringToHash("state");

        [SerializeField]
        private VideoPlayer _introVideoPlayer;
        [SerializeField]
        private GameObject _videoPanel;
        [SerializeField]
        private string _videoFileName = "Glinomes.mp4";
        [SerializeField]
        private Transform _groundCheckPoint;
        [SerializeField]
        private LayerMask _playerLayer;
        [SerializeField]
        private float _detectionRadius = 1f;
        [SerializeField]
        private float _moveSpeed = 2.5f;
        [SerializeField]
        private float _attackRange = 1.5f;
        [SerializeField]
        private float _attackCooldown = 2f;
        [SerializeField]
        private int _attackDamage = 1;
        [SerializeField]
        private float _hitStunDuration = 0.4f;
        [SerializeField]
        private AudioClip _bossMusic;
        [SerializeField]
        private AudioClip _attackSound;
        [SerializeField]
        private AudioClip _attackMissSound;
        [SerializeField]
        private AudioClip _takeDamageSound;
        [SerializeField]
        private AudioClip _deathSound;
        [SerializeField]
        private float _soundEffectVolume = DefaultSoundEffectVolume;
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private Transform _playerTransform;
        private BossAudio _audio;
        private BossHealthBar _healthBar;
        private float _nextAttackTime;
        private bool _isActivated;
        private bool _isHit;
        private bool _isDead;
        private bool _wasAttackSuccessful;
        private BossIntroSequence _intro;

        private BossGolemState CurrentAnimatorState
        {
            get => _animator != null ? (BossGolemState)_animator.GetInteger(StateParameterHash) : BossGolemState.Idle;
            set
            {
                if (_animator != null)
                {
                    _animator.SetInteger(StateParameterHash, (int)value);
                }
            }
        }

        public override void TakeDamage(int amount)
        {
            if (_isDead || _isHit || amount <= 0)
                return;

            PlayTakeDamageSound();
            StartCoroutine(HitRoutine());
            base.TakeDamage(amount);
        }

        public override void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            _isHit = true;

            StopMovement();
            CurrentAnimatorState = BossGolemState.Hit;

            PlayDeathSound();
            _audio?.StopMusic();
            base.Die();

            Destroy(gameObject, DestroyDelay);
        }

        public void DealDamage()
        {
            AnimEvent_AttackHit();
        }

        public void AnimEvent_AttackHit()
        {
            PlayAttackSound();

            Collider2D hit = Physics2D.OverlapCircle(transform.position, _attackRange, _playerLayer);
            if (hit == null)
            {
                _wasAttackSuccessful = false;
                return;
            }

            IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                _wasAttackSuccessful = false;
                return;
            }

            damageable.TakeDamage(_attackDamage);
            _wasAttackSuccessful = true;
        }

        public void AnimEvent_AttackEnd()
        {
            if (_wasAttackSuccessful == false)
                PlayAttackMissSound();
            _nextAttackTime = Time.time + _attackCooldown;
            CurrentAnimatorState = BossGolemState.Idle;
            _wasAttackSuccessful = false;
        }

        public void AnimEvent_AttackStart()
        {
            PlayAttackSound();
        }

        public void AnimEvent_HitEnd()
        {
            _isHit = false;
        }

        public void AnimEvent_DeathEnd()
        {
            Destroy(gameObject);
        }

        protected override void Awake()
        {
            base.Awake();

            _healthBar = GetComponent<BossHealthBar>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            AudioController output = FindFirstObjectByType<AudioController>();
            _audio = new BossAudio(output, _bossMusic);
            _intro = new BossIntroSequence(
                this,
                _introVideoPlayer,
                _videoPanel,
                _videoFileName,
                _healthBar,
                output,
                StopMovement,
                _audio.StartMusic,
                () => !_isDead);

            if (_videoPanel != null)
                _videoPanel.SetActive(false);
        }

        private void Update()
        {
            if (_isDead)
                return;

            if (_intro.IsPlaying)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    _intro.Finish();
                return;
            }

            if (_isActivated == false)
                ActivateIfPossible();

            if (_isActivated == false || _isHit || _playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _attackRange && Time.time >= _nextAttackTime)
            {
                StartAttack();
                return;
            }

            MoveTowardPlayer();
        }

        private void OnDestroy()
        {
            _intro?.Dispose();
            if (_isActivated)
                _audio?.StopMusic();
        }

        private void ActivateIfPossible()
        {
            if (Time.timeSinceLevelLoad < 1.5f)
                return;
            if (_isActivated || _intro.HasPlayed)
                return;
            if (_groundCheckPoint == null)
                return;

            Collider2D playerCollider =
                Physics2D.OverlapCircle(_groundCheckPoint.position, _detectionRadius, _playerLayer);

            if (playerCollider == null)
                return;

            _isActivated = true;
            _playerTransform = playerCollider.transform;

            if (!_intro.HasPlayed && _introVideoPlayer != null && _videoPanel != null)
            {
                _audio?.StopMusic();
                _intro.Play();
            }
            else
            {
                _healthBar?.SetVisible(true);
                _audio.StartMusic();
            }
        }

        private void MoveTowardPlayer()
        {
            if (_playerTransform == null || _rigidbody == null)
                return;

            float direction = Mathf.Sign(_playerTransform.position.x - transform.position.x);
            _rigidbody.velocity = new Vector2(direction * _moveSpeed, _rigidbody.velocity.y);
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * direction,
                transform.localScale.y,
                transform.localScale.z);
            CurrentAnimatorState = BossGolemState.Move;
        }

        private void StartAttack()
        {
            StopMovement();
            CurrentAnimatorState = BossGolemState.Attack;
            _wasAttackSuccessful = false;
        }

        private IEnumerator HitRoutine()
        {
            _isHit = true;
            StopMovement();
            CurrentAnimatorState = BossGolemState.Hit;
            yield return new WaitForSeconds(_hitStunDuration);
            _isHit = false;
        }

        private void StopMovement()
        {
            if (_rigidbody != null)
                _rigidbody.velocity = Vector2.zero;
        }

        private void PlayAttackSound()
        {
            _audio.Play(_attackSound, _soundEffectVolume);
        }

        private void PlayAttackMissSound()
        {
            _audio.Play(_attackMissSound, _soundEffectVolume);
        }

        private void PlayTakeDamageSound()
        {
            _audio.Play(_takeDamageSound, _soundEffectVolume);
        }

        private void PlayDeathSound()
        {
            _audio.Play(_deathSound, _soundEffectVolume);
        }

    }
}
