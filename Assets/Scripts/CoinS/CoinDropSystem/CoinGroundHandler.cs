using GameLogic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Coin))]
public sealed class CoinGroundHandler : MonoBehaviour
{
    private const float VelocityMultiplierFactor = 2f;
    private const float StopDelay = 0.5f;
    private const float RotationSpeed = 0f;
    private const float MaxAirTime = 5f;

    [Header("Ground Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _stopVelocityThreshold = 0.1f;
    [SerializeField] private float _bounceDamping = 0.7f;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private Coin _coinScript;
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
        _coinScript = GetComponent<Coin>();
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

            if (_coinScript != null && _coinScript.IsCollectable &&
                _rigidbody.velocity.magnitude < _stopVelocityThreshold * VelocityMultiplierFactor)
            {
                Invoke(nameof(StopPhysics), StopDelay);
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

    private void StopPhysics()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = RotationSpeed;

        if (_collider != null)
        {
            _collider.isTrigger = true;
        }

        if (_coinScript != null)
        {
            _coinScript.EnableCollection();
        }

        enabled = false;
    }

    private bool IsGroundLayer(int layerIndex)
    {
        return ((1 << layerIndex) & _groundLayer) != 0;
    }
}
