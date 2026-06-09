using UnityEngine;
using AudioSystem;

public sealed class EnemyAudioController : MonoBehaviour
{
    [Header("Basic Attack Sounds")]
    [SerializeField] private AudioClip _attackHitSound;
    [SerializeField] private AudioClip _attackMissSound;

    [Header("Special Attack Sounds")]
    [SerializeField] private AudioClip _specialAttackHitSound;
    [SerializeField] private AudioClip _specialAttackMissSound;

    [Header("Damage Sounds")]
    [SerializeField] private AudioClip _hurtSound;
    [SerializeField] private AudioClip _deathSound;

    private IAudioPlayer _audioPlayer;

    private void Awake()
    {
        InitializeAudioPlayer();
    }

    private void InitializeAudioPlayer()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioPlayer = new AudioPlayer(audioSource);
    }

    public void PlayAttackHit()
    {
        PlaySound(_attackHitSound);
    }

    public void PlayAttackMiss()
    {
        PlaySound(_attackMissSound);
    }

    public void PlaySpecialAttackHit()
    {
        PlaySound(_specialAttackHitSound);
    }

    public void PlaySpecialAttackMiss()
    {
        PlaySound(_specialAttackMissSound);
    }

    public void PlayHurt()
    {
        PlaySound(_hurtSound);
    }

    public void PlayDeath()
    {
        PlaySound(_deathSound);
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        _audioPlayer.PlayOneShot(audioClip);
    }

    public void PlayDeathSound()
    {
        PlaySound(_deathSound);
    }

    public void SetAttackHit(AudioClip audioClip)
    {
        _attackHitSound = audioClip;
    }

    public void SetAttackMiss(AudioClip audioClip)
    {
        _attackMissSound = audioClip;
    }

    public void SetSpecialAttackHit(AudioClip audioClip)
    {
        _specialAttackHitSound = audioClip;
    }

    public void SetSpecialAttackMiss(AudioClip audioClip)
    {
        _specialAttackMissSound = audioClip;
    }

    public void SetHurt(AudioClip audioClip)
    {
        _hurtSound = audioClip;
    }

    public void SetDeath(AudioClip audioClip)
    {
        _deathSound = audioClip;
    }
}
