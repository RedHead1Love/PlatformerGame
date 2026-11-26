using UnityEngine;

namespace Foxberry
{
    internal sealed class BossLocomotion
    {
        private readonly Transform _origin;
        private readonly Rigidbody2D _body;
        private readonly bool _preserveScale;

        public BossLocomotion(Transform origin, Rigidbody2D body, bool preserveScale)
        {
            _origin = origin;
            _body = body;
            _preserveScale = preserveScale;
        }

        public void MoveTowards(Transform target, float speed)
        {
            if (target == null || _body == null)
            {
                return;
            }

            float direction = Mathf.Sign(target.position.x - _origin.position.x);
            _body.velocity = new Vector2(direction * speed, _body.velocity.y);
            Vector3 scale = _origin.localScale;
            _origin.localScale = _preserveScale
                ? new Vector3(Mathf.Abs(scale.x) * direction, scale.y, scale.z)
                : new Vector3(direction,
                 1f,
                 1f);
        }

        public void Stop()
        {
            if (_body != null)
            {
                _body.velocity = Vector2.zero;
            }
        }
    }
}
