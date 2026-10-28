using UnityEngine;

public sealed class MiniMapProjection
{
    private readonly MiniMapLocationSelector _locations;
    private readonly Vector2 _uiSize;

    public MiniMapProjection(MiniMapLocationSelector locations, Vector2 uiSize)
    {
        _locations = locations;
        _uiSize = uiSize;
    }

    public Vector2 ToUIPosition(Vector2 position)
    {
        MiniMapData map = _locations.CurrentMap;
        if (map == null)
        {
            return Vector2.zero;
        }
        Vector2 offset = position - map.MapWorldCenter;
        return new Vector2(ProjectAxis(offset.x, map.MapWorldSize.x, _uiSize.x),
            ProjectAxis(offset.y, map.MapWorldSize.y, _uiSize.y));
    }

    public bool Contains(Vector2 position)
    {
        MiniMapData map = _locations.CurrentMap;
        return map != null && map.MapBoundsCollider != null &&
            map.MapBoundsCollider.OverlapPoint(position);
    }

    private static float ProjectAxis(float offset, float worldSize, float uiSize)
    {
        const float Half = 0.5f;
        if (Mathf.Approximately(worldSize, 0f))
        {
            return 0f;
        }
        return Mathf.Clamp(offset / (worldSize * Half), -1f, 1f) * uiSize * Half;
    }
}
