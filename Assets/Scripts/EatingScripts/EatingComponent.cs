using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Requires a trigger collider to be able to eat. Also the eatable object needs to have a collider to be eaten.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EatingComponent : MonoBehaviour
{
    private Animator animator;

    public Transform eatPoint;
    public string eatAnimationName;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Eat(Eatable eatableComponent)
    {
        animator.Play(eatAnimationName);
        eatableComponent.Consume(eatPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        Eatable eatableComponent = other.GetComponent<Eatable>();
        
        if (eatableComponent != null)
        {
            // Eat only if the crock is bigger than the threshold
            if(eatableComponent.CheckThreshold())
            {
                Eat(eatableComponent);
            }
        }
    }
}
