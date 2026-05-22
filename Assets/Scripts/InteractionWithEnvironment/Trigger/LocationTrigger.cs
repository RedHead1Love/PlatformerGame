using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class LocationTrigger : MonoBehaviour, ILocationTrigger
{
    private const string PlayerTag = "Player";
    private const float GizmosDepth = 0.1f;
    private const float GizmosZOffset = 0f;

    [SerializeField] private string _locationName = "MiddleLevel";

    public string LocationName => _locationName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || !other.CompareTag(PlayerTag))
        {
            return;
        }

        TriggerLocation();
    }

    public void TriggerLocation()
    {
        MiniMapController miniMap = FindFirstObjectByType<MiniMapController>();

        if (miniMap != null)
        {
            miniMap.SetMiniMap(_locationName);
        }
    }

    private void OnDrawGizmos()
    {
        if (!enabled)
        {
            return;
        }

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        Vector3 center = transform.position + new Vector3(boxCollider.offset.x, boxCollider.offset.y, GizmosZOffset);
        Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, GizmosDepth);

        Gizmos.DrawWireCube(center, size);
    }

    [ContextMenu("Test This 2D Trigger")]
    public void TestTrigger()
    {
        GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);

        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();

            if (playerCollider != null)
            {
                OnTriggerEnter2D(playerCollider);
            }
        }
    }
}
