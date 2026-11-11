using UnityEngine;
using UnityEngine.Serialization;

namespace DoorControl
{
    [System.Serializable]
    public sealed class KeySprite
    {
        [FormerlySerializedAs("color")]
        [SerializeField]
        private KeyColor _color;
        [FormerlySerializedAs("sprite")]
        [SerializeField]
        private Sprite _sprite;
        [FormerlySerializedAs("collectedSprite")]
        [SerializeField]
        private Sprite _collectedSprite;

        public KeyColor Color => _color;

        public Sprite Sprite => _sprite;

        public Sprite CollectedSprite => _collectedSprite;
    }
}
