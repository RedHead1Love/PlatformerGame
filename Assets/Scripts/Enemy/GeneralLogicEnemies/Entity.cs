using UnityEngine;

namespace GeneralLogicEnemies
{
    public abstract class Entity : MonoBehaviour
    {
        private const int DefaultMaxLives = 6;
        private const float DefaultDeathAnimationDelay = 1f;
        private const float GrayColorValue = 0.5f;
        private const float GrayAlphaValue = 0.7f;
        private const int DeathThreshold = 0;

        [SerializeField]
        private int _lives = DefaultMaxLives;
        [SerializeField]
        private float _deathAnimationDelay = DefaultDeathAnimationDelay;

        public event System.Action<Entity> Died;

        public int MaxLives { get; protected set; } = DefaultMaxLives;

        public bool IsDead { get; protected set; }

        public bool IsAlive => _lives > DeathThreshold && IsDead == false;

        public int CurrentLives
        {
            get => _lives;
            protected set => _lives = value;
        }

        public virtual void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0)
            {
                return;
            }

            _lives = Mathf.Max(DeathThreshold, _lives - amount);

            if (_lives <= DeathThreshold)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;

            Died?.Invoke(this);

            MarkEnemyAsKilled();
            DisablePhysics();
            PlayDeathAnimation();

            Invoke(nameof(DestroyAfterAnimation), _deathAnimationDelay);
        }

        protected virtual void Awake()
        {
            IsDead = false;
            MaxLives = Mathf.Max(MaxLives, _lives);
        }

        protected virtual void PlayDeathAnimation()
        {
            Animator animator = GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(GrayColorValue, GrayColorValue, GrayColorValue, GrayAlphaValue);
            }
        }

        protected virtual void DisablePhysics()
        {
            Collider2D entityCollider = GetComponent<Collider2D>();

            if (entityCollider != null)
            {
                entityCollider.enabled = false;
            }

            Rigidbody2D entityRigidbody = GetComponent<Rigidbody2D>();

            if (entityRigidbody != null)
            {
                entityRigidbody.velocity = Vector2.zero;
                entityRigidbody.simulated = false;
            }
        }

        protected virtual void MarkEnemyAsKilled()
        {
            EnemyRegistration enemyRegistration = GetComponent<EnemyRegistration>();

            if (enemyRegistration == null)
            {
                return;
            }

            string enemyId = enemyRegistration.GetEnemyId();

            if (string.IsNullOrEmpty(enemyId))
            {
                return;
            }

            EnemyRegistry.Instance?.MarkEnemyKilled(enemyId);
        }

        protected virtual void DestroyAfterAnimation()
        {
            Destroy(gameObject);
        }
    }
}
