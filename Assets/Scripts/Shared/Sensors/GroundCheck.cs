using UnityEngine;

namespace Shared.Sensors
{
    public sealed class GroundCheck : MonoBehaviour
    {
        [SerializeField] private Transform _checkPoint;
        [SerializeField] private Vector2 _checkSize = new Vector2(0.5f, 0.1f);
        [SerializeField] private LayerMask _groundLayerMask;

        public bool IsGrounded
        {
            get
            {
                Transform point = _checkPoint != null ? _checkPoint : transform;

                return Physics2D.OverlapBox(point.position, _checkSize, 0f, _groundLayerMask);
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
            Gizmos.DrawWireCube(point.position, _checkSize);
        }
    }
}