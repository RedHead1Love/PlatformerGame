using Cainos.LucidEditor;
using DoorControl;
using UnityEngine;

namespace ChestControl
{
    public sealed class Chest : MonoBehaviour
    {
        private const KeyCode DefaultInteractKey = KeyCode.E;
        private const float DefaultCheckRadius = 0.5f;
        private const float DefaultKeySpawnForce = 2f;
        private const float GizmoSphereRadius = 0.2f;

        [FoldoutGroup("Reference")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [FoldoutGroup("Reference")]
        [SerializeField] private Sprite _spriteOpened;

        [FoldoutGroup("Reference")]
        [SerializeField] private Sprite _spriteClosed;

        [FoldoutGroup("Interaction")]
        [SerializeField] private KeyCode _interactKey = DefaultInteractKey;

        [FoldoutGroup("Interaction")]
        [SerializeField] private float _checkRadius = DefaultCheckRadius;

        [FoldoutGroup("Interaction")]
        [SerializeField] private LayerMask _playerLayer;

        [FoldoutGroup("Key Settings")]
        [SerializeField] private GameObject _keyPrefab;

        [FoldoutGroup("Key Settings")]
        [SerializeField] private DoorControl.KeyColor _keyColor = DoorControl.KeyColor.WhiteColor;

        [FoldoutGroup("Key Settings")]
        [SerializeField] private Vector2 _keySpawnOffset = new Vector2(0f, 1f);

        [FoldoutGroup("Key Settings")]
        [SerializeField] private float _keySpawnForce = DefaultKeySpawnForce;

        [FoldoutGroup("Sound Settings")]
        [SerializeField] private AudioClip _openSound;

        [FoldoutGroup("Sound Settings")]
        [SerializeField] private AudioClip _keySpawnSound;

        [FoldoutGroup("Sound Settings")]
        [SerializeField] private float _soundVolume = 1f;

        private bool _isOpened;
        private bool _isPlayerInRange;

        private void Update()
        {
            if (_isPlayerInRange && !_isOpened && Input.GetKeyDown(_interactKey))
            {
                OpenChest();
            }
        }

        private void OpenChest()
        {
            _isOpened = true;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = _spriteOpened;
            }

            PlaySound(_openSound);
            SpawnKey();
        }

        private void SpawnKey()
        {
            if (_keyPrefab == null)
            {
                return;
            }

            Vector3 spawnPosition = transform.position + (Vector3)_keySpawnOffset;
            GameObject keyInstance = Instantiate(_keyPrefab, spawnPosition, Quaternion.identity);

            PlaySound(_keySpawnSound);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, _soundVolume);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            int bitShiftAmount = 1;
            int noLayerMatch = 0;

            if ((_playerLayer.value & (bitShiftAmount << other.gameObject.layer)) != noLayerMatch)
            {
                _isPlayerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            int bitShiftAmount = 1;
            int noLayerMatch = 0;

            if ((_playerLayer.value & (bitShiftAmount << other.gameObject.layer)) != noLayerMatch)
            {
                _isPlayerInRange = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _checkRadius);

            if (_keyPrefab != null)
            {
                Gizmos.color = Color.green;

                Vector3 spawnPosition = transform.position + (Vector3)_keySpawnOffset;

                Gizmos.DrawWireSphere(spawnPosition, GizmoSphereRadius);
                Gizmos.DrawLine(transform.position, spawnPosition);
            }
        }
    }
}
