using Player;
using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
    private const float DefaultLerpSpeed = 5f;
    private const float DefaultZPosition = -10f;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, DefaultZPosition);
    [SerializeField] private float _lerpSpeed = DefaultLerpSpeed;

    private void Awake()
    {
        if (_playerTransform == null)
        {
            _playerTransform = FindFirstObjectByType<Hero>()?.transform;
        }
    }

    private void LateUpdate()
    {
        if (_playerTransform == null) return;

        Vector3 targetPosition = _playerTransform.position + _offset;
        targetPosition.z = DefaultZPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _lerpSpeed * Time.deltaTime);
    }
}
