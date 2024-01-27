using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseEntrance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered!");
        if (other.CompareTag("Human"))
        {
            Debug.Log("Got here");
            
            if(other.GetComponent<Human>() && other.GetComponent<Human>().currentState == Human.HumanState.RunningToHouse)
            {
                Debug.Log("Got here2");
                if (transform.parent.GetComponent<House>() != null)
                {
                    transform.parent.GetComponent<House>().residentsAmount++;
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
