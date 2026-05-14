using Game.Runtime;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Startup")]
    [SerializeField] private AudioClip _defaultBGM;

    private AudioService _audioService;

    private void Awake()
    {
        _audioService = ServiceResolver.Resolve<AudioService>();
        _audioService.PlaySFXRequested       += OnPlaySFX;
        _audioService.PlayMusicRequested     += OnPlayMusic;
        _audioService.StopMusicRequested     += OnStopMusic;
        _audioService.SetMusicVolumeRequested += OnSetMusicVolume;
    }

    private void Start()
    {
        if (_defaultBGM != null)
            OnPlayMusic(_defaultBGM, true);
    }

    private void OnDestroy()
    {
        if (_audioService == null) return;
        _audioService.PlaySFXRequested       -= OnPlaySFX;
        _audioService.PlayMusicRequested     -= OnPlayMusic;
        _audioService.StopMusicRequested     -= OnStopMusic;
        _audioService.SetMusicVolumeRequested -= OnSetMusicVolume;
    }

    private void OnPlaySFX(AudioClip clip) => _sfxSource.PlayOneShot(clip);
    private void OnPlayMusic(AudioClip clip, bool loop)
    {
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }
    private void OnStopMusic() => _musicSource.Stop();
    private void OnSetMusicVolume(float volume) => _musicSource.volume = volume;
}