using UnityEngine;

namespace Shared.Sensors
{
    public sealed class GroundCheck : MonoBehaviour
    {
        private const float DefaultCheckRadius = 0.1f;

        [SerializeField] private Transform _checkPoint;
        [SerializeField] private float _checkRadius = DefaultCheckRadius;
        [SerializeField] private LayerMask _groundLayerMask;

        public bool IsGrounded
        {
            get
            {
                Transform point = _checkPoint != null ? _checkPoint : transform;

                return Physics2D.OverlapCircle(point.position, _checkRadius, _groundLayerMask);
            }
        }

        private void Awake()
        {
            if (_checkPoint == null)
            {
                _checkPoint = transform;
            }
        }

        public void SetLayers(LayerMask layerMask)
        {
            _groundLayerMask = layerMask;
        }

        private void OnDrawGizmosSelected()
        {
            Transform point = _checkPoint != null ? _checkPoint : transform;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point.position, _checkRadius);
        }
    }
}