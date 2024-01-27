using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    int residentsAmount;
    public int maxResidents;
    
    
    // Start is called before the first frame update
    void Start()
    {
        residentsAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player hit trigger");
            Rigidbody[] rbList = gameObject.GetComponentsInChildren<Rigidbody>();
            Vector3 playerDirection = other.transform.forward;

            foreach (Rigidbody rb in rbList)
            {
                rb.isKinematic = false;
                Vector3 forceDirection = new Vector3(playerDirection.x, playerDirection.y + 10.0f, playerDirection.z);
                rb.AddForce(Vector3.Normalize(forceDirection), ForceMode.Impulse);
            }
        }
    }
}
