using UnityEngine;

public sealed class FootstepManager
{
    private const string AudioSourceObjectName = "FootstepAudioSource";

    private readonly AudioSource _audioSource;

    private AudioClip _footstepSound;
    private bool _isMoving;

    public FootstepManager(AudioClip footstepSound, bool isLooping)
    {
        _footstepSound = footstepSound;

        _audioSource = CreateAudioSource(isLooping);
        _audioSource.clip = _footstepSound;
    }

    public bool IsPlaying => _isMoving && _audioSource != null && _audioSource.isPlaying;

    public void Start()
    {
        if (_audioSource == null || _footstepSound == null || _isMoving)
        {
            return;
        }

        _isMoving = true;

        if (_audioSource.isPlaying == false)
        {
            _audioSource.Play();
        }
    }

    public void Stop()
    {
        if (_audioSource == null || _isMoving == false)
        {
            return;
        }

        _isMoving = false;

        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    public void Pause()
    {
        if (_audioSource == null || _audioSource.isPlaying == false)
        {
            return;
        }

        _audioSource.Pause();
    }

    public void Resume()
    {
        if (_audioSource == null || _isMoving == false || _audioSource.isPlaying)
        {
            return;
        }

        _audioSource.Play();
    }

    public void SetSound(AudioClip newClip, bool playImmediately = false)
    {
        if (_audioSource == null)
        {
            return;
        }

        bool wasPlaying = _audioSource.isPlaying;

        if (wasPlaying)
        {
            _audioSource.Stop();
        }

        _footstepSound = newClip;
        _audioSource.clip = newClip;

        if (playImmediately && _isMoving && _footstepSound != null)
        {
            _audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        if (_audioSource == null)
        {
            return;
        }

        _audioSource.volume = Mathf.Clamp01(volume);
    }

    private AudioSource CreateAudioSource(bool isLooping)
    {
        GameObject audioSourceObject = new GameObject(AudioSourceObjectName);

        if (Camera.main != null)
        {
            audioSourceObject.transform.SetParent(Camera.main.transform);
        }

        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = isLooping;

        return audioSource;
    }
}