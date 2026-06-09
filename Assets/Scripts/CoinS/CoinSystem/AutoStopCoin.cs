using GameLogic;
using UnityEngine;

public sealed class AutoStopCoin : MonoBehaviour
{
    private const float DefaultStopDelay = 0.5f;
    private const float DefaultMinVelocity = 0.1f;
    private const float DefaultMaxLifetime = 10f;
    private const float CheckInitialDelay = 0.1f;
    private const float CheckRepeatInterval = 0.2f;
    private const float MaxLifetimeSeconds = 3f;
    private const float VelocityDampingFactor = 0.7f;
    private const float CheckDelay = 0.2f;
    private const float AngularVelocityZero = 0f;
    private const float SelfDestructDelay = 0.1f;
    private const string GroundLayerName = "Ground";

    [Header("Auto Stop Settings")]
    [SerializeField] private float _stopDelay = DefaultStopDelay;
    [SerializeField] private float _minVelocity = DefaultMinVelocity;
    [SerializeField] private float _maxLifetime = DefaultMaxLifetime;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ICoin _coin;
    private float _spawnTime;
    private bool _isStopped;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _coin = GetComponent<ICoin>();
        _spawnTime = Time.time;

        Invoke(nameof(StartStopCheck), _stopDelay);
        Destroy(gameObject, _maxLifetime);
    }

    private void StartStopCheck()
    {
        InvokeRepeating(nameof(CheckForStop), CheckInitialDelay, CheckRepeatInterval);
    }

    private void CheckForStop()
    {
        if (_isStopped || _rigidbody == null)
        {
            return;
        }

        if (_rigidbody.velocity.magnitude < _minVelocity || Time.time - _spawnTime > MaxLifetimeSeconds)
        {
            StopCoin();
        }
    }

    private void StopCoin()
    {
        if (_isStopped || _rigidbody == null)
        {
            return;
        }

        _isStopped = true;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = AngularVelocityZero;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        if (_collider != null)
        {
            _collider.isTrigger = true;
        }

        if (_coin != null)
        {
            _coin.EnableCollection();
        }

        CancelInvoke(nameof(CheckForStop));
        Destroy(this, SelfDestructDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GroundLayerName) && _rigidbody != null)
        {
            _rigidbody.velocity *= VelocityDampingFactor;
            Invoke(nameof(CheckForStop), CheckDelay);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
