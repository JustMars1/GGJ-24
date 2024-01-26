using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioController
{
    public static AudioSource musicSource;

    public static readonly AudioMixer audioMixer = Resources.Load<AudioMixer>("AudioMixer");

    private static List<AudioSource> audioSources = new List<AudioSource>();
    private static int noOfTimesFailed = 0;

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

    /// <summary>
    /// Loopataan soundSource taulukon l�pi, onko joku audio source k�ytt�m�tt�.
    /// Jos on, k�ytet��n sit� ja soitetaan ��ni efekti.
    /// Mik�li ei ole k�ytt�m�t�nt� source, luodaan uusi.
    /// </summary>
    /// <param name="soundEffect">��niefekti joka halutaan soittaa.</param>
    /// <param name="position">Lokaatio mihin ��nisource luodaan. Vaikuttaa siihen mist� suunnasta ��ni kuuluu.</param>
    public static void PlaySound(AudioClip soundEffect, Vector3 position)
    {
        if(audioSources.Count > 0)
        {
            foreach (AudioSource source in audioSources)
            {
                if (!source.isPlaying)
                {
                    source.clip = soundEffect;
                    source.Play();
                    break;
                }
                else
                {
                    noOfTimesFailed++;
                }
            }

            if(noOfTimesFailed >= audioSources.Count)
            {
                audioSources.Add(AudioCreateSource.CreateAudioSource(position));
            }
        }
        else
        {
            audioSources.Add(AudioCreateSource.CreateAudioSource(position));
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
            musicSource = AudioCreateSource.CreateAudioSource(Camera.main.transform.position);
            musicSource.transform.parent = Camera.main.transform;
        }

        musicSource.clip = musicClip;
        musicSource.Play();
    }
}