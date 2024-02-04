using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocFootsteps : MonoBehaviour
{
    // Audio
    public AudioClip rightFootstep;
    public AudioClip leftFootstep;

    public void PlayLeftFootstep()
    {
        AudioController.Play(leftFootstep, AudioGroup.Sound);
    }

    public void PlayRightFootstep()
    {
        AudioController.Play(rightFootstep, AudioGroup.Sound);
    }
}
