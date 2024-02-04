using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.Utilities;

public static class AudioController
{
    public static AudioSource musicSource;

    public static readonly AudioMixer audioMixer = Resources.Load<AudioMixer>("AudioMixer");

    private static List<AudioSource> audioSources = new List<AudioSource>();
    private static int noOfTimesFailed = 0;

    private static List<AudioSource> pausedAudios = new List<AudioSource>();

    public static void ClearCachedSources() 
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
            {
                AudioSource.Destroy(source.gameObject);
            }
        }
        audioSources.Clear();
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        musicSource = null;
        noOfTimesFailed = 0;
        audioSources.Clear();
    }
#endif

    public static void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("masterVol", Mathf.Log10(value) * 20);
    }

    public static void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVol", Mathf.Log10(value) * 20);
    }

    public static void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("soundVol", Mathf.Log10(value) * 20);
    }

    public static void SetVoiceVolume(float value)
    {
        audioMixer.SetFloat("voiceVol", Mathf.Log10(value) * 20);
    }

    /// <summary>
    /// Loopataan soundSource taulukon l�pi, onko joku audio source k�ytt�m�tt�.
    /// Jos on, k�ytet��n sit� ja soitetaan ��ni efekti.
    /// Mik�li ei ole k�ytt�m�t�nt� source, luodaan uusi.
    /// </summary>
    /// <param name="soundEffect">��niefekti joka halutaan soittaa.</param>
    /// <param name="position">Lokaatio mihin ��nisource luodaan. Vaikuttaa siihen mist� suunnasta ��ni kuuluu.</param>
    public static void PlaySound(AudioClip soundEffect, Vector3 position, bool isLoopable)
    {
        // On yksi tai useampi audio source
        if(audioSources.Count > 0)
        {
            foreach (AudioSource source in audioSources)
            {
                // On audiosource vapaana
                if (!source.isPlaying)
                {
                    if(isLoopable)
                    {
                        source.loop = true;
                    } else
                    {
                        source.loop = false;
                    }
                    source.clip = soundEffect;
                    source.Play();
                    break;
                }
                else
                {
                    noOfTimesFailed++;
                }
            }

            // On yksi tai useampi audio source mutta yksik��n ei ole vapaana
            if(noOfTimesFailed >= audioSources.Count)
            {
                AudioSource soundSource = CreateAudioSource(position);
                audioSources.Add(soundSource);
                soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];

                soundSource.clip = soundEffect;
                soundSource.Play();
            }
        }
        // Ei yht�k��n audio sourcea
        else
        {
            AudioSource soundSource = CreateAudioSource(position);
            audioSources.Add(soundSource);
            soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];

            soundSource.clip = soundEffect;
            soundSource.Play();
        }

        noOfTimesFailed = 0;
    }

    /// <summary>
    /// Luo audio sourcen musiikkia varten ja parenttaa sen kameraan, koska siin� on audiolistener.
    /// </summary>
    /// <param name="musicClip">Musiikki klippi joka soitetaan.</param>
    public static void PlayMusic(AudioClip musicClip)
    {
        if(musicSource == null)
        {
            musicSource = CreateAudioSource(Camera.main.transform.position);
            musicSource.transform.parent = Camera.main.transform;
            musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public static AudioSource CreateAudioSource(Vector3 position)
    {
        GameObject audioGo = new GameObject();
        AudioSource audioSource = audioGo.AddComponent<AudioSource>();
        audioGo.transform.position = position;

        return audioSource;
    }

    public static void StopPlayingSound(AudioClip soundEffect)
    {
        foreach (AudioSource source in audioSources)
        {
            // Check if the AudioSource is playing the specified AudioClip
            if (source.isPlaying && source.clip == soundEffect)
            {
                // Stop the AudioSource
                source.Stop();
                break;
            }
        }
    }

    public static void PausePlayingSounds()
    {
        foreach (AudioSource source in audioSources)
        {
            // Check if the AudioSource is playing
            if (source.isPlaying)
            {
                // Pause the AudioSource
                source.Pause();
                pausedAudios.Add(source);
            }
        }
    }

    public static void ResumePausedAudio()
    {
        List<AudioSource> sourcesToRemove = new List<AudioSource>();

        foreach (AudioSource paused in pausedAudios)
        {
            // Check if the AudioSource is playing
            if (paused != null)
            {
                // Resume playing the AudioSource
                paused.Play();
                sourcesToRemove.Add(paused);
            }
        }

        // Remove resumed sources from pausedAudios
        foreach (AudioSource removedSource in sourcesToRemove)
        {
            pausedAudios.Remove(removedSource);
        }
    }
}