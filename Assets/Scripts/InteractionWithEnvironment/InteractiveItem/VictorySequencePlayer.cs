using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public sealed class VictorySequencePlayer : IDisposable
{
    private const string MainMenuSceneName = "MainMenu";
    private readonly MonoBehaviour _host;
    private readonly VideoPlayer _victoryVideoPlayer;
    private readonly GameObject _videoPanel;
    private readonly string _videoFileName;
    private readonly AudioController _audioController;
    private readonly Action _hideDoor;
    private Coroutine _videoCoroutine;
    private bool _isVictorySequencePlaying;

    public bool IsPlaying => _isVictorySequencePlaying;

    public VictorySequencePlayer(MonoBehaviour host, VideoPlayer player, GameObject panel,
        string fileName, AudioController audio, Action hideDoor)
    {
        _host = host;
        _victoryVideoPlayer = player;
        _videoPanel = panel;
        _videoFileName = fileName;
        _audioController = audio;
        _hideDoor = hideDoor;
    }

    public void Dispose()
    {
        if (_videoCoroutine != null)
        {
            _host.StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }
        if (_victoryVideoPlayer != null)
        {
            _victoryVideoPlayer.loopPointReached -= OnVideoFinished;
            _victoryVideoPlayer.errorReceived -= OnVideoError;
        }
        if (_isVictorySequencePlaying)
        {
            _isVictorySequencePlaying = false;
            MiniMapController.IsCutscenePlaying = false;
            if (_victoryVideoPlayer != null)
            {
                _victoryVideoPlayer.Stop();
            }
            if (_videoPanel != null)
            {
                _videoPanel.SetActive(false);
            }
        }
    }

    public void Play()
    {
        _isVictorySequencePlaying = true;
        MiniMapController.IsCutscenePlaying = true;

        _hideDoor();

        if (_audioController != null)
        {
            _audioController.PauseBackgroundMusic();
            _audioController.StopBossMusic();
        }

        if (_victoryVideoPlayer != null && _videoPanel != null)
        {
            ClearRenderTexture();

            _videoPanel.SetActive(true);
            AudioListener.pause = false;

            _victoryVideoPlayer.Stop();
            _victoryVideoPlayer.source = VideoSource.Url;
            _victoryVideoPlayer.url = Application.streamingAssetsPath + "/" + _videoFileName.Trim();

            _victoryVideoPlayer.loopPointReached -= OnVideoFinished;
            _victoryVideoPlayer.loopPointReached += OnVideoFinished;
            _victoryVideoPlayer.errorReceived -= OnVideoError;
            _victoryVideoPlayer.errorReceived += OnVideoError;

            _victoryVideoPlayer.Play();

            if (_videoCoroutine != null) _host.StopCoroutine(_videoCoroutine);
            _videoCoroutine = _host.StartCoroutine(VideoWatchdogRoutine());
        }
        else
        {
            FinishGameAndReturnToMenu();
        }
    }

    private IEnumerator VideoWatchdogRoutine()
    {
        float startWait = 5f;
        while (_isVictorySequencePlaying && !_victoryVideoPlayer.isPlaying && startWait > 0f)
        {
            startWait -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (_isVictorySequencePlaying && !_victoryVideoPlayer.isPlaying)
        {
            SkipOrEndVideo();
            yield break;
        }

        float maxPlayTime = 60f;
        while (_isVictorySequencePlaying && _victoryVideoPlayer.isPlaying && maxPlayTime > 0f)
        {
            maxPlayTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (_isVictorySequencePlaying)
        {
            SkipOrEndVideo();
        }
    }

    private void OnVideoFinished(VideoPlayer player) { SkipOrEndVideo(); }

    private void OnVideoError(VideoPlayer player, string message) { SkipOrEndVideo(); }

    public void SkipOrEndVideo()
    {
        if (!_isVictorySequencePlaying) return;
        _isVictorySequencePlaying = false;
        MiniMapController.IsCutscenePlaying = false;

        if (_videoCoroutine != null)
        {
            _host.StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }

        if (_victoryVideoPlayer != null)
        {
            _victoryVideoPlayer.loopPointReached -= OnVideoFinished;
            _victoryVideoPlayer.errorReceived -= OnVideoError;
            _victoryVideoPlayer.Stop();
            ClearRenderTexture();
        }

        if (_videoPanel != null) _videoPanel.SetActive(false);

        FinishGameAndReturnToMenu();
    }

    private void ClearRenderTexture()
    {
        if (_victoryVideoPlayer == null || _victoryVideoPlayer.targetTexture == null) return;

        RenderTexture target = _victoryVideoPlayer.targetTexture;
        if (!target.IsCreated()) target.Create();

        RenderTexture previousTexture = RenderTexture.active;
        RenderTexture.active = target;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = previousTexture;
    }

    private void FinishGameAndReturnToMenu()
    {
        Dispose();
        GameProgressResetter.Reset();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetNewGameFlag();
        }

        SceneManager.LoadScene(MainMenuSceneName);
    }
}
