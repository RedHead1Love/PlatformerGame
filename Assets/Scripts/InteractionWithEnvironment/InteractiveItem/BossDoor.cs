using System.Collections.Generic;
using System;
using UnityEngine.Video;
using UnityEngine;

public sealed class BossDoor : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const float MessageDisplayDuration = 2f;
    private const float AllBossesDefeatedMessageDuration = 4.5f;
    private const KeyCode InteractionKey = KeyCode.F;
    private const float MessageDisplayDistance = 3f;
    private const float PausedTimeScaleThreshold = 0.01f;

    [SerializeField] private string[] _bossIds = { "226", "227", "228", "229" };

    [SerializeField] private GameObject _interactionMessagePrefab;
    [SerializeField] private GameObject _temporaryMessagePrefab;
    [SerializeField] private int _messageSortingOrder = -1;

    [SerializeField] private GameObject[] _blockingUIObjects;

    [SerializeField] private VideoPlayer _victoryVideoPlayer;
    [SerializeField] private GameObject _videoPanel;
    [SerializeField] private string _videoFileName = "Victory.mp4";

    public static event Action OnBossDoorOpenedGlobal;

    private BossDoorMessageView _messages;
    private Transform _playerTransform;
    private AudioController _audioController;
    private bool _isPlayerNear;
    private bool _isOpened;
    private bool _hasShownAllBossesDefeatedMessage;

    private VictorySequencePlayer _victory;

    private void OnEnable()
    {
        EnemyManager.OnEnemyKilledGlobal += OnEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyManager.OnEnemyKilledGlobal -= OnEnemyKilled;
    }

    private void Start()
    {
        InitializeReferences();
        _victory = new VictorySequencePlayer(this, _victoryVideoPlayer, _videoPanel,
            _videoFileName, _audioController, DisableDoorVisualsAndColliders);
        _messages = new BossDoorMessageView(this, _interactionMessagePrefab,
            _temporaryMessagePrefab, _messageSortingOrder, IsMenuOrPauseActive);
        _messages.Initialize();

        if (_videoPanel != null) _videoPanel.SetActive(false);

        if (AreAllBossesKilledInSave())
        {
            _hasShownAllBossesDefeatedMessage = true;
        }
    }

    private void Update()
    {
        if (_victory != null && _victory.IsPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) _victory.SkipOrEndVideo();
            return;
        }

        _messages?.RefreshVisibility();

        if (_playerTransform == null || _isOpened || IsMenuOrPauseActive())
        {
            return;
        }

        UpdatePlayerProximity();
        HandlePlayerInteraction();
    }

    private void OnDestroy()
    {
        _messages?.Dispose();
        _victory?.Dispose();
    }

    private bool IsMenuOrPauseActive()
    {
        if (Time.timeScale < PausedTimeScaleThreshold)
        {
            return true;
        }

        if (_blockingUIObjects != null)
        {
            foreach (GameObject uiObject in _blockingUIObjects)
            {
                if (uiObject != null && uiObject.activeInHierarchy)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnEnemyKilled()
    {
        if (_hasShownAllBossesDefeatedMessage || _isOpened)
        {
            return;
        }

        if (AreAllBossesKilledInSave())
        {
            _hasShownAllBossesDefeatedMessage = true;
            _messages?.ShowTemporaryMessage(BossDoorText.GetAllBossesDefeatedMessage(), AllBossesDefeatedMessageDuration);

            if (_isPlayerNear)
            {
                ShowBossStatus();
            }
        }
    }

    private bool AreAllBossesKilledInSave()
    {
        if (EnemyManager.Instance == null || _bossIds == null || _bossIds.Length == 0)
        {
            return false;
        }

        HashSet<string> killedEnemies = EnemyManager.Instance.GetKilledEnemies();

        foreach (string bossId in _bossIds)
        {
            if (killedEnemies.Contains(bossId) == false)
            {
                return false;
            }
        }

        return true;
    }

    private void InitializeReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(PlayerTag);

        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }

        _audioController = FindFirstObjectByType<AudioController>();
    }

    private void UpdatePlayerProximity()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
        bool wasPlayerNear = _isPlayerNear;

        _isPlayerNear = distanceToPlayer <= MessageDisplayDistance;

        if (_isPlayerNear && wasPlayerNear == false)
        {
            ShowBossStatus();
        }
        else if (_isPlayerNear == false && wasPlayerNear)
        {
            HideBossStatus();
        }
    }

    private void HandlePlayerInteraction()
    {
        if (_isPlayerNear && Input.GetKeyDown(InteractionKey))
        {
            TryOpenDoor();
        }
    }

    private void ShowBossStatus()
    {
        int aliveBossCount = CountAliveBosses();
        string statusMessage = BossDoorText.GetBossStatusMessage(aliveBossCount, _bossIds.Length, InteractionKey);

        _messages?.ShowInteractionMessage(statusMessage);
    }

    private void HideBossStatus()
    {
        _messages?.HideInteractionMessage();
    }

    private void TryOpenDoor()
    {
        int aliveBossCount = CountAliveBosses();

        if (aliveBossCount == 0)
        {
            OpenDoor();
            return;
        }

        int defeatedCount = _bossIds.Length - aliveBossCount;

        string message = LocalizationManager.CurrentLanguage switch
        {
            LocalizationManager.Language.English => $"Not all bosses are defeated. ({defeatedCount}/{_bossIds.Length})",
            LocalizationManager.Language.Turkish => $"Tüm patronlar yenilmedi. ({defeatedCount}/{_bossIds.Length})",
            _ => $"Не все боссы побеждены. Осталось ({defeatedCount}/{_bossIds.Length})"
        };

        _messages?.ShowTemporaryMessage(message, MessageDisplayDuration);
    }

    private int CountAliveBosses()
    {
        int aliveCount = 0;

        if (EnemyManager.Instance == null)
        {
            return aliveCount;
        }

        foreach (string bossId in _bossIds)
        {
            if (EnemyManager.Instance.IsEnemyAlive(bossId))
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }

    private void OpenDoor()
    {
        _isOpened = true;
        _messages?.Dispose();
        OnBossDoorOpenedGlobal?.Invoke();
        _audioController?.PlayBossDoorOpenSound();

        _victory.Play();
    }

    private void DisableDoorVisualsAndColliders()
    {
        foreach (Renderer rendererComponent in GetComponentsInChildren<Renderer>())
        {
            rendererComponent.enabled = false;
        }

        foreach (Collider2D collider2DComponent in GetComponentsInChildren<Collider2D>())
        {
            collider2DComponent.enabled = false;
        }

        foreach (Collider colliderComponent in GetComponentsInChildren<Collider>())
        {
            colliderComponent.enabled = false;
        }
    }

}
