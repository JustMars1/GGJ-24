using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioCreateSource
{
    public static AudioSource CreateAudioSource(Vector3 position)
    {
        GameObject audioGo = new GameObject();
        AudioSource audioSource = audioGo.AddComponent<AudioSource>();
        audioGo.transform.position = position;

        return audioSource;
    }
}