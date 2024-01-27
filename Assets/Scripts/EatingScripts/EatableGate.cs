using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatableGate : Eatable
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(CheckThreshold())
            {
                //Consume();
            }
        }
    }
}
