using System;
using UnityEngine;

public class AudioService
{
    public event Action<AudioClip> PlaySFXRequested;
    public event Action<AudioClip, bool> PlayMusicRequested;
    public event Action StopMusicRequested;
    public event Action<float> SetMusicVolumeRequested;

    public void PlaySFX(AudioClip clip) => PlaySFXRequested?.Invoke(clip);
    public void PlayMusic(AudioClip clip, bool loop = true) => PlayMusicRequested?.Invoke(clip, loop);
    public void StopMusic() => StopMusicRequested?.Invoke();
    public void SetMusicVolume(float volume) => SetMusicVolumeRequested?.Invoke(volume);
}