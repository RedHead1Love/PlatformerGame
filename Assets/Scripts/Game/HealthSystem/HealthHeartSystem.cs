using UnityEngine;

namespace Foxberry
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class HealthHeartSystem : MonoBehaviour
    {
        [SerializeField]
        private Sprite _fullHeartSprite;
        [SerializeField]
        private Sprite _emptyHeartSprite;
        private SpriteRenderer _spriteRenderer;

        public void SetHeartFull(bool isFull)
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            _spriteRenderer.sprite = isFull ? _fullHeartSprite : _emptyHeartSprite;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
