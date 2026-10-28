using System;
using UnityEngine;
using UnityEngine.Video;

public sealed class BossIntroSequence : IDisposable
{
    private readonly MonoBehaviour _host;
    private readonly VideoPlayer _introVideoPlayer;
    private readonly GameObject _videoPanel;
    private readonly string _videoFileName;
    private readonly BossHealthBar _healthBar;
    private readonly AudioController _audioController;
    private readonly Action _stopMovement;
    private readonly Action _startMusic;
    private readonly Func<bool> _isAlive;
    private Coroutine _videoCoroutine;
    private bool _isVideoPlaying;
    private bool _hasPlayedIntro;

    public bool IsPlaying => _isVideoPlaying;
    public bool HasPlayed => _hasPlayedIntro;

    public BossIntroSequence(MonoBehaviour host, VideoPlayer player, GameObject panel,
        string fileName, BossHealthBar healthBar, AudioController audio,
        Action stopMovement, Action startMusic, Func<bool> isAlive)
    {
        _host = host;
        _introVideoPlayer = player;
        _videoPanel = panel;
        _videoFileName = fileName;
        _healthBar = healthBar;
        _audioController = audio;
        _stopMovement = stopMovement;
        _startMusic = startMusic;
        _isAlive = isAlive;
    }

    public void Dispose()
    {
        if (_videoCoroutine != null)
        {
            _host.StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }
        if (_introVideoPlayer != null)
        {
            _introVideoPlayer.loopPointReached -= OnVideoFinished;
            _introVideoPlayer.errorReceived -= OnVideoError;
        }
        if (_isVideoPlaying)
        {
            _isVideoPlaying = false;
            MiniMapController.IsCutscenePlaying = false;
            if (_introVideoPlayer != null)
            {
                _introVideoPlayer.Stop();
            }
            if (_videoPanel != null)
            {
                _videoPanel.SetActive(false);
            }
        }
    }

    public void Play()
    {
        _hasPlayedIntro = true;
        _isVideoPlaying = true;
        MiniMapController.IsCutscenePlaying = true;
        _healthBar?.SetVisible(false);

        _audioController?.PauseBackgroundMusic();

        _stopMovement();

        if (_introVideoPlayer != null && _videoPanel != null)
        {
            ClearRenderTexture();

            _videoPanel.SetActive(true);
            AudioListener.pause = false;

            _introVideoPlayer.Stop();
            _introVideoPlayer.source = VideoSource.Url;
            _introVideoPlayer.url = Application.streamingAssetsPath + "/" + _videoFileName.Trim();

            _introVideoPlayer.loopPointReached -= OnVideoFinished;
            _introVideoPlayer.loopPointReached += OnVideoFinished;

            _introVideoPlayer.errorReceived -= OnVideoError;
            _introVideoPlayer.errorReceived += OnVideoError;

            _introVideoPlayer.Play();

            if (_videoCoroutine != null)
            {
                _host.StopCoroutine(_videoCoroutine);
            }

            _videoCoroutine = _host.StartCoroutine(VideoWatchdogRoutine());
        }
        else
        {
            Finish();
        }
    }

    private System.Collections.IEnumerator VideoWatchdogRoutine()
    {
        float startWait = 5f;
        while (_isVideoPlaying && !_introVideoPlayer.isPlaying && startWait > 0f)
        {
            startWait -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (_isVideoPlaying && !_introVideoPlayer.isPlaying)
        {
            Finish();
            yield break;
        }

        float maxPlayTime = 30f;
        while (_isVideoPlaying && _introVideoPlayer.isPlaying && maxPlayTime > 0f)
        {
            maxPlayTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (_isVideoPlaying)
        {
            Finish();
        }
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        Finish();
    }

    private void OnVideoError(VideoPlayer player, string message)
    {
        Finish();
    }

    public void Finish()
    {
        if (!_isVideoPlaying) return;
        _isVideoPlaying = false;
        MiniMapController.IsCutscenePlaying = false;

        if (_videoCoroutine != null)
        {
            _host.StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }

        if (_introVideoPlayer != null)
        {
            _introVideoPlayer.loopPointReached -= OnVideoFinished;
            _introVideoPlayer.errorReceived -= OnVideoError;
            _introVideoPlayer.Stop();
            ClearRenderTexture();
        }

        if (_videoPanel != null)
        {
            _videoPanel.SetActive(false);
        }

        _healthBar?.SetVisible(true);

        if (_isAlive())
        {
            _startMusic();
        }
    }

    private void ClearRenderTexture()
    {
        if (_introVideoPlayer == null || _introVideoPlayer.targetTexture == null)
        {
            return;
        }

        RenderTexture target = _introVideoPlayer.targetTexture;
        if (!target.IsCreated())
        {
            target.Create();
        }

        RenderTexture previousTexture = RenderTexture.active;
        RenderTexture.active = target;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = previousTexture;
    }
}
