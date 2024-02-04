using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioGroup
{
    Sound,
    Voice,
    Music,
    None
}

public static class AudioController
{
    public static readonly AudioMixer audioMixer = Resources.Load<AudioMixer>("AudioMixer");
    public static readonly GlobalAudioClips globalAudioClips = Resources.Load<GlobalAudioClips>("GlobalAudioClips");
    static AudioPool _audioPool;

    const float MIN_VOLUME = 0.00001f;
    
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        _audioPool = null;
    }
#endif

    public static void SetMasterVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VOLUME, 1.0f);
        audioMixer.SetFloat("masterVol", Mathf.Log10(value) * 20);
    }

    public static void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VOLUME, 1.0f);
        audioMixer.SetFloat("musicVol", Mathf.Log10(value) * 20);
    }

    public static void SetSoundVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VOLUME, 1.0f);
        audioMixer.SetFloat("soundVol", Mathf.Log10(value) * 20);
    }

    public static void SetVoiceVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VOLUME, 1.0f);
        audioMixer.SetFloat("voiceVol", Mathf.Log10(value) * 20);
    }

    public static AudioSource Play(AudioClip clip, AudioGroup group, bool looping = false, bool spatial = false, Vector3 pos = default, bool ignorePause = false)
    {
        if (_audioPool == null)
        {
            _audioPool = new GameObject("Audio Pool").AddComponent<AudioPool>();
        }

        AudioSource soundSource = _audioPool.pool.Get();

        switch (group)
        {
            case AudioGroup.Sound:
                soundSource.gameObject.name = "Sound/" + clip.name;
                soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Sound")[0];
                break;
            case AudioGroup.Voice:
                soundSource.gameObject.name = "Sound/" + clip.name;
                soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Voice")[0];
                break;
            case AudioGroup.Music:
                soundSource.gameObject.name = "Music/" + clip.name;
                soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
                break;
            case AudioGroup.None:
            default:
                soundSource.gameObject.name = clip.name;
                soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
                break;
        }

        soundSource.ignoreListenerPause = ignorePause;

        soundSource.transform.parent = _audioPool.transform;
        soundSource.transform.position = pos;

        soundSource.spatialize = spatial;
        soundSource.spatialBlend = spatial ? 1 : 0;
        soundSource.rolloffMode = AudioRolloffMode.Linear;

        soundSource.time = 0;
        soundSource.clip = clip;
        soundSource.loop = looping;
        soundSource.Play();

        if (!looping)
        {
            _audioPool.ReleaseAfter(soundSource, clip.length);
        }

        return soundSource;
    }

    public static void StopAudio(AudioSource audio)
    {
        _audioPool.Release(audio);
    }
}