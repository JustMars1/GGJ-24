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
        if(other.CompareTag("Human"))
        {
            if(transform.parent.GetComponent<House>() != null)
            {
                transform.parent.GetComponent<House>().residentsAmount++;
                Destroy(other);
            }
        }
    }
}
