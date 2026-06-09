using DoorControl;
using System.Collections;
using UnityEngine;

public sealed class SimpleKey : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const float CollectAnimationDuration = 0.5f;
    private const float ScaleMultiplier = 0.2f;
    private const float RotationSpeed = 360f;
    private const float DebugRayLength = 2f;
    private const float GizmoSphereRadius = 0.5f;

    [SerializeField] private KeyColor _keyColor = KeyColor.BlackColor;

    public KeyColor Color => _keyColor;

    private void Start()
    {
        InitializeKey();
    }

    private void Update()
    {
        DrawDebugRay();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerTag))
        {
            return;
        }

        CollectKey(other.gameObject);
    }

    private void OnDrawGizmos()
    {
        DrawGizmoSphere();
    }

    private void InitializeKey()
    {
        EnsureActiveState();
        ConfigureCollider();
        ApplyKeyColor();
    }

    private void EnsureActiveState()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
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

        spriteRenderer.color = GetColorFromEnum(_keyColor);
    }

    private Color GetColorFromEnum(KeyColor keyColor)
    {
        return keyColor switch
        {
            KeyColor.WhiteColor => UnityEngine.Color.white,
            KeyColor.BlackColor => UnityEngine.Color.black,
            KeyColor.YellowColor => UnityEngine.Color.yellow,
            KeyColor.BlueColor => UnityEngine.Color.blue,
            _ => UnityEngine.Color.white
        };
    }

    private void CollectKey(GameObject player)
    {
        AddKeyToPlayer(player);
        StartCoroutine(CollectAnimationCoroutine());
    }

    private void AddKeyToPlayer(GameObject player)
    {
        KeyCollection keyCollection = player.GetComponent<KeyCollection>();

        if (keyCollection == null)
        {
            keyCollection = player.GetComponentInParent<KeyCollection>();
        }

        if (keyCollection != null)
        {
            keyCollection.AddKey(_keyColor);
        }
    }

    private IEnumerator CollectAnimationCoroutine()
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
        transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
        transform.localScale = originalScale * (1 + progress * ScaleMultiplier);

        if (spriteRenderer != null)
        {
            UpdateSpriteAlpha(spriteRenderer, progress);
        }
    }

    private void UpdateSpriteAlpha(SpriteRenderer spriteRenderer, float progress)
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1 - progress;
        spriteRenderer.color = spriteColor;
    }

    private void DrawDebugRay()
    {
        Debug.DrawRay(transform.position, Vector3.up * DebugRayLength, UnityEngine.Color.red);
    }

    private void DrawGizmoSphere()
    {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawWireSphere(transform.position, GizmoSphereRadius);
    }
}
