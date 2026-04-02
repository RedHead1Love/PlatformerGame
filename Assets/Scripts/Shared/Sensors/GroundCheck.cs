using UnityEngine;

namespace Shared.Sensors
{
    public sealed class GroundCheck : MonoBehaviour
    {
        private const float DefaultCheckRadius = 0.1f;

        [SerializeField] private Transform _checkPoint;
        [SerializeField] private float _checkRadius = DefaultCheckRadius;
        [SerializeField] private LayerMask _groundLayerMask;

        public void SetLayers(LayerMask layerMask) => _groundLayerMask = layerMask;

        public bool IsGrounded => Physics2D.OverlapCircle(_checkPoint.position, _checkRadius, _groundLayerMask);

        private void OnDrawGizmosSelected()
        {
            if (_checkPoint == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_checkPoint.position, _checkRadius);
        }
    }
}