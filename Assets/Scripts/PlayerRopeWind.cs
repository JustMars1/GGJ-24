using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRopeWind : MonoBehaviour
{
    public bool inWindZone;
    Rigidbody rb;

    public int windStrength;

    private float startingTimer = 5.0f;
    private float windTimer;

    private bool directionFormed;
    Vector2 random = Vector2.zero;
    Vector3 windDir = Vector3.zero;

    [SerializeField]
    private Animator animator;

    public HingeJoint hingeJoint;

    void Start()
    {
        windTimer = startingTimer;
        directionFormed = false;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(inWindZone)
        {
            if(windTimer <= 1)
            {
                if (!directionFormed)
                {   
                    random = Random.insideUnitCircle;
                    windDir = new Vector3(random.x, 0, random.y);

                    print(random);
                    directionFormed = true;
                }

                if(windTimer <= 0)
                {
                    windTimer = startingTimer;
                    directionFormed = false;
                }

                rb.AddForce(windDir * windStrength);
            }

            windTimer -= Time.deltaTime;
        }
    }

    public void DetatchRope()
    {
        Destroy(hingeJoint);
        animator.SetBool("droppedFromHeli", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("windArea"))
        {
            inWindZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("windArea"))
        {
            inWindZone = false;
        }
    }
}
