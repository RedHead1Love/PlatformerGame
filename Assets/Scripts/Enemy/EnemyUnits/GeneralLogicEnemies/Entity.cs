using UnityEngine;

namespace GeneralLogicEnemies
{
    public abstract class Entity : MonoBehaviour
    {
        private const int DefaultMaxLives = 6;
        private const float DefaultDeathAnimationDelay = 1f;
        private const float GrayColorValue = 0.5f;
        private const float GrayAlphaValue = 0.7f;

        [SerializeField] private int _lives = DefaultMaxLives;
        [SerializeField] private float _deathAnimationDelay = DefaultDeathAnimationDelay;

        public int MaxLives { get; protected set; } = DefaultMaxLives;
        public bool IsDead { get; protected set; }
        public bool IsAlive => _lives > 0 && !IsDead;

        protected int Lives
        {
            get => _lives;
            set => _lives = value;
        }

        public event System.Action<Entity> OnEntityDeath;

        protected virtual void Awake()
        {
            IsDead = false;
        }

        public virtual void TakeDamage(int amount)
        {
            int deathThreshold = 1;

            if (IsDead)
            {
                return;
            }

            _lives -= amount;

            if (_lives < deathThreshold)
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

            OnEntityDeath?.Invoke(this);

            DisablePhysics();
            PlayDeathAnimation();
            MarkEnemyAsKilled();

            Invoke(nameof(DestroyAfterAnimation), _deathAnimationDelay);
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
                entityRigidbody.simulated = false;
            }
        }

        protected virtual void MarkEnemyAsKilled()
        {
            EnemyRegistration enemyRegistration = GetComponent<EnemyRegistration>();

            if (enemyRegistration != null)
            {
                string enemyId = enemyRegistration.GetEnemyId();

                if (!string.IsNullOrEmpty(enemyId))
                {
                    if (EnemyManager.Instance != null)
                    {
                        EnemyManager.Instance.MarkEnemyKilled(enemyId);
                    }
                }
            }
        }

        protected virtual void DestroyAfterAnimation()
        {
            Destroy(gameObject);
        }
    }
}
