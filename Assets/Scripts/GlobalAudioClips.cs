using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Audio Clips", menuName = "Global Audio Clips", order = 0)]
public class GlobalAudioClips : ScriptableObject
{
    [SerializeField] AudioClip _pressClip;
    public AudioClip pressClip => _pressClip;

    [SerializeField] private AudioClip _selectClip;
    public AudioClip selectClip => _selectClip;

    [SerializeField] private AudioClip _slideClip;
    public AudioClip slideClip => _slideClip;
}