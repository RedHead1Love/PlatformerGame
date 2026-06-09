using DoorControl;
using System.Collections;
using UnityEngine;

public sealed class SimpleKey : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const float CollectAnimationDuration = 0.5f;
    private const float ScaleMultiplier = 0.2f;
    private const float RotationSpeed = 360f;
    private const float GizmoRadius = 0.5f;

    [SerializeField] public KeyColor _keyColor = KeyColor.BlackColor;

    private bool _isCollected;

    private void Start()
    {
        InitializeKey();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isCollected || other.CompareTag(PlayerTag) == false)
        {
            return;
        }

        CollectKey(other.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GizmoRadius);
    }

    public void SetKeyColor(KeyColor keyColor)
    {
        _keyColor = keyColor;

        ApplyKeyColor();
    }

    private void InitializeKey()
    {
        ConfigureCollider();
        ApplyKeyColor();
    }

    private void ConfigureCollider()
    {
        Collider2D keyCollider = GetComponent<Collider2D>();

        if (keyCollider != null)
        {
            keyCollider.isTrigger = true;
        }
    }

    private void ApplyKeyColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = GetColorFromKeyColor(_keyColor);
    }

    private Color GetColorFromKeyColor(KeyColor keyColor)
    {
        return keyColor switch
        {
            KeyColor.WhiteColor => Color.white,
            KeyColor.BlackColor => Color.black,
            KeyColor.YellowColor => Color.yellow,
            KeyColor.BlueColor => Color.blue,
            _ => Color.white
        };
    }

    private void CollectKey(GameObject player)
    {
        _isCollected = true;

        KeyCollection keyCollection = player.GetComponent<KeyCollection>();

        if (keyCollection == null)
        {
            keyCollection = player.GetComponentInParent<KeyCollection>();
        }

        keyCollection?.AddKey(_keyColor);

        StartCoroutine(PlayCollectionAnimation());
    }

    private IEnumerator PlayCollectionAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D keyCollider = GetComponent<Collider2D>();

        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < CollectAnimationDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / CollectAnimationDuration;

            ApplyAnimationTransform(originalScale, progress, spriteRenderer);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void ApplyAnimationTransform(Vector3 originalScale, float progress, SpriteRenderer spriteRenderer)
    {
        transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        transform.localScale = originalScale * (1f + progress * ScaleMultiplier);

        if (spriteRenderer != null)
        {
            UpdateSpriteAlpha(spriteRenderer, progress);
        }
    }

    private void UpdateSpriteAlpha(SpriteRenderer spriteRenderer, float progress)
    {
        Color spriteColor = spriteRenderer.color;

        spriteColor.a = 1f - progress;

        spriteRenderer.color = spriteColor;
    }
}