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
        AudioController.PlaySound(leftFootstep, transform.position, false);
    }

    public void PlayRightFootstep()
    {
        AudioController.PlaySound(rightFootstep, transform.position, false);
    }
}
