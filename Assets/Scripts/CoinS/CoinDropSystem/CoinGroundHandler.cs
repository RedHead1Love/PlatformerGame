using UnityEngine;

public sealed class CoinGroundHandler : MonoBehaviour
{
    private const float DefaultStopVelocity = 0.1f;
    private const float GroundCheckDelay = 0.15f;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _stopVelocity = DefaultStopVelocity;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private bool _isGrounded;
    private bool _isStopped;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_isStopped || _isGrounded == false || _rigidbody == null)
        {
            return;
        }

        if (_rigidbody.velocity.magnitude <= _stopVelocity)
        {
            StopOnGround();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject.layer) == false)
        {
            return;
        }

        _isGrounded = true;

        Invoke(nameof(CheckStopAfterGroundHit), GroundCheckDelay);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            _isGrounded = false;
        }
    }

    private void CheckStopAfterGroundHit()
    {
        if (_rigidbody != null && _rigidbody.velocity.magnitude <= _stopVelocity)
        {
            StopOnGround();
        }
    }

    private void StopOnGround()
    {
        if (_isStopped || _rigidbody == null)
        {
            return;
        }

        _isStopped = true;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        if (_collider != null)
        {
            _collider.isTrigger = true;
        }
    }

    private bool IsGroundLayer(int layer)
    {
        return (_groundLayer.value & (1 << layer)) != 0;
    }
}