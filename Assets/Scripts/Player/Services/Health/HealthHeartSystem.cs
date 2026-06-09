using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class HealthHeartSystem : MonoBehaviour
{
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetHeartFull(bool isFull)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = isFull ? _fullHeartSprite : _emptyHeartSprite;
        }
    }
}
