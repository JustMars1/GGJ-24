using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Requires a trigger collider to be able to eat. Also the eatable object needs to have a collider to be eaten.
/// </summary>
//[RequireComponent(typeof(Animator))]
public class EatingComponent : MonoBehaviour
{
    [SerializeField] 
    private Animator animator;

    public Transform eatPoint;
    public PlayerController playerController;

    // Audio
    public AudioClip[] eatSoundList;

    void Start()
    {
        //animator = GetComponent<Animator>();
    }

    public void Eat(Eatable eatableComponent)
    {
        if (playerController.isOnSlope)
        {
            animator.SetBool("slideEat", true);
        }
        else
        {
            animator.SetBool("startEat", true);
        }
        eatableComponent.Consume(eatPoint);
        print("eat");
        PlayEatingSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        Eatable eatableComponent = other.GetComponent<Eatable>();
        
        if (eatableComponent != null)
        {
            // Eat only if the crock is bigger than the threshold
            if(eatableComponent.CheckThreshold())
            {
                eatableComponent.GetComponent<Collider>().enabled = false;
                Eat(eatableComponent);
            }
        }
    }

    public void PlayEatingSound()
    {
        if (eatSoundList.Length > 0)
        {
            AudioController.Play(eatSoundList[Random.Range(0, eatSoundList.Length)], AudioGroup.Sound);
        }
    }
}