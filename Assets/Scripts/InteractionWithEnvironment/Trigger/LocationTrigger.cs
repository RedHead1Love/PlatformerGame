using UnityEngine;

public sealed class LocationTrigger2D : MonoBehaviour, ILocationTrigger
{
    [SerializeField] private string locationName = "MiddleLevel";

    public string LocationName => locationName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled == false || other.CompareTag("Player") == false)
        {
            return;
        }

        TriggerLocation();
    }

    public void TriggerLocation()
    {
        MiniMapController miniMap = FindFirstObjectByType<MiniMapController>();

        miniMap?.SetMiniMap(locationName);
    }

    private void OnDrawGizmos()
    {
        const float gizmosDepth = 0.1f;

        if (enabled == false)
        {
            return;
        }

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        Vector3 center = transform.position + new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0f);
        Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, gizmosDepth);

        Gizmos.DrawWireCube(center, size);
    }

    [ContextMenu("Test This 2D Trigger")]
    public void TestTrigger()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        Collider2D playerCollider = player.GetComponent<Collider2D>();

        if (playerCollider != null)
        {
            OnTriggerEnter2D(playerCollider);
        }
    }
}