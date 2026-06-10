using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Bounds")]
    [SerializeField] private bool _useBounds = false;
    [SerializeField] private Vector2 _minBounds;
    [SerializeField] private Vector2 _maxBounds;

    private void Start()
    {
        FindTargetIfNeeded();
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        FollowTarget();
    }

    private void FindTargetIfNeeded()
    {
        if (_target != null)
            return;

        if (Hero.Instance != null)
        {
            _target = Hero.Instance.transform;
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
        if (player != null)
        {
            _target = player.transform;
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = _target.position + _offset;
        targetPosition = ClampPositionToBounds(targetPosition);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            _smoothSpeed * Time.deltaTime);
    }

    private Vector3 ClampPositionToBounds(Vector3 position)
    {
        if (!_useBounds)
        {
            return position;
        }

        position.x = Mathf.Clamp(position.x, _minBounds.x, _maxBounds.x);
        position.y = Mathf.Clamp(position.y, _minBounds.y, _maxBounds.y);

        return position;
    }
}