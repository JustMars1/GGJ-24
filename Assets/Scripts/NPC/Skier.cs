using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Skier : Eatable
{
    [SerializeField] 
    LayerMask _slopeMask;

    private Vector3 _globalSlopeDirection = Vector3.forward;
    private Rigidbody _rb;
    private bool _isOnRamp;
    private bool _isOnSlope;
    private bool _isOnGround;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isOnGround = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit1, 1.2f);
        
        _rb.AddForce(Vector3.down*9.81f, ForceMode.Acceleration);
        if (_isOnGround)
        {
            _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
        }
        
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit2, 1.2f, _slopeMask))
        {
            Debug.Log("Skier On Slope");

            Vector3 slopeAxis = Vector3.Cross(_globalSlopeDirection, hit2.normal);
            Vector3 slopeSlideDir = Vector3.Cross(hit2.normal, slopeAxis);
            Debug.DrawLine(transform.position, transform.position + slopeSlideDir, Color.yellow);
            
            _rb.MovePosition(_rb.position + Time.fixedDeltaTime * 20.0f * slopeSlideDir);

            if (Vector3.Dot(slopeSlideDir, Vector3.up) > 0.0f)
            {
                _isOnRamp = true;
            }

            _isOnSlope = true;
        }
        else
        {
            if (_isOnRamp)
            {
                Debug.Log("Launch off ramp!");
                _rb.AddForce(_globalSlopeDirection*15.0f+Vector3.up*5.0f, ForceMode.Impulse);
                _isOnRamp = false;
            }

            _isOnSlope = false;
        }
    }
}
