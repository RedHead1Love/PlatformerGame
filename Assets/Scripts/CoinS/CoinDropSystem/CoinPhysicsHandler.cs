using GameLogic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public sealed class CoinPhysicsHandler : MonoBehaviour, ICoinPhysicsHandler
{
    private const float DefaultStopVelocityThreshold = 0.1f;
    private const float DefaultBounceDamping = 0.7f;
    private const float MaxAirTime = 5f;
    private const float VelocityMultiplierFactor = 2f;
    private const float StopDelaySeconds = 0.5f;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _stopVelocityThreshold = DefaultStopVelocityThreshold;
    [SerializeField] private float _bounceDamping = DefaultBounceDamping;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ICoin _coin;
    private bool _isOnGround;
    private float _timeInAir;

    public void Initialize(LayerMask groundLayer)
    {
        _groundLayer = groundLayer;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _coin = GetComponent<ICoin>();
    }

    private void Update()
    {
        if (!_isOnGround)
        {
            _timeInAir += Time.deltaTime;

            if (_timeInAir > MaxAirTime)
            {
                StopPhysics();
            }
        }
        else if (_rigidbody.velocity.magnitude < _stopVelocityThreshold)
        {
            StopPhysics();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            _isOnGround = true;

            _rigidbody.velocity *= _bounceDamping;
            _rigidbody.angularVelocity *= _bounceDamping;

            if (_coin != null && _coin.IsCollectable &&
                _rigidbody.velocity.magnitude < _stopVelocityThreshold * VelocityMultiplierFactor)
            {
                Invoke(nameof(StopPhysics), StopDelaySeconds);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            _isOnGround = false;
        }
    }

    public void StopPhysics()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        if (_collider != null)
        {
            _collider.isTrigger = true;
        }

        if (_coin != null)
        {
            _coin.EnableCollection();
        }

        enabled = false;
    }

    private bool IsGroundLayer(int layerIndex)
    {
        return ((1 << layerIndex) & _groundLayer) != 0;
    }
}
