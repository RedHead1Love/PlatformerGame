using UnityEngine;

[System.Serializable]
public sealed class MiniMapData
{
    private const float DefaultMapWidth = 50f;
    private const float DefaultMapHeight = 50f;

    [UnityEngine.Serialization.FormerlySerializedAs("locationName")]
    [SerializeField] private string _locationName;
    [UnityEngine.Serialization.FormerlySerializedAs("mapTexture")]
    [SerializeField] private Sprite _mapTexture;
    [UnityEngine.Serialization.FormerlySerializedAs("mapWorldSize")]
    [SerializeField] private Vector2 _mapWorldSize = new Vector2(DefaultMapWidth, DefaultMapHeight);
    [UnityEngine.Serialization.FormerlySerializedAs("mapWorldCenter")]
    [SerializeField] private Vector2 _mapWorldCenter = Vector2.zero;

    [UnityEngine.Serialization.FormerlySerializedAs("mapBoundsCollider")]
    [SerializeField] private Collider2D _mapBoundsCollider;

    public string LocationName => _locationName;
    public Sprite MapTexture => _mapTexture;
    public Vector2 MapWorldSize => _mapWorldSize;
    public Vector2 MapWorldCenter => _mapWorldCenter;
    public Collider2D MapBoundsCollider => _mapBoundsCollider;
}
