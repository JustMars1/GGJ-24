using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    public AudioClip testClip;

    // Start is called before the first frame update
    void Start()
    {
        AudioController.PlayMusic(testClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        //AudioController.PlaySound(testClip, Camera.main.transform.position);
    }
}
